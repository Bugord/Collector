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
        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackMessage> FeedbackMessages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Change> Changes { get; set; }
        public DbSet<FieldChange> FieldChanges { get; set; }
        public DbSet<Upload> Uploads { get; set; }
        public DbSet<PayNotification> PayNotifications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friend>(builder =>
            {
                builder.HasOne(relatedType => relatedType.Owner).WithMany(user => user.Friends)
                    .HasForeignKey(friend => friend.OwnerId);
            });

            modelBuilder.Entity<Friend>().HasOne(friend => friend.Owner).WithMany(user => user.Friends)
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Debt>().HasMany(debt => debt.Changes).WithOne(change => change.ChangedDebt)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Change>().HasMany(change => change.FieldChanges).WithOne(change => change.Change)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Friend>().HasMany(friend => friend.Debts).WithOne(debt => debt.Friend)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ChangeTracker.ApplyAuditInformation();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}