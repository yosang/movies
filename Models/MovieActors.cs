namespace movies.Models;

public class MovieActor
{
    public int MovieId { get; set; }
    public int ActorId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}