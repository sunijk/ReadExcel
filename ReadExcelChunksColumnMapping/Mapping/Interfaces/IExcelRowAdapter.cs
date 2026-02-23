using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Mapping.Interfaces
{
    public interface IExcelRowAdapter
    {
        SubBorrowerDetail Map(Dictionary<string, object> row);
    }
}
