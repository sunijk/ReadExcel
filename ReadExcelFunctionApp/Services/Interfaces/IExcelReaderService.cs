using ReadExcelcFunctionApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelcFunctionApp.Services.Interfaces
{
    public interface IExcelReaderService
    {
        ExcelReadResult ReadExcel(Stream stream);
        ExcelReadResult ReadExcel(string filePath);
    }

}
