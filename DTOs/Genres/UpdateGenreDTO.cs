using System.ComponentModel.DataAnnotations;

namespace movies.DTOs;

public class UpdateGenreDTO
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
}