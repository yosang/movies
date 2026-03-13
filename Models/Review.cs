using movies.Models;

public class Review
{
    public int Id { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }

    public int MovieId { get; set; } // FK

    public Movie? Movie { get; set; }
}