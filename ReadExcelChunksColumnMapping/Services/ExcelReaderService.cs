
using ExcelDataReader;
using global::ReadExcelChunksFuncColumnMapping.Model;
using global::ReadExcelChunksFuncColumnMapping.Services.Interfaces;
using ReadExcelChunksFuncColumnMapping.Mapping.Interfaces;
using ReadExcelChunksFuncColumnMapping.Mapping.LoanExcelFunctionApp.Mapping;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ReadExcelChunksFuncColumnMapping.Services
{
    /// <summary>
    /// Service responsible for reading loan Excel files,
    /// mapping data to domain models, and triggering validations.
    /// </summary>
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly IMainLoanValidationService mainValidation;
        private readonly ISubLoanValidationService subValidation;
        private readonly IExcelRowAdapter adapter;
        public ExcelReaderService(IMainLoanValidationService mainValidation,
            ISubLoanValidationService subValidation,
            IExcelRowAdapter adapter)
        {
            this.mainValidation = mainValidation;
            this.subValidation = subValidation;
            this.adapter = adapter;
        }

        /// <summary>
        /// Reads the Excel file
        /// </summary>
        public ExcelReadResult ReadExcel(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ReadExcelInChunks(stream);
            }
        }

        /// <summary>
        /// Reads the Excel file in chunks, extracts sub-loan data using
        /// config-driven column mapping, and performs validation.
        /// </summary>
        public ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize = 300)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new ExcelReadResult();
            var borrowers = new List<SubBorrowerDetail>();

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                int rowIndex = 0;
                int headerRowIndex = -1;

                // Column name -> index
                Dictionary<string, int> columnIndexes = new Dictionary<string, int>();
                var chunk = new List<SubBorrowerDetail>();

                while (reader.Read())
                {

                    // Detect header row & Read header 
                    if (headerRowIndex == -1)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var cell = reader.GetValue(i)?.ToString();

                            if (Normalize(cell) == Normalize("Primary VGD"))
                            {
                                headerRowIndex = rowIndex;
                                columnIndexes = BuildColumnIndex(reader);
                                break;
                            }
                        }
                    }
                    else
                    {
                        /* old code with column name hard haded*/
                        //var detail = new SubBorrowerDetail
                        //{
                        //    PrimaryVGD = GetString(reader, columnIndexes, "Primary VGD"),
                        //    IFRS9Stage = GetString(reader, columnIndexes, "IFRS 9 Stage"),
                        //    TypeOfCreditInstrument = GetString(reader, columnIndexes, "Type of Credit Instrument"),
                        //    PRSSubLimit = GetDecimal(reader, columnIndexes, "PRS Sub-Limit"),
                        //    LoanStartDate = GetDate(reader, columnIndexes, "Loan Start Date"),
                        //    LoanEndDate = GetDate(reader, columnIndexes, "Loan End Date")
                        //};

                        // Build dictionary for the row
                        var rowDict = new Dictionary<string, object>();

                        foreach (var kv in columnIndexes)
                        {
                            var columnName = kv.Key;
                            var columnIndex = kv.Value;

                            rowDict[columnName] = reader.GetValue(columnIndex);
                        }

                        // Map using config-driven adapter using ColumnMappingOptions and 
                        var detail = this.adapter.Map(rowDict);

                        chunk.Add(detail);

                        // When chunk size reached
                        if (chunk.Count >= chunkSize)
                        {
                            subValidation.Validate(chunk, result.Validation);
                            borrowers.AddRange(chunk);
                            chunk.Clear();
                        }
                    }

                    rowIndex++;
                }

                // Process remaining rows
                if (chunk.Count > 0)
                {
                    subValidation.Validate(chunk, result.Validation);
                    borrowers.AddRange(chunk);
                }

                result.Borrowers = borrowers;
            }

            return result;
        }

        private Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        {
            var indexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var header = reader.GetValue(i)?.ToString();

                if (!string.IsNullOrWhiteSpace(header))
                {
                    indexes[header.Trim()] = i;
                }
            }

            return indexes;
        }

        //private Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        //{
        //    var indexes = new Dictionary<string, int>();

        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        var header = reader.GetValue(i)?.ToString();

        //        if (!string.IsNullOrWhiteSpace(header))
        //        {
        //            indexes[Normalize(header.Trim())] = i;
        //        }
        //    }

        //    return indexes;
        //}

        /// <summary>
        /// Detect multiple Template
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>


        //private Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        //{
        //    var dict = new Dictionary<string, int>();

        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        var header = reader.GetValue(i)?.ToString();
        //        if (!string.IsNullOrWhiteSpace(header))
        //        {
        //            dict[Normalize(header)] = i;
        //        }
        //    }

        //    return dict;
        //}

        /// <summary>
        /// Get String from cell
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetString(IExcelDataReader reader, Dictionary<string, int> columns, string name)
        {
            var key = Normalize(name);

            if (columns.TryGetValue(key, out var index))
            {
                return reader.GetValue(index)?.ToString();
            }

            return null;
        }

        /// <summary>
        /// Get Decimal column data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private decimal GetDecimal(IExcelDataReader reader,
            Dictionary<string, int> columns,
            string name)
        {
            var value = GetString(reader, columns, name);
            return decimal.TryParse(value, out var result) ? result : 0;
        }

        /// <summary>
        /// Get Date column data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private DateTime? GetDate(IExcelDataReader reader,
            Dictionary<string, int> columns,
            string name)
        {
            DateTime result;
            var value = GetString(reader, columns, name);
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Normalize string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // Replace all whitespace characters (tabs, CR, LF, etc.)
            var normalized = value
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .Replace("\u00A0", " ") // non-breaking space
                .Trim();

            // Collapse multiple spaces into one
            while (normalized.Contains("  "))
            {
                normalized = normalized.Replace("  ", " ");
            }

            return normalized.ToLowerInvariant();
        }


    }
}



