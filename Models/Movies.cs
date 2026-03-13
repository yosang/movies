namespace movies.Models;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Foreign keys
    public int GenreId { get; set; }
    public int StudioId { get; set; }

    // Navigation props
    public Genre Genre { get; set; } = null!;
    public Studio Studio { get; set; } = null!;

    public List<MovieActor> MovieActors { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}