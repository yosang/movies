using Microsoft.EntityFrameworkCore;
using movies.Models;

namespace movies.Context;

public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Studio> Studios { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }

    // We need a constructor when using dependency injection to pass the connection string
    public MoviesContext(DbContextOptions<MoviesContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // Key/Property requirements
        mb.Entity<Movie>().Property(e => e.Name).IsRequired(); // A movie must has a name

        mb.Entity<Genre>()
            .Property(e => e.Name).IsRequired(); // A Genre must have a name

        mb.Entity<Studio>()
            .Property(e => e.Name).IsRequired();

        mb.Entity<MovieActor>() // A join table must have composite key (Its a PK which consists of foreign keys)
            .HasKey(e => new { e.MovieId, e.ActorId });

        // One-To-Many: Movie - Genre
        mb.Entity<Movie>()
            .HasOne(e => e.Genre)
            .WithMany(e => e.Movies)
            .HasForeignKey(e => e.GenreId)
            .OnDelete(DeleteBehavior.NoAction);

        // One-To-Many: Movie - Studio
        mb.Entity<Movie>()
            .HasOne(e => e.Studio)
            .WithMany(e => e.Movies)
            .HasForeignKey(e => e.StudioId)
            .OnDelete(DeleteBehavior.NoAction);

        // Many-To-Many: Movie - MovieActors - Actors
        mb.Entity<MovieActor>() // Has one Movie
            .HasOne(e => e.Movie)
            .WithMany(ma => ma.MovieActors)
            .HasForeignKey(e => e.MovieId); // foreign key

        mb.Entity<MovieActor>() // Has one Actor
            .HasOne(e => e.Actor)
            .WithMany(ma => ma.MovieActors)
            .HasForeignKey(e => e.ActorId); // foreign key

        // Seeds
        mb.Entity<Genre>()
            .HasData(new List<Genre>()
            {
               new Genre { Id = 1, Name = "Classic"} ,
               new Genre { Id = 2, Name = "Romantic"},
               new Genre { Id = 3, Name = "Animation"}
            });

        mb.Entity<Studio>()
            .HasData(new List<Studio>()
            {
                new Studio { Id = 1, Name = "Castle Rock Entertainment"},
                new Studio { Id = 2, Name = "Baja Studios"},
                new Studio { Id = 3, Name = "Walt Disney"}
            });

        mb.Entity<Movie>()
            .HasData(new List<Movie>  // Seed some data through migrations
            {
                    new Movie() { Id = 1, Name = "Shawshank Redemption", GenreId = 1, StudioId = 1},
                    new Movie() { Id = 2, Name = "Titanic", GenreId = 2, StudioId = 2},
                    new Movie() { Id = 3, Name = "The LionKing", GenreId = 3, StudioId = 3},
            });

        mb.Entity<Actor>()
            .HasData(new List<Actor>()
            {
                new Actor { Id = 1, Name = "Morgan Freeman"},
                new Actor { Id = 2, Name = "Leonardo Dicaprio"},
                new Actor { Id = 3, Name = "Rowan Atkinson"}
            });

        mb.Entity<MovieActor>()
            .HasData(new List<MovieActor>()
            {
               new MovieActor { MovieId = 1, ActorId = 1},
               new MovieActor { MovieId = 2, ActorId = 2},
               new MovieActor { MovieId = 3, ActorId = 3}
            });
    }
}