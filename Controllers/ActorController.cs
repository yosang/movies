using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.DTOs;
using movies.Models;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ActorController : ControllerBase
{
    private readonly MoviesContext _ctx;

    public ActorController(MoviesContext context)
    {
        _ctx = context;
    }

    /// <summary>
    /// Retrieve a list of actors
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <response code="200">Retrieved</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetActorDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetActorDTO>>> GetActor(int page = 1, int pageSize = 5)
    {
        if (_ctx == null) return NotFound();

        return await _ctx.Actors
                            .OrderBy(e => e.Id)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(e => new GetActorDTO { Id = e.Id, Name = e.Name })
                            .ToListAsync();
    }

    /// <summary>
    /// Retrieve an actor by id
    /// </summary>
    /// <param name="id"></param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetActorDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetActorDTO>> GetActor(int id)
    {
        if (_ctx == null) return NotFound();

        var actor = await _ctx.Actors.Where(e => e.Id == id)
                                        .Select(e => new GetActorDTO { Id = e.Id, Name = e.Name })
                                        .FirstOrDefaultAsync();

        if (actor == null) return NotFound();

        return actor;
    }

    /// <summary>
    /// Create an actor
    /// </summary>
    /// <param name="actorDTO"></param>
    /// <response code="201">Creation successful</response>
    /// <response code="400">Missing required properties</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(GetActorDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GetActorDTO>>> CreateActor(CreateActorDTO actorDTO)
    {
        var actor = new Actor { Name = actorDTO.Name };

        _ctx.Actors.Add(actor);

        await _ctx.SaveChangesAsync();

        return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, new GetActorDTO { Id = actor.Id, Name = actor.Name });
    }

    /// <summary>
    /// Update an actor by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="actorDTO"></param>
    /// <response code="204">Update successful</response>
    /// <response code="404">Unable to find by id</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateActor(int id, UpdateActorDTO actorDTO)
    {
        var actor = await _ctx.Actors.FindAsync(id);
        if (actor == null) return NotFound();

        actor.Name = actorDTO.Name;

        await _ctx.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Delete an actor by its id
    /// </summary>
    /// <param name="id"></param>
    /// <response code="204">Deletion successful</response>
    /// <response code="404">Unable to find by id</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteActor(int id)
    {
        var actor = await _ctx.Actors.FindAsync(id);
        if (actor == null) return NotFound();

        _ctx.Remove(actor);

        await _ctx.SaveChangesAsync();

        return NoContent();
    }
}