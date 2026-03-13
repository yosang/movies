using movies.Models;

namespace movies.DTOs;

public class GetReviewDTO
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public int Rating { get; set; }
    public MovieSimpleDTO Movie { get; set; } = null!;
}