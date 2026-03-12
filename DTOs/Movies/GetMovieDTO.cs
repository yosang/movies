namespace movies.DTOs;

public class GetMovieDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Studio { get; set; } = string.Empty;
    public ICollection<GetActorDTO> Actors { get; set; } = new List<GetActorDTO>();
}