using System;
using System.Collections.Generic;
using System.Text;
using Collector.DAO.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Collector.DAO.Extentions
{
    public static class ChangeTrackerExtension
    {
        public static void ApplyAuditInformation(this ChangeTracker tracker)
        {
            foreach (var entry in tracker.Entries())
            {
                if(!(entry.Entity is BaseEntity baseAudit))
                    continue;

                var now = DateTime.UtcNow;

                switch (entry.State)
                {
                    case EntityState.Added:
                        baseAudit.Created = now;
                        baseAudit.Modified = null;
                        break;
                    case EntityState.Modified:
                        baseAudit.Modified = now;
                        break;

                }
            }
        }
    }
}
