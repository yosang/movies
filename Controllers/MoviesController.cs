using Microsoft.AspNetCore.Mvc;

[ApiController] // Marks this class as a controller
[Route("[controller]")] // The name of this route is automatically MoviesController without Controller prefix
public class MoviesController : ControllerBase
{

    // public List<string> someData = new() { "Yosmel", "test" };

    // [HttpGet]
    // public ActionResult<IEnumerable<string>> GetMovies()
    // {
    //     if (someData.Count < 1) return NotFound();

    //     return someData;
    // }
}