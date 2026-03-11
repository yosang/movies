namespace movies.Models;

public class Studio
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Movie>? Movies { get; set; }
}