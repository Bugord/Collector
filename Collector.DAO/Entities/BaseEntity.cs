﻿using System;

namespace Collector.DAO.Entities
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public long CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? Modified { get; set; }
    }
}
