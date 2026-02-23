using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.Services.Interfaces
{
    public interface ISubLoanValidationService
    {
        void Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation);
    }
}
