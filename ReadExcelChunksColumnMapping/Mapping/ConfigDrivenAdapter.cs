using ReadExcelChunksFuncColumnMapping.Mapping.Interfaces;
using ReadExcelChunksFuncColumnMapping.Mapping.LoanExcelFunctionApp.Mapping;
using ReadExcelChunksFuncColumnMapping.Model;
using System;
using System.Collections.Generic;

namespace ReadExcelChunksFuncColumnMapping.Mapping
{
    /// <summary>
    /// To Map between model properties and Excel column names.
    /// Loaded from configuration.
    public class ConfigDrivenAdapter : IExcelRowAdapter
    {

        private readonly Dictionary<string, string> columnMapping;

        public ConfigDrivenAdapter(Dictionary<string, string> columnMapping)
        {
            this.columnMapping = columnMapping;
        }

        /// <summary>
        ///  Map columns from the configuration values
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public SubBorrowerDetail Map(Dictionary<string, object> row)
        {
            var model = new SubBorrowerDetail();

            var properties = typeof(SubBorrowerDetail).GetProperties();

            foreach (var prop in properties)
            {
                if (!columnMapping.ContainsKey(prop.Name))
                    continue;

                string columnName = columnMapping[prop.Name];

                if (!row.ContainsKey(columnName))
                    continue;

                var value = row[columnName];
                if (value == null)
                    continue;

                try
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    var convertedValue = Convert.ChangeType(value, targetType);
                    prop.SetValue(model, convertedValue);
                }
                catch
                {
                    //  logging
                }
            }
            return model;
        }
    }
}