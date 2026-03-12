using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movies.Context;
using movies.DTOs;

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

        return await _ctx.Actors.Skip((page - 1) * pageSize)
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

    // [HttpPost]
    // public async Task<ActionResult<IEnumerable<GetActorDTO>>> GetActor(GetActorDTO getActorDTO)
    // {

    // }
    // [HttpPut("{id}")]
    // public async Task<ActionResult<IEnumerable<GetActorDTO>>> GetActor(GetActorDTO getActorDTO)
    // {

    // }
    // [HttpDelete("{id}")]
    // public async Task<ActionResult<IEnumerable<GetActorDTO>>> GetActor(GetActorDTO getActorDTO)
    // {

    // }
}