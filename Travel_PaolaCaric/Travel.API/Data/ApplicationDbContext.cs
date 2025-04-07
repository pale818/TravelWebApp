using Microsoft.EntityFrameworkCore;
using System;
using Travel.API.Models;


namespace Travel.API.Data
{

    //povezuje backend i bazu podataka
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext>options) : base(options) { }


        //DbSet is a from EntityFramCore, database set
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Guide> Guides { get; set; }
        //public DbSet<TripGuide> TripGuides { get; set; }


        // Makes sure EF Core uses the actual table name "Destination" and not "Destinations"
        // "Trips" is used in the code for better meaning, plurar
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Destination>().ToTable("Destination");
            modelBuilder.Entity<Trip>().ToTable("Trip");
            modelBuilder.Entity<Guide>().ToTable("Guide");





        }

    }
}
