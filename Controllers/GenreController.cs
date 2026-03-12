using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.DTOs;
using movies.Models;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class GenreController : ControllerBase
{
    private readonly MoviesContext _ctx;

    public GenreController(MoviesContext context)
    {
        _ctx = context;
    }

    /// <summary>
    /// Retrieve all genres
    /// </summary>
    /// <response code="200">Genres retrieved</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<GetGenreDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetGenreDTO>>> GetGenres(int page = 1, int pageSize = 5)
    {
        if (_ctx.Genres == null) return NotFound();

        return await _ctx.Genres.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Select(e => new GetGenreDTO { Id = e.Id, Name = e.Name })
                                .ToListAsync();
    }

    /// <summary>
    /// Get a single genre by its id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">The genre requested</response>
    /// <response code="404">Unable to find genre with provided ID</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GetGenreDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetGenreDTO>> GetGenre(int id)
    {
        if (_ctx.Genres == null) return NotFound();

        var genre = await _ctx.Genres.Where(e => e.Id == id)
                                        .Select(e => new GetGenreDTO { Id = e.Id, Name = e.Name })
                                        .FirstOrDefaultAsync();

        if (genre == null) return NotFound();

        return genre;
    }

    /// <summary>
    /// Create a new genre
    /// </summary>
    /// <param name="genreDTO"></param>
    /// <response code="201">Genre created</response>
    /// <response code="400">Missing required properties</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GetGenreDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetGenreDTO>> AddGenre(CreateGenreDTO genreDTO)
    {
        var genre = new Genre { Name = genreDTO.Name };

        _ctx.Genres.Add(genre);

        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, new GetGenreDTO { Id = genre.Id, Name = genre.Name });
    }

    /// <summary>
    /// Update a genre by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="genreDTO"></param>
    /// <response code="204">Update successful</response>
    /// <response code="400">Missing required properties</response>
    /// <response code="404">Unable to find genre with provided ID</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateGenre(int id, UpdateGenreDTO genreDTO)
    {
        var genre = await _ctx.Genres.FindAsync(id);
        if (genre == null) return NotFound();

        genre.Name = genreDTO.Name;

        await _ctx.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Removes a genre by its id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="204">Deletion successful</response>
    /// <response code="404">Unable to find genre with provided ID</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteGenre(int id)
    {
        var genre = await _ctx.Genres.FindAsync(id);
        if (genre == null) return NotFound();

        _ctx.Remove(genre);
        await _ctx.SaveChangesAsync();
        return NoContent();
    }
}