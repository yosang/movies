using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using movies.Models;
using movies.Context;
using Microsoft.AspNetCore.Authorization;
using movies.DTOs;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class StudioController : ControllerBase
{

    private readonly MoviesContext _ctx;

    public StudioController(MoviesContext context)
    {
        _ctx = context;
    }

    /// <summary>
    /// Retrieve a list of studios
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetStudioDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetStudioDTO>>> GetStudios(int page = 1, int pageSize = 5)
    {
        if (_ctx.Studios == null) return NotFound();

        // Here we are using the GetStudioDTO to map out the properties we want from the entity
        return await _ctx.Studios
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(s => new GetStudioDTO { Id = s.Id, Name = s.Name })
                            .ToListAsync();
    }

    /// <summary>
    /// Retrieve a studio by its id
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetStudioDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetStudioDTO>> GetStudio(int id)
    {
        if (_ctx.Studios == null) return NotFound();

        var studio = await _ctx.Studios.Where(e => e.Id == id)
                                        .Select(e => new GetStudioDTO { Id = e.Id, Name = e.Name })
                                        .FirstOrDefaultAsync();

        if (studio == null) return NotFound();

        return studio;
    }

    /// <summary>
    /// Create a new studio
    /// </summary>
    /// <param name="studioDTO"></param>
    /// <response code="201">Returns the newly created studio</response>
    /// <response code="400">Missing required studio properties</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GetStudioDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetStudioDTO>> CreateStudio(CreateStudioDTO studioDTO)
    {
        var studio = new Studio { Name = studioDTO.Name };

        _ctx.Studios.Add(studio);

        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudio), new { id = studio.Id }, new GetStudioDTO { Id = studio.Id, Name = studio.Name });
    }

    /// <summary>
    /// Update an existing studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="studioDTO"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateStudio(int id, UpdateStudioDTO studioDTO)
    {
        var studio = await _ctx.Studios.FindAsync(id);
        if (studio == null) return NotFound();

        studio.Name = studioDTO.Name;

        await _ctx.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete an existing studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteStudio(int id)
    {
        var studio = await _ctx.Studios.FindAsync(id);
        if (studio == null) return NotFound();

        _ctx.Remove(studio);

        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}