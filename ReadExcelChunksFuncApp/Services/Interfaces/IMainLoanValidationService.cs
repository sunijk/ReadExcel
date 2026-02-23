using ReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Services.Interfaces
{
    public interface IMainLoanValidationService
    {
        void Validate(MainLoanDetails mainLoan, ValidationResult validation);
    }
}
