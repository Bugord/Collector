﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Collector.BL.Models.Debt
{
    public class DebtPayDTO
    {
        [Required]
        public long DebtId { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value must be not negative and less than 2 147 483 647")]
        public float? Value { get; set; }
        public string Message { get; set; }
    }
}
