using ExcelDataReader;
using LoanReadExcelChunksFuncApp.Model;
using LoanReadExcelChunksFuncApp.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReadExcelChunksDurableFuncApp.Functions
{
    public class ProcessChunkActivity
    {
        private readonly ISubLoanValidationService _subValidation;

        public ProcessChunkActivity(ISubLoanValidationService subValidation)
        {
            _subValidation = subValidation;
        }

        /// <summary>
        /// Process Chunk Activity
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        [Function("ProcessChunkActivity")]
        public ValidationResult Run(
            [ActivityTrigger] ChunkInfo chunk)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var borrowers = new List<SubBorrowerDetail>();
            var validation = new ValidationResult();

            using (var stream = new FileStream(
                chunk.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var sheet = reader.AsDataSet().Tables[0];

                for (int i = chunk.StartRow; i < chunk.EndRow && i < sheet.Rows.Count; i++)
                {
                    var row = sheet.Rows[i];

                    borrowers.Add(new SubBorrowerDetail
                    {
                        PrimaryVGD = row[0]?.ToString(),
                        IFRS9Stage = row[1]?.ToString(),
                        TypeOfCreditInstrument = row[2]?.ToString(),
                        PRSSubLimit = decimal.TryParse(row[3]?.ToString(), out var d) ? d : 0m,
                        LoanStartDate = System.DateTime.TryParse(row[4]?.ToString(), out var s) ? s : (System.DateTime?)null,
                        LoanEndDate = System.DateTime.TryParse(row[5]?.ToString(), out var e) ? e : (System.DateTime?)null
                    });
                    //// Map columns to the entity model
                    //var detail = new SubBorrowerDetail
                    //{
                    //    PrimaryVGD = GetString(reader, columnIndexes, "Primary VGD"),
                    //    IFRS9Stage = GetString(reader, columnIndexes, "IFRS 9 Stage"),
                    //    TypeOfCreditInstrument = GetString(reader, columnIndexes, "Type of Credit Instrument"),
                    //    PRSSubLimit = GetDecimal(reader, columnIndexes, "PRS Sub-Limit"),
                    //    LoanStartDate = GetDate(reader, columnIndexes, "Loan Start Date"),
                    //    LoanEndDate = GetDate(reader, columnIndexes, "Loan End Date")
                    //};

                }
            }

            _subValidation.Validate(borrowers, validation);
            return validation;
        }
    }
}

//using ExcelDataReader;
//using ExcelDataReader;
//using LoanReadExcelChunksFuncApp.Model;
//using LoanReadExcelChunksFuncApp.Services.Interfaces;
//using Microsoft.Azure.Functions.Worker;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;

//namespace ReadExcelChunksDurableFuncApp.Functions
//{
//    public class ProcessChunkActivity
//    {
//        private readonly ISubLoanValidationService subValidation;

//        public ProcessChunkActivity(ISubLoanValidationService subValidation)
//        {
//            this.subValidation = subValidation;
//        }

//        [Function("ProcessChunkActivity")]
//        public ValidationResult Run(
//            [ActivityTrigger] ChunkInfo chunk)
//        {
//            var borrowers = new List<SubBorrowerDetail>();
//            var validation = new ValidationResult();

//            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//            using (var stream = new FileStream(chunk.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//            using (var reader = ExcelReaderFactory.CreateReader(stream))
//            {
//                var dataSet = reader.AsDataSet();
//                var sheet = dataSet.Tables[0];

//                for (int i = chunk.StartRow; i < chunk.EndRow; i++)
//                {
//                    var row = sheet.Rows[i];

//                    var borrower = new SubBorrowerDetail
//                    {
//                        PrimaryVGD = row[0]?.ToString(),
//                        IFRS9Stage = row[1]?.ToString(),
//                        TypeOfCreditInstrument = row[2]?.ToString(),
//                        PRSSubLimit = decimal.TryParse(row[3]?.ToString(), out var d) ? d : 0
//                    };

//                    borrowers.Add(borrower);
//                }
//            }

//            // Run validation rules
//            subValidation.Validate(borrowers, validation);

//            return validation;
//        }
//    }

//}
