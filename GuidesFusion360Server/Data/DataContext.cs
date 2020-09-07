using GuidesFusion360Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GuidesFusion360Server.Data
{
    /// <summary>Class to create db with EF based on models.</summary>
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<GuideModel> Guides { get; set; }

        public DbSet<PartGuideModel> PartGuides { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<GuideModel>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.Guides);

            modelBuilder.Entity<PartGuideModel>()
                .HasOne(x => x.Guide)
                .WithMany(x => x.PartGuides);
        }
    }
}