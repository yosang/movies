using System.ComponentModel.DataAnnotations;
using movies.Models;

namespace movies.DTOs;

public class UpdateReviewDTO
{
    public string Comment { get; set; } = null!;
    public int Rating { get; set; }
    public int MovieId { get; set; }
}