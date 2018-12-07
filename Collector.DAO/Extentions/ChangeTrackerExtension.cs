using System;
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
                        break;
                    case EntityState.Modified:
                        foreach (var entry2 in entry.Properties)
                        {
                            if (entry2.Metadata.IsConcurrencyToken)
                            {
                                entry2.OriginalValue = entry2.CurrentValue;
                            }
                        }
                       
                        baseAudit.Modified = now;
                        break;
                    
                }
            }
        }
    }
}
