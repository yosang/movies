# Project
In this little project we are implementing a ASP.NET Core Web Api with Controllers and connecting it to a minimalistic MySQL database to perform simple CRUD operations.

[Resource 1](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0)
[Resource 2](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-10.0)

# Template
The template use for this project is `dotnet new web`, which is a total empty template, we are building this from scratch.

# Concepts
- Designing API's with controllers
- appsettings.json (for connection string configuration)
- Dependency Injection (injecting DbContext to the api)
- Migrations

## Designing a controller

### ControllerBase class
To get started with controllers we simply need to import the namespace `Microsoft.AspNetCore.Mvc`, in here we get access to `ControllersBase`, which is the base class we use to define a class as a Controller class.

### ApiController attribute
Once we got a class that inherits from `ControllerBase`, we can simply add the attribute `[ApiController]` to it, this class is now a controller.

### Route attribute
The simplest approach is to add a `[Route("[controller]")]` attribute to the class, this makes whatever the file name is (without the Controller) prefix the name of the route.

For example in my case I got `MoviesController.cs` as the file name, the route endpoint will then be `/movies` thanks to this attribute.

If we wwant to specify the route, we can also do so, for example `[Route("api/movies")]`, or `[Route("api/[controller]")]`.

### HTTP Methods
To enable a HTTP verb to a method we simply use the attributes `[HttpGet]`, `[HttpPost]` etc.

### Return types

#### Primitive data type
We can return whatever we want from an endpoint, `AspNetCore` will ensure its properly serialized to JSON. For example here im just returning a string to test the controller.

However, when returning a collection based type, it is recommended to use `IEnumerable<T>` for better performance. 

```c#
using Microsoft.AspNetCore.Mvc;

[ApiController] // Marks this class as a controller
[Route("[controller]")] // The name of this route is automatically MoviesController without Controller prefix
public class MoviesController : ControllerBase
{

    public string[] someData = new[] { "Yosmel", "test" };

    [HttpGet]
    public string[] GetMovies()
    {
        return someData.ToArray(); ;
    }
}
```

#### ActionResult<T>
The `ActionResult` class is a default implementation of the `IActionResult` interface. It is useful when we want to return meaningful HTTP responses (status codes) based on conditional checks. We might not care about it if we always return the same type of data and we dont need to handle errors with specific http status codes.

In the example above, Im always returning an array.

Now lets imagine we want to check if the array has items before returning data:

```c#
    [HttpGet]
    public ActionResult<string[]> GetMovies()
    {
        if (someData.Length < 1) return NotFound();

        return someData;
    }
```

Here we see that we can retuurn `NotFound()` which returns `404` or just the `data`, which under the hood is automatically wrapped under `Ok()` and returns `200`.

#### IActionResult
Its a flexible interface contract, that allows us to return multiple types of data, however we have to manually wrap the response object.

```c#
    [HttpGet]
    public IActionResult GetMovies()
    {
        if (someData.Length < 1) return NotFound();

        return Ok(someData);
    }
```

#### Async/Await
When working with async work, like databases, we want to to make this method `async` with `Task`.
<>

## Adding Controller service
Once a controller has been setting up and we want to add it to our application, we simply put the following in `Program.cs`.

Before `app.build()`
```c#
builder.Services.AddControllers()
```

After `app.build`
```c#
app.MapControllerS();
```

# appsettings.json
- Dependency Injection (injecting DbContext to the api)
- Migrations

# Technologies
- .NET 9
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFramewworkCore.Design
- Swashbucke.AspNetCore

# Usage

# Author
[Yosmel Chiang](https://github.com/yosang)