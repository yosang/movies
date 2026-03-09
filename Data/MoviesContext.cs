using Microsoft.EntityFrameworkCore;
using movies.Models;

namespace movies.Context;

public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    // We need a constructor when using dependency injection to pass the connection string
    public MoviesContext(DbContextOptions<MoviesContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Movie>()
            .Property(e => e.Name)
            .IsRequired(); // A movie must has a name

        mb.Entity<Movie>()
            .HasData(new List<Movie>  // Seed some data through migrations
            {
                new Movie() { Id = 1, Name = "Shawshank Redemption"},
                new Movie() { Id = 2, Name = "Titanic"},
                new Movie() { Id = 3, Name = "The LionKing"},
            });
    }
}