using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.BL.Models.Debt
{
    public class ChangesReturnDTO
    {
        public List<ChangeReturnDTO> Changes { get; set; }
        public bool HasMore { get; set; }
    }
}
