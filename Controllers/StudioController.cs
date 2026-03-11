using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using movies.Models;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class StudioController : ControllerBase
{
    /// <summary>
    /// Retrieve a list of studios
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Studio>>> GetStudios(int page = 1, int pageSize = 5)
    {
        // Add logic

        return new List<Studio>(); // Placelder - Replace with List of studios
    }

    /// <summary>
    /// Retrieve a studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Studio>> GetStudio(int id)
    {
        // Add logic

        return new Studio(); // Placeholder - Replace with studio entity
    }

    /// <summary>
    /// Create a new studio
    /// </summary>
    /// <param name="studioDTO"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult> CreateStudio(StudioDTO studioDTO)
    {
        // Add logic

        return Ok(); // Placeholder - replace with CreatedAtAction
    }

    /// <summary>
    /// Update an existing studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="studioDTO"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateStudio(int id, StudioDTO studioDTO)
    {
        // Add logic

        return NoContent();
    }

    /// <summary>
    /// Delete an existing studio by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStudio(int id)
    {
        // Add logic

        return NoContent();
    }
}