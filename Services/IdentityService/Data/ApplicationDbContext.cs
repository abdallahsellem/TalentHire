using Microsoft.EntityFrameworkCore;
using TalentHire.Services.IdentityService.Models;
namespace TalentHire.Services.IdentityService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Add DbSets for each entity
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<User> Users { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Fluent API configurations (if needed)
        }
    }
}
