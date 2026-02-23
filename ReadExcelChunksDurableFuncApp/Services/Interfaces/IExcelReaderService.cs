using LoanReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Services.Interfaces
{
    public interface IExcelReaderService
    {
        ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize);
        ExcelReadResult ReadExcel(string filePath);
    }

}
