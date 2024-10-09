using FreelancePay.Entities;
using Microsoft.EntityFrameworkCore;
namespace FreelancePay.Contract
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Client relationship
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany() 
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Freelancer relationship
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Freelancer)
                .WithMany() 
                .HasForeignKey(i => i.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>(t =>
            {
                t.Property(d => d.TransferDate)
                .HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<Payment>(p =>
            {
                p.Property(d => d.PaymentDate)
                .HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<Invoice>(i =>
            {
                i.Property(d => d.DueDate)
                .HasColumnType("timestamp without time zone");
            });

            modelBuilder.Entity<Invoice>(i =>
            {
                i.Property(d => d.PaidDate)
                .HasColumnType("timestamp without time zone");
            });
            modelBuilder.Entity<Invoice>(i =>
            {
                i.Property(d => d.CreatedAt)
                .HasColumnType("timestamp without time zone");
            });
        }

    }
}
