
using ExcelDataReader;
using LoanReadExcelChunksFuncApp.Model;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReadExcelChunksDurableFuncApp.Functions
{
    public class SplitExcelActivity
    {
        private const int ChunkSize = 500;

        [Function("SplitExcelActivity")]
        public List<ChunkInfo> Run(
            [ActivityTrigger] string filePath)
        {
            var chunks = new List<ChunkInfo>();
            filePath = @"C:\Users\sunij\Desktop\EBRD-POC\LoanDetails.xlsx";

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet();
                var sheet = dataSet.Tables[0];

                int totalRows = sheet.Rows.Count;
                int start = 7; // skip header & loan header details

                while (start < totalRows)
                {
                    int end = Math.Min(start + ChunkSize, totalRows);

                    chunks.Add(new ChunkInfo
                    {
                        FilePath = filePath,
                        StartRow = start,
                        EndRow = end
                    });

                    start = end;
                }
            }
            return chunks;
        }
    }

}



////using ExcelDataReader;
////using LoanReadExcelChunksFuncApp.Model;
////using Microsoft.Azure.Functions.Worker;
////using System;
////using System.Collections.Generic;
////using System.IO;
////using System.Text;

////namespace ReadExcelChunksDurableFuncApp.Functions
////{
////    public class SplitExcelActivity
////    {
////        private const int ChunkSize = 1000;

////        [Function("SplitExcelActivity")]
////        public List<ChunkInfo> Run(
////            [ActivityTrigger] string filePath)
////        {
////            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

////            var chunks = new List<ChunkInfo>();

////            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
////            using (var reader = ExcelReaderFactory.CreateReader(stream))
////            {
////                var sheet = reader.AsDataSet().Tables[0];
////                int totalRows = sheet.Rows.Count;
////                int start = 1;   // row 0 is the column-header row — skip it

////                while (start < totalRows)
////                {
////                    int end = Math.Min(start + ChunkSize, totalRows);
////                    chunks.Add(new ChunkInfo
////                    {
////                        FilePath = filePath,
////                        StartRow = start,
////                        EndRow = end
////                    });
////                    start = end;
////                }
////            }

////            return chunks;
////        }
////    }
////}
