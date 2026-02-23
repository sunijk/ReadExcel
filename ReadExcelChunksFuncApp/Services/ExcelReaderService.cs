
using ExcelDataReader;
using global::ReadExcelChunksFuncApp.Model;
using global::ReadExcelChunksFuncApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ReadExcelChunksFuncApp.Services
{
    /// <summary>
    /// Service responsible for reading loan Excel files,
    /// mapping data to domain models, and triggering validations.
    /// </summary>
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly IMainLoanValidationService mainValidation;
        private readonly ISubLoanValidationService subValidation;
        private readonly SubLoanColumnMapping columnConfig;
        private readonly ILogger<ExcelReaderService> _logger;
        public ExcelReaderService(IMainLoanValidationService mainValidation, ISubLoanValidationService subValidation,
            IOptions<SubLoanColumnMapping> columnConfig, ILogger<ExcelReaderService> logger)
        {
            this.mainValidation = mainValidation;
            this.subValidation = subValidation;
            this.columnConfig = columnConfig.Value;
            _logger = logger;
        }

        /// <summary>
        /// Reads an Excel file from the specified file-system path.
        /// </summary>
        /// <param name="filePath"> path to the .xlsx file.</param>
        /// <returns> containing the parsed data and validation errors.</returns>

        public ExcelReadResult ReadExcel(string filePath)
        {
            _logger.LogInformation("Opening Excel file from path: {FilePath}", filePath);
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ReadExcelInChunks(stream);
            }
        }

        /// <summary>
        /// Reads the Excel file, in chinks extracts main loan and sub-borrower rows,,
        /// and performs validation.
        /// </summary>
        /// /// <returns>the parsed data and any validation errors.</returns>
        public ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize = 300)
        {
            _logger.LogInformation("Read Excel In Chunks");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new ExcelReadResult();
            var borrowers = new List<SubBorrowerDetail>();

            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {

                int rowIndex = 0;
                int headerRowIndex = -1;
                Dictionary<string, int> columnIndexes = new Dictionary<string, int>();

                var chunk = new List<SubBorrowerDetail>();

                while (reader.Read())
                {
                    _logger.LogInformation("Detect header row & Read header ");
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
                        _logger.LogDebug(" Map sub borrower columns to the entity model");

                        // Map columns to the entity model
                        var detail = new SubBorrowerDetail
                        {
                            PrimaryVGD = GetString(reader, columnIndexes, "Primary VGD"),
                            IFRS9Stage = GetString(reader, columnIndexes, "IFRS 9 Stage"),
                            TypeOfCreditInstrument = GetString(reader, columnIndexes, "Type of Credit Instrument"),
                            PRSSubLimit = GetDecimal(reader, columnIndexes, "PRS Sub-Limit"),
                            LoanStartDate = GetDate(reader, columnIndexes, "Loan Start Date"),
                            LoanEndDate = GetDate(reader, columnIndexes, "Loan End Date")
                        };

                        chunk.Add(detail);

                        // When chunk size reached
                        if (chunk.Count >= chunkSize)
                        {
                            _logger.LogDebug("Perform subloan validations");
                            // Perform subloan validations
                            this.subValidation.Validate(chunk, result.Validation);
                            borrowers.AddRange(chunk);
                            chunk.Clear();
                        }
                    }

                    rowIndex++;
                }

                _logger.LogDebug("Process remaining rows after chink size");
                // Process remaining rows
                if (chunk.Count > 0)
                {
                    // Perform subloan validations
                    this.subValidation.Validate(chunk, result.Validation);
                    borrowers.AddRange(chunk);
                }

                result.Borrowers = borrowers;
            }
            return result;
        }

        /// <summary>
        /// Builds a dictionary that maps  column header names to their  column indexes.
        /// This allows subsequent rows to be read by logical name rather
        /// than by a hard-coded index.
        /// </summary>
        private Dictionary<string, int> BuildColumnIndex(IExcelDataReader reader)
        {
            var dict = new Dictionary<string, int>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var header = reader.GetValue(i)?.ToString();
                if (!string.IsNullOrWhiteSpace(header))
                {
                    dict[Normalize(header)] = i;
                }
            }

            return dict;
        }

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
        /// Normalises a string for loose comparison by collapsing all whitespace 
        /// into single spaces and converting to lower-case.
        /// </summary>
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



