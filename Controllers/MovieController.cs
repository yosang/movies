using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.Models;

[ApiController] // Marks this class as a controller
[Route("[controller]")] // The name of this route is automatically MoviesController without Controller prefix
[Produces("application/json")]
public class MovieController : ControllerBase
{
    private readonly MoviesContext _ctx; // Repository for the DbContext

    // Constructor: Sets the repository through dependency injection
    public MovieController(MoviesContext context) => _ctx = context;

    /// <summary>
    /// Gets a list of movies
    /// </summary>
    /// <returns>List of movies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        var movies = await _ctx.Movies.ToListAsync();
        if (movies == null) return NotFound();

        return movies;
    }

    /// <summary>
    /// Gets a single movie
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Movie</returns>
    /// <remarks>
    /// Sample:
    /// {
    ///     "id": 1,
    ///     "Name": "Titanic"
    /// }
    /// </remarks>
    /// <response code="200">Returns a single movie</response>
    /// <response code="404">If no movie with the specified id was found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
        var movie = await _ctx.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        return movie;
    }

    /// <summary>
    /// Creates a new movie from a JSON body
    /// </summary>
    /// <param name="movie"></param>
    /// <returns>The new movie created</returns>
    [HttpPost] // POST / - Creates a new movie
    [ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
    [Authorize] // Requires authorization through jwt
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Movie>> AddMovie(Movie movie)
    {
        _ctx.Movies.Add(movie);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    /// <summary>
    /// Updates a movie by its ID and JSON body with the properties to update.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updates"></param>
    /// <returns>No content</returns>
    [HttpPut("{id}")] // PUT - Update a movie, by including the body
    [Authorize]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movie>> UpdateMovie(int id, Movie updates)
    {
        if (id != updates.Id) return BadRequest();

        var movie = await _ctx.Movies.FindAsync(id);

        if (movie == null) return NotFound();

        // Entry gives direct access to an already tracked entity
        // Which allows us to modify its current values and replace only those that differ with SetValues (copies values from an object)
        _ctx.Entry(movie).CurrentValues.SetValues(updates);

        // We dont need to check if the state is modified to save changes, it happens automtically if there are changes, however its nice to debug
        if (_ctx.Entry(movie).State == EntityState.Modified) Console.WriteLine("Movie was updated!"); // For debugging

        await _ctx.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a movie by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Movie>> DeleteMovie(int id)
    {
        var movie = await _ctx.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        _ctx.Movies.Remove(movie);

        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}