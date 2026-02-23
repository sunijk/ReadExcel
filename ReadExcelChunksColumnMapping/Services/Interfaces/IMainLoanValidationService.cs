using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Services.Interfaces
{
    public interface IMainLoanValidationService
    {
        void Validate(MainLoanDetails mainLoan, ValidationResult validation);
    }
}
