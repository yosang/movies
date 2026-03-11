namespace movies.Models;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation props
    public ICollection<MovieActor>? MovieActors { get; set; }
}