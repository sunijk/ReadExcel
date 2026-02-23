using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Model
{
    public class ChunkInfo
    {
        public string FilePath { get; set; }
        public int StartRow { get; set; }
        public int EndRow { get; set; }
    }
}
