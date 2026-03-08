using ElevatorCaseFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElevatorCaseFlow.Infrastructure.Data
{
    /// <summary>
    /// The main database context — acts as the bridge between
    /// our C# objects and the SQL Server database.
    /// Each DbSet represents one table in the database.
    /// </summary>
    public class AppDbContext : DbContext
    {
        // Constructor — receives database configuration from outside
        // This is how we keep connection strings out of the code
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // This represents the "ElevatorCases" table in SQL Server
        public DbSet<ElevatorCase> ElevatorCases { get; set; }

        /// <summary>
        /// Configures the database schema — column rules, indexes etc.
        /// Called automatically by EF Core when setting up the database.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the ElevatorCases table
            modelBuilder.Entity<ElevatorCase>(entity =>
            {
                // Set the table name explicitly
                entity.ToTable("ElevatorCases");

                // Primary key — auto increments
                entity.HasKey(e => e.Id);

                // CaseNumber must be unique — no two cases with same number
                entity.HasIndex(e => e.CaseNumber)
                      .IsUnique();

                // CaseNumber is required, max 20 characters
                entity.Property(e => e.CaseNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                // ClientName is required, max 100 characters
                entity.Property(e => e.ClientName)
                      .IsRequired()
                      .HasMaxLength(100);

                // Country is required, max 100 characters
                entity.Property(e => e.Country)
                      .IsRequired()
                      .HasMaxLength(100);

                // BuildingType is required, max 50 characters
                entity.Property(e => e.BuildingType)
                      .IsRequired()
                      .HasMaxLength(50);

                // XmlPayload can be large — no length limit
                entity.Property(e => e.XmlPayload)
                      .IsRequired();

                // RejectionReason is optional, max 500 characters
                entity.Property(e => e.RejectionReason)
                      .HasMaxLength(500);
            });
        }
    }
}

