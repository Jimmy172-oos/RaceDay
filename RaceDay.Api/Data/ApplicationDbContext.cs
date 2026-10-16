using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;

namespace RaceDay.Api.Data
{
    // ApplicationDbContext is the bridge between our C# classes and the SQL Server database.
    public class ApplicationDbContext : DbContext
    {
        // Constructor: it receives the options (like the connection string)
        // and passes them to the base DbContext class.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Each DbSet becomes one table in the database.
        // (= null! only removes the yellow warning, EF Core fills these in for us)
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;

        // Extra rules so the code-first database matches the Part 1 SQL script.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- Users ----------
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role", "[Role] IN ('Organizer', 'Participant')"));
            });

            // ---------- Events ----------
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Events_EventType", "[EventType] IN ('Running', 'Walking', 'Cycling')"));

                // One Organizer (User) can create many Events
                entity.HasOne(e => e.Organizer)
                      .WithMany()
                      .HasForeignKey(e => e.OrganizerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- Categories ----------
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                // One Event has many Categories (deleting an event deletes its categories)
                entity.HasOne(c => c.Event)
                      .WithMany(e => e.Categories)
                      .HasForeignKey(c => c.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------- Enrollments ----------
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.Property(e => e.EnrollmentDate)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Status).HasDefaultValue("Pending");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Enrollments_Status", "[Status] IN ('Confirmed', 'Pending', 'Cancelled')"));

                // A participant cannot enroll in the same category twice
                entity.HasIndex(e => new { e.ParticipantId, e.CategoryId }).IsUnique();

                entity.HasOne(e => e.Participant)
                      .WithMany()
                      .HasForeignKey(e => e.ParticipantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany()
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- Results ----------
            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(r => r.CreatedAt)
                      .HasColumnType("datetime")
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(r => r.Status).HasDefaultValue("Pending");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Results_Status", "[Status] IN ('Finished', 'DNF', 'DNS', 'Pending')"));

                // One Enrollment has exactly one Result
                entity.HasOne(r => r.Enrollment)
                      .WithOne(e => e.Result)
                      .HasForeignKey<Result>(r => r.EnrollmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------- Profiles ----------
            modelBuilder.Entity<Profile>(entity =>
            {
                // One User has one Profile
                entity.HasOne(p => p.User)
                      .WithOne(u => u.Profile)
                      .HasForeignKey<Profile>(p => p.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}