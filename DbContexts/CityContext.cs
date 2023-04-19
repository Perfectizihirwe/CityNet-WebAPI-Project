using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityContext : DbContext
    {
        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        public CityContext(DbContextOptions<CityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New York")
                {
                    Id = 1,
                    Description = "The one with that big park"
                },
                new City("Paris")
                {
                    Id = 2,
                    Description = "The one with the tall tower"
                }
            );

            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the United States."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 102-story skyscraper located in Midtown Manhattan."
                },
                new PointOfInterest("Eiffel Tower")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                },
                new PointOfInterest("The Louvre")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "The world's largest museum."
                }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}