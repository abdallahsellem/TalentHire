using Microsoft.EntityFrameworkCore;
using TalentHire.Models;
namespace TalentHire.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Add DbSets for each entity
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<RecommendedApplicant> RecommendedApplicants { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Fluent API configurations (if needed)
        }
    }
}
