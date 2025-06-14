using Microsoft.EntityFrameworkCore;
using TalentHire.Services.ApplicationsService.Models;
namespace TalentHire.Services.ApplicationsService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Add DbSets for each entity

        public DbSet<Application> Jobs { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Fluent API configurations (if needed)
        }
    }
}
