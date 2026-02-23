using ReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.ValidationRuleEngine.Interface
{
    public interface IValidationRule<T>
    {
        void Validate(T model, int rowNumber, ValidationResult result);
    }
}
