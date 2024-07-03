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
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<TourByCarType> TourByCarTypes { get; set; }
        //migrate et
        public DbSet<Inclusive> InclusiveServices { get; set; }
        //migrate et
        public DbSet<Restaurant> Restaurants { get; set; }
        //migrate et
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.Property(e => e.SinglePrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.DoublePrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TriplePrice).HasColumnType("decimal(18, 2)");
            });
            modelBuilder.Entity<TourByCarType>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.Dinner).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Gala_Dinner_Foreign_Alc).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Gala_Dinner_Local_Alc).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Gala_Dinner_Simple).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.TakeAway).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Lunch).HasColumnType("decimal(18, 2)");
            });
        }
    }
}
