using Microsoft.EntityFrameworkCore;
using TalentHire.Services.JobService.Models;
namespace TalentHire.Services.JobService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Add DbSets for each entity

        public DbSet<Job> Jobs { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Fluent API configurations (if needed)
        }
    }
}
