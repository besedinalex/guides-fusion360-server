using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GuidesFusion360Server.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        
        public DbSet<Guide> Guides { get; set; }
        
        public DbSet<PartGuide> PartGuides { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().
                HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
