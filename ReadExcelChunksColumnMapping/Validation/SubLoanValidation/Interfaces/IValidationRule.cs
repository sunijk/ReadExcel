using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRuleEngine.Interface
{
    public interface IValidationRule<T>
    {
        void Validate(T model, int rowNumber, ValidationResult result);
    }
}
