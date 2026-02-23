using LoanReadExcelChunksFuncApp.Mapping.Interfaces;
using LoanReadExcelChunksFuncApp.Model;
using System;
using System.Collections.Generic;

namespace LoanReadExcelChunksFuncApp.Mapping
{
    public class ConfigDrivenAdapter : IExcelRowAdapter
    {
        private readonly Dictionary<string, string> columnMapping;

        public ConfigDrivenAdapter(Dictionary<string, string> columnMapping)
        {
            this.columnMapping = columnMapping;
        }

        /// <summary>
        ///  Map columns without template
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        //public SubBorrowerDetail Map(Dictionary<string, object> row)
        //{
        //    var model = new SubBorrowerDetail();

        //    var properties = typeof(SubBorrowerDetail).GetProperties();

        //    foreach (var prop in properties)
        //    {
        //        if (!columnMapping.ContainsKey(prop.Name))
        //            continue;

        //        string columnName = columnMapping[prop.Name];

        //        if (!row.ContainsKey(columnName))
        //            continue;

        //        var value = row[columnName];
        //        if (value == null)
        //            continue;

        //        try
        //        {
        //            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        //            var convertedValue = Convert.ChangeType(value, targetType);
        //            prop.SetValue(model, convertedValue);
        //        }
        //        catch
        //        {
        //            //  logging
        //        }
        //    }

        //    return model;
        //}



        /// <summary>
        ///  Map columns with multiple template
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnMapping"></param>
        /// <returns></returns>
        public SubBorrowerDetail Map(
        Dictionary<string, object> row,
        Dictionary<string, string> columnMapping)
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
                    var targetType =
                        Nullable.GetUnderlyingType(prop.PropertyType)
                        ?? prop.PropertyType;

                    var converted = Convert.ChangeType(value, targetType);
                    prop.SetValue(model, converted);
                }
                catch
                {
                    // optional logging
                }
            }

            return model;
        }
    }
}
