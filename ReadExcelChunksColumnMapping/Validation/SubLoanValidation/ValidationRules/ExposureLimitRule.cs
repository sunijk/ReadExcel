using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules.ValidationRules
{
    public class ExposureLimitRule : ISubLoanRule
    {
        public void Validate(SubBorrowerDetail model, int rowNumber, ValidationResult result)
        {
            //  logic
        }
    }
}
