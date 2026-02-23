using ReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Services.Interfaces
{
    public interface IExcelReaderService
    {
        ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize);
        ExcelReadResult ReadExcel(string filePath);
    }

}
