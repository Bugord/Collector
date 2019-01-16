using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class FieldChangeExtentions
    {
        public static FieldChangeReturnDTO ToChangeReturnDTO(this FieldChange fieldChange)
        {
            return new FieldChangeReturnDTO
            {
                ChangedField = fieldChange.FieldName,
                NewValue = fieldChange.NewValue,
                OldValue = fieldChange.OldValue
            };
        }

        public static T HandleChange<T>(this T oldValue, T newValue, string fieldName, out FieldChange fieldChange)
        {
            fieldChange = null;
            if (oldValue.Equals(newValue)) return oldValue;
            fieldChange = new FieldChange
            {
                OldValue = oldValue.ToString(),
                NewValue = newValue.ToString(),
                FieldName = fieldName
            };
            return newValue;
        }
    }
}
