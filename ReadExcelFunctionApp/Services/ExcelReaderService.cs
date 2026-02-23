
using ExcelDataReader;
using global::ReadExcelcFunctionApp.Model;
using global::ReadExcelcFunctionApp.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ReadExcelcFunctionApp.Services
{
    /// <summary>
    /// Service responsible for opening an Excel workbook, extracting loan header data
    /// and sub-borrower records, and delegating validation to the appropriate validators.
    /// </summary>
    public class ExcelReaderService : IExcelReaderService
    {
        private readonly IMainLoanValidationService _mainValidation;
        private readonly ISubLoanValidationService _subValidation;
        private readonly SubLoanColumnMapping _columnConfig;
        private readonly ILogger<ExcelReaderService> _logger;
        public ExcelReaderService(IMainLoanValidationService mainValidation, ISubLoanValidationService subValidation,
            IOptions<SubLoanColumnMapping> columnConfig, ILogger<ExcelReaderService> logger)
        {
            _mainValidation = mainValidation;
            _subValidation = subValidation;
            _columnConfig = columnConfig.Value;
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

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return ReadExcel(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open Excel file at path: {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Reads an Excel workbook as stream.
        /// Parses the first worksheet for both main loan header details and sub-borrower rows,
        /// then runs validation on both sections.
        /// </summary>
        /// <returns>the parsed data and any validation errors.</returns>

        public ExcelReadResult ReadExcel(Stream stream)
        {
            try
            {
                _logger.LogInformation("Reading Excel stream and converting to DataSet.");

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var result = new ExcelReadResult();
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet();
                    var sheet = dataSet.Tables[0];

                    // Read Excel data for Header & Sub loan
                    result.MainLoan = ReadHeaderDetails(sheet);
                    result.Borrowers = ReadSubLoanDetails(sheet);
                }

                // Validate headear and subloan separately
                _mainValidation.Validate(result.MainLoan, result.Validation);
                _subValidation.Validate(result.Borrowers, result.Validation);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading or validating the Excel workbook.");
                throw;
            }
        }

        /// <summary>
        /// Read Header Details
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private MainLoanDetails ReadHeaderDetails(DataTable sheet)
        {
            _logger.LogDebug("Reading main loan header details from fixed cell positions.");

            var main = new MainLoanDetails();

            // Zero-based indexing:
            // B2 = [1][1]
            main.UGB = sheet.Rows[1][1]?.ToString();

            // B3
            main.SubLoansOriginated = sheet.Rows[2][1]?.ToString();

            // B4
            main.SubLoansReport = sheet.Rows[3][1]?.ToString();

            // B5
            if (DateTime.TryParse(sheet.Rows[4][1]?.ToString(), out var start))
            {
                main.ReportingPeriodStart = start;
            }

            // B6
            if (DateTime.TryParse(sheet.Rows[5][1]?.ToString(), out var end))
            {
                main.ReportingPeriodEnd = end;
            }

            return main;
        }



        /// <summary>
        /// Locates the sub-borrower header row by searching for the "Primary VGD" column header,
        /// then iterates subsequent rows to build a list of SubBorrowerDetail records.
        /// </summary>
        private List<SubBorrowerDetail> ReadSubLoanDetails(DataTable sheet)
        {
            var list = new List<SubBorrowerDetail>();

            int headerRow = FindSubBorrowerHeaderRow(sheet);

            if (headerRow == -1)
            {
                _logger.LogWarning("Sub-borrower header row not found. Returning empty list.");
                return list;
            }

            _logger.LogDebug("Sub-borrower header row found at index {Row}.", headerRow);

            // Build a column-name → column-index dictionary from the header row
            var columns = GetColumnIndexes(sheet, headerRow);

            // Iterate every row after the header; skip rows with an empty first cell
            for (int i = headerRow + 1; i < sheet.Rows.Count; i++)
            {
                var row = sheet.Rows[i];

                if (row[0] == DBNull.Value)
                {
                    _logger.LogDebug("Skipping empty row at index {Row}.", i);
                    continue;
                }

                var detail = new SubBorrowerDetail
                {
                    PrimaryVGD = GetString(row, columns, "Primary VGD"),
                    IFRS9Stage = GetString(row, columns, "IFRS 9 Stage"),
                    TypeOfCreditInstrument = GetString(row, columns, "Type of Credit Instrument"),
                    PRSSubLimit = GetDecimal(row, columns, "PRS Sub-Limit"),
                    LoanStartDate = GetDate(row, columns, "Loan Start Date"),
                    LoanEndDate = GetDate(row, columns, "Loan End Date")
                };
                list.Add(detail);
            }
            _logger.LogDebug("Sub-borrower rows parsed: {Count}.", list.Count);

            return list;
        }

        // <summary>
        /// Scans all cells in the worksheet to find the row index of the sub-borrower header,
        /// identified by a cell whose normalised value equals "Primary VGD".
        /// </summary>
        private int FindSubBorrowerHeaderRow(DataTable sheet)
        {
            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                for (int j = 0; j < sheet.Columns.Count; j++)
                {
                    var value = sheet.Rows[i][j]?.ToString();
                    if (Normalize(value) == Normalize("Primary VGD"))
                    {
                        _logger.LogDebug("'Primary VGD' header found at row {Row}, col {Col}.", i, j);
                        return i;
                    }
                  
                }
            }
            return -1;
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

        /// <summary>
        ///   /// Get Column Indexes
        /// Builds a dictionary mapping each column header name to column index,
        /// based on the content of the specified header row.
        /// </summary>
        /// <summary>

        private Dictionary<string, int> GetColumnIndexes(DataTable sheet, int headerRow)
        {
            var dict = new Dictionary<string, int>();

            for (int j = 0; j < sheet.Columns.Count; j++)
            {
                var header = sheet.Rows[headerRow][j]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(header))
                    dict[header] = j;
            }

            return dict;
        }

        /// <summary>
        /// Get String value from the cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetString(DataRow row, Dictionary<string, int> columns, string name)
        {
            return columns.ContainsKey(name)
                ? row[columns[name]]?.ToString()
                : null;
        }

        /// <summary>
        /// Get Decimal value from the cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private decimal GetDecimal(DataRow row, Dictionary<string, int> columns, string name)
        {
            if (columns.ContainsKey(name) &&
                decimal.TryParse(row[columns[name]]?.ToString(), out var val))
                return val;

            return 0;
        }

        /// <summary>
        /// Get GetDate value from the cell
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columns"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private DateTime? GetDate(DataRow row, Dictionary<string, int> columns, string name)
        {
            if (columns.ContainsKey(name) &&
                DateTime.TryParse(row[columns[name]]?.ToString(), out var val))
                return val;

            return null;
        }


    }
}



