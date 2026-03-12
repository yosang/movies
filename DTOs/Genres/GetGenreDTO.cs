using System.ComponentModel.DataAnnotations;

namespace movies.DTOs;

public class GetGenreDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}