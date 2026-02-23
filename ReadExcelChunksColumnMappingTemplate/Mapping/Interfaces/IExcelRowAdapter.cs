using LoanReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Mapping.Interfaces
{
    public interface IExcelRowAdapter
    {
       // SubBorrowerDetail Map(Dictionary<string, object> row);
        SubBorrowerDetail Map(Dictionary<string, object> row, Dictionary<string, string> columnMapping);
    }
}
