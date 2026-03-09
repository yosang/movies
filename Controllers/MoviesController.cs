using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.Models;

[ApiController] // Marks this class as a controller
[Route("[controller]")] // The name of this route is automatically MoviesController without Controller prefix
public class MoviesController : ControllerBase
{
    private readonly MoviesContext _ctx; // Repository for the DbContext

    // Constructor: Sets the repository through dependency injection
    public MoviesController(MoviesContext context) => _ctx = context;

    /// <summary>
    /// Gets a list of movies
    /// </summary>
    /// <returns>List of movies</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        var movies = await _ctx.Movies.ToListAsync();
        if (movies == null) return NotFound();

        return movies;
    }

    [HttpGet("{id}")] // GET /id - Get a single movie
    public async Task<ActionResult<Movie>> GetMovie(int id)
    {
        var movie = await _ctx.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        return movie;
    }

    [HttpPost] // POST / - Creates a new movie
    public async Task<ActionResult<Movie>> AddMovie(Movie movie)
    {
        _ctx.Movies.Add(movie);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")] // PUT - Update a movie, by including the body
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

    [HttpDelete("{id}")]
    public async Task<ActionResult<Movie>> DeleteMovie(int id)
    {
        var movie = await _ctx.Movies.FindAsync(id);
        if (movie == null) return NotFound();

        _ctx.Movies.Remove(movie);

        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}