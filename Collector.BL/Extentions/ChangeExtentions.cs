using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class ChangeExtentions
    {
        public static ChangeReturnDTO ToChangeReturnDTO(this Change change)
        {
            return new ChangeReturnDTO
            {
                ChangeTime = change.Created,
                FieldChanges = change.FieldChanges?.Select(fieldChange => fieldChange.ToChangeReturnDTO()).ToList(),
            };
        }
    }
}
