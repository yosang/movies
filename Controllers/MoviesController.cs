using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.Models;

[ApiController] // Marks this class as a controller
[Route("[controller]")] // The name of this route is automatically MoviesController without Controller prefix
public class MoviesController : ControllerBase
{
    private readonly MoviesContext _ctx;

    public MoviesController(MoviesContext context)
    {
        _ctx = context;
    }

    [HttpGet] // GET - / Get all movies
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
        _ctx.Add(movie);
        await _ctx.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
    }

    [HttpPut("{id}")] // PUT - Update a movie, by including the body
    public void UpdateMovie(int id, Movie updates) { }

    [HttpDelete("{id}")]
    public void UpdateMovie(int id) { }
}