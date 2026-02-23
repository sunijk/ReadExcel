using LoanReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoanReadExcelChunksFuncApp.Services.Interfaces
{
    public interface ISubLoanValidationService
    {
        void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation);
    }
}
