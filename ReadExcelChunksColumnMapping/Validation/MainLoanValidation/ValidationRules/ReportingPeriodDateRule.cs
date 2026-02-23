using ReadExcelChunksFuncColumnMapping.Model;
using ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelChunksFuncColumnMapping.ValidationRules.MainLoanValidation.ValidationRules
{
    public class ReportingPeriodDateRule : IMainLoanRule
    {
        /// <summary>
        /// Validates that the reporting period start date is earlier than the end date.
        /// </summary>
        public void Validate(MainLoanDetails model, ValidationResult result)
        {
            if (model.ReportingPeriodStart.HasValue &&
                model.ReportingPeriodEnd.HasValue &&
                model.ReportingPeriodStart >= model.ReportingPeriodEnd)
            {
                result.HeaderErrors.Add(
                    "Reporting Period Start Date must be less than Reporting Period End Date.");
            }
        }
    }
}
