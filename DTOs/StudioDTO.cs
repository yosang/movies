using System.ComponentModel.DataAnnotations;

public class StudioDTO
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;
}