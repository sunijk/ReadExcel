using ReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncApp.Services.Interfaces
{
    public interface ISubLoanValidationService
    {
        void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation);
    }
}
