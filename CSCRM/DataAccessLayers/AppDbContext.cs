using CSCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CSCRM.DataAccessLayers
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Itinerary> Itineraries { get; set; }
        public DbSet<Tour> Tours { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.SinglePrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.DoublePrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TriplePrice).HasColumnType("decimal(18, 2)");
            });
        }
    }
}
