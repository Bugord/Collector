using System;
using System.Collections.Generic;
using System.Text;
using Collector.BL.Models.Authorization;

namespace Collector.BL.Models.Debt
{
    public class ChangeReturnDTO
    {
        public IList<FieldChangeReturnDTO> FieldChanges;
        public DateTime ChangeTime;
    }
}
