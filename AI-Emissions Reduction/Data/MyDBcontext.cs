using AI_Emissions_Reduction.Data.Entity;
using AI_Emissions_Reduction.Data.Entity.One_to_Many;
using Microsoft.EntityFrameworkCore;

namespace AI_Emissions_Reduction.Data
{
    public class MyDBcontext : DbContext
    {
        public MyDBcontext(DbContextOptions<MyDBcontext> options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAvailability> EmployeeAvailabilities { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WasteCollection> WasteCollections { get; set; }
        public DbSet<WasteType> WasteTypes { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>()
        .HasOne(f => f.user)
        .WithOne(u => u.feedBack)
        .HasForeignKey<Feedback>(f => f.UserId)
        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Payment>()
        .HasOne(p => p.User)
        .WithMany(u => u.Payments)
        .HasForeignKey(p => p.UserId)
        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WasteCollection>()
    .HasOne(wc => wc.user)
    .WithMany(u => u.WasteCollectionRequests)
    .HasForeignKey(wc => wc.UserId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WasteCollection>()
                .HasOne(wc => wc.employee)
                .WithMany(e => e.WasteCollection)
                .HasForeignKey(wc => wc.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
