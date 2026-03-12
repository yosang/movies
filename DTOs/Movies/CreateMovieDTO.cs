using movies.Models;

namespace movies.DTOs;

public class CreateMovieDTO
{
    public string Name { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public int StudioId { get; set; }
    public ICollection<int> ActorIds { get; set; } = new List<int>() { };
}