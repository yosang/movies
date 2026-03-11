using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
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
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<Genre>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Genre>>> GetGenres(int page = 1, int pageSize = 2)
    {
        if (!await _ctx.Genres.AnyAsync()) return NotFound();

        int pagination = (page - 1) * pageSize;

        return await _ctx.Genres.Skip(pagination).Take(pageSize).ToListAsync(); ;
    }

    /// <summary>
    /// Get a single genre by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
    public async Task<ActionResult<Genre>> GetGenre(int id)
    {
        var genre = await _ctx.Genres.FindAsync(id);

        if (genre == null) return NotFound();

        return genre;
    }

    /// <summary>
    /// Create a new genre
    /// </summary>
    /// <param name="genreDTO"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Genre), StatusCodes.Status201Created)]
    public async Task<ActionResult<Genre>> AddGenre([FromBody] GenreDTO genreDTO)
    {
        var genre = new Genre { Name = genreDTO.Name };

        _ctx.Genres.Add(genre);

        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    /// <summary>
    /// Update a genre by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="genreDTO"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateGenre(int id, [FromBody] GenreDTO genreDTO)
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
    /// <returns></returns>
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