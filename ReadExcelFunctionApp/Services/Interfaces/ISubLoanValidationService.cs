using ReadExcelcFunctionApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelcFunctionApp.Services.Interfaces
{
    public interface ISubLoanValidationService
    {
        List<string> Validate(List<SubBorrowerDetail> borrowers, ValidationResult validation);
    }
}
