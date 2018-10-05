using System.Threading;
using System.Threading.Tasks;
using Collector.DAO.Entities;
using Collector.DAO.Extentions;
using Microsoft.EntityFrameworkCore;

namespace Collector.DAO.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DataContext(DbContextOptions options) : base(options)
        {
            
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyAuditInformation();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
