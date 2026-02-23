using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.ValidationRuleEngine.Interface
{
    public interface ISubLoanRuleFactory
    {
        List<ISubLoanRule> GetActiveValidationRules(Dictionary<string, string> columnMapping);
       // IEnumerable<ISubLoanRule> CreateRules();
    }
}
