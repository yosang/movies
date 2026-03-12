namespace movies.DTOs;

public class UpdateMovieDTO
{
    public string Name { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public int StudioId { get; set; }
    public List<int> Actors { get; set; } = new();
}