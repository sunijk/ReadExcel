using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Services.Interfaces
{
    public interface IExcelReaderService
    {
        ExcelReadResult ReadExcelInChunks(Stream stream, int chunkSize);
        ExcelReadResult ReadExcel(string filePath);
    }

}
