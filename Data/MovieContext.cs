using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Data
{
    public class MovieContext: DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options):base(options) { }
        
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().Property(b => b.Title).IsRequired();
            modelBuilder.Entity<Movie>().Property(b => b.Title).HasMaxLength(500);

            modelBuilder.Entity<Genre>().Property(b => b.Name).IsRequired();
            modelBuilder.Entity<Genre>().Property(b => b.Name).HasMaxLength(50);
            
        }


    }
}
