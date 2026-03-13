using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.DTOs;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ReviewController : ControllerBase
{

    private readonly MoviesContext _ctx;

    public ReviewController(MoviesContext context)
    {
        _ctx = context;
    }

    /// <summary>
    /// Retrieve a list of reviews
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retrieved</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetReviewDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetReviewDTO>>> GetReview(int page = 1, int pageSize = 2)
    {
        if (_ctx.Reviews == null) return NotFound();

        return await _ctx.Reviews
                                .OrderBy(e => e.Id)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Select(r => new GetReviewDTO()
                                {
                                    Id = r.Id,
                                    Comment = r.Comment,
                                    Rating = r.Rating,
                                    Movie = new MovieSimpleDTO { Id = r.Movie!.Id, Name = r.Movie.Name }
                                })
                                .ToListAsync();
    }

    /// <summary>
    /// Retrieve a review by id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Retreived</response>
    /// <response code="404">Notfound by id</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetReviewDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetReviewDTO>> GetReview(int id)
    {
        if (_ctx.Movies == null) return NotFound();

        var review = await _ctx.Reviews
                                    .Where(e => e.Id == id)
                                    .Select(r => new GetReviewDTO()
                                    {
                                        Id = r.Id,
                                        Comment = r.Comment,
                                        Rating = r.Rating,
                                        Movie = new MovieSimpleDTO { Id = r.Movie!.Id, Name = r.Movie.Name }
                                    })
                                    .FirstOrDefaultAsync();

        return review == null ? NotFound() : review;
    }

    /// <summary>
    /// Create a review
    /// </summary>
    /// <param name="dto"></param>
    /// <response code="201">Creation successful</response>
    /// <response code="400">Missing required properties</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateReview(CreateReviewDTO dto)
    {
        var review = new Review
        {
            Comment = dto.Comment,
            Rating = dto.Rating,
            MovieId = dto.MovieId
        };

        _ctx.Reviews.Add(review);

        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
    }

    /// <summary>
    /// Updates a review by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <response code="204">Update successful</response>
    /// <response code="404">Not found by id</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UpdateReviewDTO), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateReview(int id, UpdateReviewDTO dto)
    {
        var review = await _ctx.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        review.Comment = dto.Comment;
        review.Rating = dto.Rating;
        review.MovieId = dto.MovieId;

        await _ctx.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete a review by id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="204">Deletion successful</response>
    /// <response code="404">Not found by id</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReview(int id)
    {
        var review = await _ctx.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        _ctx.Remove(review);

        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}