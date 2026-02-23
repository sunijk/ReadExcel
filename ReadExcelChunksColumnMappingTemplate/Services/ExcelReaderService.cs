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
        private readonly IMainLoanValidationService mainValidation;
        private readonly ISubLoanValidationService subValidation;
        private readonly IExcelRowAdapter adapter;
        private readonly ColumnMappingOptions columnMappingOptions;

        public ExcelReaderService(
            IMainLoanValidationService mainValidation,
            ISubLoanValidationService subValidation,
            IExcelRowAdapter adapter,
            ColumnMappingOptions columnMappingOptions)
        {
            this.mainValidation = mainValidation;
            this.subValidation = subValidation;
            this.adapter = adapter;
            this.columnMappingOptions = columnMappingOptions;
        }

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

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int rowIndex = 0;
                int headerRowIndex = -1;

                var columnIndexes = new Dictionary<string, int>();

                // The Mapping section from the matched template
                // (model property name → Excel column header)
                Dictionary<string, string> templateMapping = null;

                var chunk = new List<SubBorrowerDetail>();

                while (reader.Read())
                {
                    // --------------------------------------------------------
                    // Phase 1: scan rows until the header row is found
                    // --------------------------------------------------------
                    if (headerRowIndex == -1)
                    {
                        bool headerFound = false;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var cell = reader.GetValue(i)?.ToString();

                            // Look for the Excel column header that the active
                            // template maps to "PrimaryVGD"
                            if (!columnMappingOptions.Templates.TryGetValue(
                                    columnMappingOptions.ActiveTemplateName,
                                    out var activeDef))
                                throw new InvalidOperationException(
                                    $"Active template '{columnMappingOptions.ActiveTemplateName}' not found.");

                            activeDef.Mapping.TryGetValue("PrimaryVGD", out string primaryVgdHeader);

                            if (Normalize(cell) == Normalize(primaryVgdHeader))
                            {
                                headerRowIndex = rowIndex;
                                columnIndexes = BuildColumnIndex(reader);
                                headerFound = true;
                                break;
                            }
                        }

                        if (headerFound)
                        {
                            // Use the active template directly — no detection needed
                            var def = columnMappingOptions.Templates[
                                          columnMappingOptions.ActiveTemplateName];
                            templateMapping = def.Mapping;
                        }
                    }
                    // --------------------------------------------------------
                    // Phase 2: map and collect data rows  (your original logic)
                    // --------------------------------------------------------
                    else
                    {
                        var rowDict = new Dictionary<string, object>();

                        foreach (var kv in columnIndexes)
                            rowDict[kv.Key] = reader.GetValue(kv.Value);

                        var detail = adapter.Map(rowDict, templateMapping);
                        chunk.Add(detail);

                        if (chunk.Count >= chunkSize)
                        {
                            subValidation.Validate(chunk, result.Validation, templateMapping);
                            borrowers.AddRange(chunk);
                            chunk.Clear();
                        }
                    }

                    rowIndex++;
                }

                // Flush remaining rows
                if (chunk.Count > 0)
                {
                    subValidation.Validate(chunk, result.Validation, templateMapping);
                    borrowers.AddRange(chunk);
                }

                result.Borrowers = borrowers;
            }

            return result;
        }

        // ----------------------------------------------------------------
        // Helpers  (unchanged from your original)
        // ----------------------------------------------------------------

        private Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        {
            var indexes = new Dictionary<string, int>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var header = reader.GetValue(i)?.ToString();
                if (!string.IsNullOrWhiteSpace(header))
                    indexes[header.Trim()] = i;
            }

            return indexes;
        }

        private string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var normalized = value
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\u00A0", " ")
                .Trim();

            while (normalized.Contains("  "))
                normalized = normalized.Replace("  ", " ");

            return normalized.ToLowerInvariant();
        }
    }
}