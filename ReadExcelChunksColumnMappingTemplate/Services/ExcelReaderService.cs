using ExcelDataReader;
using LoanReadExcelChunksFuncApp.Mapping.Interfaces;
using LoanReadExcelChunksFuncApp.Mapping.LoanExcelFunctionApp.Mapping;
using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoanReadExcelChunksFuncApp.Services
{
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly IMainLoanValidationService _mainValidation;
        private readonly ISubLoanValidationService _subValidation;
        private readonly IExcelRowAdapter _adapter;
        private readonly Dictionary<string, string> _activeMapping;
        private readonly string _activeTemplateName;

        // How many consecutive blank rows before we treat the data as finished.
        // Handles cases where rows have formatting but no data.
        private const int MaxConsecutiveBlankRows = 5;

        public ExcelReaderService(
            IMainLoanValidationService mainValidation,
            ISubLoanValidationService subValidation,
            IExcelRowAdapter adapter,
            ColumnMappingOptions columnMappingOptions)
        {
            _mainValidation = mainValidation;
            _subValidation = subValidation;
            _adapter = adapter;

            if (columnMappingOptions.Templates.Count == 0)
                throw new InvalidOperationException(
                    "No templates loaded. Check TemplateLoader ran correctly at startup.");

            var template = columnMappingOptions.Templates.Values.First();
            _activeTemplateName = template.TemplateName;
            _activeMapping = template.Mapping;
        }

        // ----------------------------------------------------------------
        // Public API
        // ----------------------------------------------------------------

        public ExcelReadResult ReadExcel(string filePath)
        {
            using (var stream = File.Open(
                filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ReadExcelInChunks(stream);
            }
        }

        public ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize = 300)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new ExcelReadResult();
            var borrowers = new List<SubBorrowerDetail>();
            var chunk = new List<SubBorrowerDetail>();

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int rowIndex = 0;
                int headerRowIndex = -1;
                int consecutiveBlanks = 0;

                Dictionary<string, int> columnIndexes = null;

                while (reader.Read())
                {
                    // --------------------------------------------------
                    // Phase 1: find the header row
                    // --------------------------------------------------
                    if (headerRowIndex == -1)
                    {
                        if (TryDetectHeaderRow(reader, out columnIndexes))
                            headerRowIndex = rowIndex;

                        rowIndex++;
                        continue;
                    }

                    // --------------------------------------------------
                    // Phase 2: read data rows
                    // --------------------------------------------------

                    // Check whether the entire row is blank
                    bool rowIsBlank = IsRowBlank(reader);

                    if (rowIsBlank)
                    {
                        consecutiveBlanks++;

                        // Stop reading after N consecutive blank rows —
                        // this is the end of real data in the sheet.
                        if (consecutiveBlanks >= MaxConsecutiveBlankRows)
                            break;

                        rowIndex++;
                        continue;
                    }

                    // Row has data — reset blank counter
                    consecutiveBlanks = 0;

                    var rowDict = BuildRowDictionary(reader, columnIndexes);
                    var detail = _adapter.Map(rowDict, _activeMapping);
                    chunk.Add(detail);

                    // Flush chunk when size reached
                    if (chunk.Count >= chunkSize)
                    {
                        _subValidation.Validate(chunk, result.Validation, _activeMapping);
                        borrowers.AddRange(chunk);
                        chunk.Clear();
                    }

                    rowIndex++;
                }

                // Flush remaining rows that didn't fill a full chunk
                if (chunk.Count > 0)
                {
                    _subValidation.Validate(chunk, result.Validation, _activeMapping);
                    borrowers.AddRange(chunk);
                }
            }

            result.Borrowers = borrowers;
            return result;
        }

        // ----------------------------------------------------------------
        // Blank row detection
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns true if every cell in the current reader row is null or whitespace.
        /// Checks all columns so a row with data only in later columns is not skipped.
        /// </summary>
        private static bool IsRowBlank(IExcelDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i)?.ToString();
                if (!string.IsNullOrWhiteSpace(val))
                    return false;
            }
            return true;
        }

        // ----------------------------------------------------------------
        // Header detection
        // ----------------------------------------------------------------

        private bool TryDetectHeaderRow(
            IExcelDataReader reader,
            out Dictionary<string, int> columnIndexes)
        {
            columnIndexes = null;

            if (!_activeMapping.TryGetValue("PrimaryVGD", out string expectedHeader))
                throw new InvalidOperationException(
                    $"Template '{_activeTemplateName}' has no mapping for 'PrimaryVGD'.");

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string cell = reader.GetValue(i)?.ToString();
                if (Normalize(cell) == Normalize(expectedHeader))
                {
                    columnIndexes = BuildColumnIndex(reader);
                    return true;
                }
            }

            return false;
        }

        // ----------------------------------------------------------------
        // Row helpers
        // ----------------------------------------------------------------

        private static Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        {
            var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string header = reader.GetValue(i)?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(header) && !indexes.ContainsKey(header))
                    indexes[header] = i;
            }
            return indexes;
        }

        private static Dictionary<string, object> BuildRowDictionary(
            IExcelDataReader reader,
            Dictionary<string, int> columnIndexes)
        {
            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in columnIndexes)
                dict[kv.Key] = reader.GetValue(kv.Value);
            return dict;
        }

        // ----------------------------------------------------------------
        // Normaliser
        // ----------------------------------------------------------------

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var sb = new StringBuilder(value.Length);
            bool lastSpace = false;
            foreach (char c in value)
            {
                if (c == '\r' || c == '\n' || c == '\t' || c == '\u00A0' || c == ' ')
                {
                    if (!lastSpace) { sb.Append(' '); lastSpace = true; }
                }
                else
                {
                    sb.Append(char.ToLowerInvariant(c));
                    lastSpace = false;
                }
            }
            return sb.ToString().Trim();
        }
    }
}