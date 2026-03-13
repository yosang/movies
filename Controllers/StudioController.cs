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
    /// <response code="200">Retrieved all</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetStudioDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<GetStudioDTO>>> GetStudio(int page = 1, int pageSize = 2)
    {
        if (_ctx.Studios == null) return NotFound();

        // Here we are using the GetStudioDTO to map out the properties we want from the entity
        return await _ctx.Studios
                            .OrderBy(e => e.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(s => new GetStudioDTO { Id = s.Id, Name = s.Name })
                            .ToListAsync();
    }

    /// <summary>
    /// Retrieve a studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Retrieved by id</response>
    /// <response code="404">Unable to find by id</response>
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
    /// <response code="201">Creation successful</response>
    /// <response code="400">Missing required properties</response>
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
    /// <response code="204">Update successful</response>
    /// <response code="404">Unable to find by id</response>
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
    /// <response code="204">Delete successful</response>
    /// <response code="404">Unable to find by id</response>
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