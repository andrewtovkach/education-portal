using EducationPortal.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EducationPortal.Web.Data
{
    public class EducationPortalDbContext : DbContext
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<EducationMaterial> EducationMaterials { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<AnswerHistory> AnswerHistories { get; set; }
        public DbSet<AnswerHistoryData> AnswerHistoryData { get; set; }
        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<TestCompletion> TestCompletions { get; set; }
        public DbSet<Module> Modules { get; set; }

        public EducationPortalDbContext(DbContextOptions<EducationPortalDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Module>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Modules);

            modelBuilder.Entity<EducationMaterial>()
                .HasOne(c => c.Module)
                .WithMany(c => c.EducationMaterials);

            modelBuilder.Entity<Test>()
                .HasOne(c => c.Module)
                .WithMany(c => c.Tests);

            modelBuilder.Entity<Question>()
                .HasOne(c => c.Test)
                .WithMany(c => c.Questions);

            modelBuilder.Entity<Answer>()
                .HasOne(c => c.Question)
                .WithMany(c => c.Answers);

            modelBuilder.Entity<AnswerHistory>()
                .HasOne(c => c.AnswerHistoryData)
                .WithMany(c => c.AnswerHistories);

            modelBuilder.Entity<AnswerHistory>()
                .HasOne(c => c.Answer)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne(c => c.TestCompletion)
                .WithMany(c => c.Attempts);

            modelBuilder.Entity<AnswerHistoryData>()
                .HasOne(c => c.Attempt)
                .WithMany(c => c.AnswerHistoryData)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestCompletion>()
                .HasOne(c => c.Test)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
