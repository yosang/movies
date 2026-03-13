using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.DTOs;
using movies.Models;


[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private readonly MoviesContext _ctx;

    public MovieController(MoviesContext context)
    {
        _ctx = context;
    }

    /// <summary>
    /// Retrieves a list of movies
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retrieved</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetMovieDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetMovieDTO>>> GetMovie(int page = 1, int pageSize = 2)
    {
        if (_ctx.Movies == null) return NotFound();

        return await _ctx.Movies
                                .OrderBy(m => m.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Select(m => new GetMovieDTO
                                {
                                    Id = m.Id,
                                    Name = m.Name,
                                    Genre = m.Genre.Name,
                                    Studio = m.Studio.Name,
                                    Actors = m.MovieActors.
                                                    Select(a => new GetActorDTO
                                                    {
                                                        Id = a.Actor.Id,
                                                        Name = a.Actor.Name
                                                    })
                                                    .ToList()
                                })
                                .ToListAsync();
    }

    /// <summary>
    /// Retrieves a movie by its id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Retrieved</response>
    /// <response code="404">Unable to find by id</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetMovieDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetMovieDTO>> GetMovie(int id)
    {
        if (_ctx.Movies == null) return NotFound();

        var movie = await _ctx.Movies
                                    .Where(e => e.Id == id)
                                    .Select(m => new GetMovieDTO
                                    {
                                        Id = m.Id,
                                        Name = m.Name,
                                        Genre = m.Genre.Name,
                                        Studio = m.Studio.Name,
                                        Actors = m.MovieActors.
                                                    Select(a => new GetActorDTO
                                                    {
                                                        Id = a.Actor.Id,
                                                        Name = a.Actor.Name
                                                    })
                                                    .ToList()
                                    })
                                    .FirstOrDefaultAsync();

        if (movie == null) return NotFound();

        return movie;
    }

    /// <summary>
    /// Create a new movie
    /// </summary>
    /// <param name="dto"></param>
    /// <response code="201">Creation successful</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GetMovieDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetMovieDTO>> CreateMovie(CreateMovieDTO dto)
    {
        var movie = new Movie
        {
            Name = dto.Name,
            GenreId = dto.GenreId,
            StudioId = dto.StudioId,
            MovieActors = dto.ActorIds.Select(e => new MovieActor { ActorId = e }).ToList() // EFCore automatically fills out with missing MovieId's
        };

        _ctx.Add(movie);
        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, new GetMovieDTO { Id = movie.Id, Name = movie.Name });
    }

    /// <summary>
    /// Update a movie by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <response code="200">Update successful</response>
    /// <response code="404">Unable to find by id</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMovie(int id, UpdateMovieDTO dto)
    {
        var movie = await _ctx.Movies.Include(e => e.MovieActors).FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null) return NotFound();

        // Always update name, the others are optional
        movie.Name = dto.Name;

        if (dto.GenreId != 0) movie.GenreId = dto.GenreId;
        if (dto.StudioId != 0) movie.StudioId = dto.StudioId;

        if (dto.Actors.Any() && dto.Actors.FirstOrDefault() != 0)
        {
            movie.MovieActors.Clear();
            movie.MovieActors.AddRange(
                dto.Actors.Distinct().Select(id => new MovieActor { ActorId = id })
            );
        }

        await _ctx.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await _ctx.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        _ctx.Remove(movie);
        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}