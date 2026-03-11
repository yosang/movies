- [Project](#project)
  - [Resources](#resources)
- [Template](#template)
- [Concepts](#concepts)
  - [Designing a controller](#designing-a-controller)
    - [ControllerBase class](#controllerbase-class)
    - [ApiController attribute](#apicontroller-attribute)
    - [Route attribute](#route-attribute)
    - [HTTP Methods](#http-methods)
    - [Return types](#return-types)
      - [Primitive data type](#primitive-data-type)
      - [ActionResult<T>](#actionresult)
      - [IActionResult](#iactionresult)
      - [Async/Await](#asyncawait)
  - [Adding Controller service](#adding-controller-service)
- [appsettings.json](#appsettingsjson)
- [Dependency Inject](#dependency-injection)
    - [Dependency Injection (injecting DbContext to the api)](#dependency-injection-injecting-dbcontext-to-the-api)
    - [Constructor](#constructor)
    - [AddDbContext](#adddbcontext)
    - [Dependency Injection (injecting DbContext to the controllers)](#dependency-injection-injecting-dbcontext-to-the-controllers)
        - [Manual dependency injection](#manual-dependency-injection)
- [Handling Migrations](#handling-migrations)
- [Swagger Documentation](#swagger-documentation)
  - [OpenAPI documentation](#openapi-documentation)
  - [XML Documentation](#xml-documentation)
  - [Adding comments](#adding-comments)
  - [Return types](#return-types-1)
  - [Remarks](#remarks)
  - [Response codes](#response-codes)
- [Authentication](#authentication)
  - [JWT Configuration](#jwt-configuration)
  - [JWT Controller](#jwt-controller)
  - [User authentication](#user-authentication)
    - [DTOs](#dtos)
- [CORS](#cors)
  - [Multiple policies](#multiple-policies)
- [Technologies](#technologies)
- [Usage](#usage)
- [Author](#author)

# Project
In this little project we are implementing a ASP.NET Core Web Api with Controllers and connecting it to a minimalistic MySQL database to perform simple CRUD operations.

## Resources
- [Basics](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0)
- [Return types](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-10.0)
- [AddDbContext](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontextoptions)
- [Dependency Inject](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontext-in-dependency-injection-for-aspnet-core)
- [Dependency Injection Controllers](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-9.0https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection?view=aspnetcore-9.0)

# Template
The template used for this project is `dotnet new web`, which is a total empty template, we are building this from scratch.

# Concepts
- Designing API's with controllers
- appsettings.json (for connection string configuration)
- Dependency Injection (injecting DbContext to the api)
- Dependency Injection (injecting the controller with DbContext)
- Migrations
- Swagger documentation

## Designing a controller

### ControllerBase class
To get started with controllers we simply need to import the namespace `Microsoft.AspNetCore.Mvc`, in here we get access to `ControllerBase`, which is the base class we use to define a class as a Controller class.

### ApiController attribute
Once we got a class that inherits from `ControllerBase`, we can simply add the attribute `[ApiController]` to it, this class is now a controller.

### Route attribute
The simplest approach is to add a `[Route("[controller]")]` attribute to the class, this makes whatever the file name is (without the Controller) prefix the name of the route.

For example in my case I got `MoviesController.cs` as the file name, the route endpoint will then be `/movies` thanks to this attribute.

If we want to specify the route, we can also do so, for example `[Route("api/movies")]`, or `[Route("api/[controller]")]`.

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
When working with async work, like databases, we want to make this method `async` with `Task`.

## Adding Controller service
Once a controller has been setting up and we want to add it to our application, we simply put the following in `Program.cs`.

Before `app.build()`
```c#
builder.Services.AddControllers()
```

After `app.build`
```c#
app.MapControllers();
```

# appsettings.json
`appsettings.json` is a configuration file that stores key-value pairs. It allows us to avoid hardcoding, and we can exclude this file entirely from our version control system through `.gitignore`.
# Dependency Injection

## Dependency Injection (injecting DbContext to the api)
Instead of hardcoding the connection string in the `DbContext` class by overriding the `OnConfiguring` method like I have done earlier, we are going to implement `Dependency Injection` to provide the database context to our API and controllers.

### Constructor
Since we are no longer hardcoding the connection string, we need to pass the configuration to our `DbContext` class somehow.

We do this by defining a constructor that takes `DbContextOptions<T>` parameter, this parameter carries all configurations set through `Dependency Injection` with the `AddDbContext` method. The documentation for this parameter is found [here](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontextoptions).

```c#
public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public MoviesContext(DbContextOptions<MoviesContext> options) : base(options) { }
}
```

This allows **ASP.NET Core DI** to fully configure and provide the context.

### AddDbContext
Instead of adding configurations to instances of this class by overriding `OnConfiguring`, we add configurations to `DbContextOptions` through `AddDbContext` as its specifically designed for `Dependency Injection`.

This:
- Builds the `DbContextOptions` with the connection string.
- Registers `MoviesContext` in DI as Scoped by default (one instance per HTTP request)

```c#
var conString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not defined");

builder.Services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString));
```

Now, any controller that asks for `MoviesContext` in its constructor will get a new instance per request, which is safe and recommended.

The documentation for Dependency Inject is AspNetCore can be found [here](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#dbcontext-in-dependency-injection-for-aspnet-core).

## Dependency Injection (injecting DbContext to the controllers)
Once we have injected the `DbContext`, we simply need to make a constructor for each controllers that allow access to the `Context`.

```c#
private readonly MoviesContext _ctx; // Repository for the DbContext

public MoviesController(MoviesContext context) => _ctx = context; // context is requested here, the DI container delivers
```

Here we are simply populating our property with the context retrieved from the DI container.

What happens under the hood:
- ASP.NET Core sees that `MoviesController` needs `MoviesContext`.
- The DI container looks up `MoviesContext` registration.
- It creates a `scoped instance` for this request and injects it.
- Once the HTTP request ends, the context is disposed automatically.

### Manual dependency injection
We can also configure this manually:

```c#
var optionsBuilder = new DbContextOptionsBuilder<MoviesContext>();
optionsBuilder.UseMySQL(conString);
var context = new MoviesContext(optionsBuilder.Options);

builder.Services.AddControllers().AddControllersAsServices(); // Allows controllers as services to be resolved from DI
builder.Services.AddSingleton(context);
```

- Here we manually built the options and created the context.
- `AddControllersAsServices` - Allows registration and injection of services to controllers using a constructor.
- `AddSingleton` - Adds the context of the database class to the DI container and registers one instance shared across all requests, which can cause concurrency issues.
- `Controllers` get `MoviesContext` via constructor injection automatically once registered.

Instead of doing it this way, we can use `AddDbContext` instead of the code above.

```c#
builder.Services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString));
```

`AddDbContext` does two things:
- Builds `DbContextOptions` with the connection string
- Adds `MoviesContext` to the DI container as **Scoped by default** (one instance per HTTP request)

# Handling Migrations
- `dotnet ef migrations add InitialSeed` - Creates a new migration
- `dotnet ef migrations remove` - Removes a previous migration (Migrations are applied in order and can only be removed from the top (last-in-first-out).)
- `dotnet ef migrations list` - List all current migrations
- `dotnet database update` - Uploads the migration changes to the database
- `dotnet ef database update 0` - Rolls back the migration to start, we can also specify a migrationname to rollback to instead.
- `dotnet database drop` - Deletes the entire database and all its tables ( full reset )

# Swagger Documentation
As we are using Swagger to document our API, there with the `Swashbuckle.AspNetCore` package.
- `Swagger` - Gives us a set of tools to configure and define the API documentation.
    - To use it we can add `builder.Services.AddSwaggerGen()` and`app.UseSwagger()` to `Program.cs`.
        - `SwaggerGen()` will automatically bring in the controller endpoints to swagger.
- `Swagger UI` - The tools that allows us to visualize our API, it provides us with an endpoint `/swagger`.
    - To use it we can add `app.UseSwaggerUI()` to `Program.cs`.

## OpenAPI documentation
OpenAPI is already built in `AspNetCore`, so we dont need to add anything extra.


However to further configure our Swagger documentation we can use the `OpenApiInfo` class to give our `SwaggerUI` some context.

```c#
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movie API",
        Description = "An ASP.NET Core API to manage CRUD operations on Movies"
    });

});
```

## XML Documentation
Swagger also allows us to provide XML comments from comments directly on the endpoints, this is a handy little feature that allows us to add some context to the endpoints.

To enable XML documentation we first must add this to the `.csproj` file under `<PropertyGroup>`.
```c#
    <GenerateDocumentationFile>true</GenerateDocumentationFile> // Generates <project>.xml, which is usually located in obj/Debug/net9.0/movies.xml
    <NoWarn>$(NoWarn);CS1591</NoWarn> // Shuts up the warning about missing XML comments 
```

This file is the one responsible for generating documentation out of comments like this
```c#
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
```
To allow our application to bring in this `.xml` file to `Swagger` we must point to the path of this file then add that path to `.IncludeXmlComments()` method.

```c#
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movie API",
        Description = "An ASP.NET Core API to manage CRUD operations on Movies"
    });

    // Gets the name of the assembly and adds .xml to it    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 

    // Resolves the full path, which is from root of the application and adds the <name>.xml file at the end
    // My current base directory path is /home/yosang/Downloads/movies/bin/Debug/net9.0/
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    // Adds the full path to the method
    // /home/yosang/Downloads/movies/bin/Debug/net9.0/movies.xml
    options.IncludeXmlComments(xmlPath);
});
```

## Adding comments
To add a comment we simply initiate with `///` and fill out.

```c#
    /// <summary>
    /// Gets a list of movies
    /// </summary>
    /// <returns>List of movies</returns>
```
Now we can see that SwaggerUI updates perfectly.

![alt text](image.png)

Each time the application builds, a new `.xml` is generated and brought into `Swagger`.

## Return types
To allow Swagger to show proper return types we can configure the `[ProducesResponseType]` attribute for each endpoint.

This allows Swagger to pull the response types and show them on the `SwaggerUI`.

We can pass the type of the entity we are returning, and the status code.

```c#
[ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
```

## Remarks
Adds some nice little examples to the Swagger documentation

```c#
    /// <remarks>
    /// Sample:
    /// {
    ///     "id": 1,
    ///     "Name": Titanic
    /// }
    /// </remarks>
```

## Response codes
Adds some context to the different response codes

```c#
    /// <response code="200">Returns a single movie</response>
    /// <response code="404">If no movie with the specified id was found</response>
```

# Authentication

## JWT Configuration
We are going to add our configuration settings for JWT in [appsettings.json](#appsettingsjson).

```json
  "JwtSettings":{
    "SecretKey": "ALongSecretKeyOfAtLeast40Charactersasasdasdasdasdasdasd1231231231sx12s121",
    "Issuer": "MyIssuer",
    "Audience": "MyAudience",
    "ExpiryMinutes": 60
  }
```

So in order to work with these settings, we are going to bind them to a class instance.

This class will match exactly the appsettings key:value pairs.
```c#
namespace movies.Auth;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }

}
```

And to bind it we can do it this way
- `GetSection` - Gets the values from the specified section.
- `Get<T>` - Attempts to bind the returned configuration values from `GetSection` to an instance of the specified type.

```c#
    // Binds the configuration settings from appsettings to the class
    var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
    services.AddSingleton(jwtSettings!);
```

Further, adding JWT configuration is going to bloat our `Program.cs`, so we are going to introduce a service extension pattern.

Simply we create a new class:

```c#
namespace movies.Auth;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Binds the configuration settings from appsettings to the class
        var jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddSingleton(jwtSettings!);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

        return services;
    }
}
```

Because the method uses `this IServiceCollection services`, it becomes an extension method for `IServiceCollection`, technically it just extends the `type`. This allows us to call `builder.Services.AddJwtAuthentication(...)` while keeping the configuration logic in a separate class.

We are also defining the paramter config of type `IConfiguration` so that we can pass `builder.Configuration` into this method.

After logic is implemented we are returning `services`, so that we can allow for more chaining, ultimately our `Program.cs` code will end up like this

```c#
// Services
builder.Services.AddDatabase(builder.Configuration) // Database context DI
                .AddJwtAuthentication(builder.Configuration) // JWT DI
                .AddAuthorization() // Adds authorization
                .AddSwaggerDoc() // Swagger documentation generator
                .AddControllers(); // Finally scans and adds the controller services
```

To read more about this pattern, checkout this [resource](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0#register-groups-of-services-with-extension-methods).

This pattern is actually used everywhere in ASP.NET Core application:

```c#
public static class MvcServiceCollectionExtensions
{
    public static IMvcBuilder AddControllers(this IServiceCollection services)
    {
        // registers MVC services
    }
}
```

## JWT Controller
We need a controller in order to retrieve a token, so we create a simple one that uses the `Singleton` from the DI container. The path to this controller will be `/Auth` and a single endpoint `POST /token`.
```c#
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtSettings _jwt;

    // DI constructor
    public AuthController(JwtSettings jwtSettings)
    {
        _jwt = jwtSettings;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken()
    {
        // Array containing claims
        // Claims a simply pieces of information about the user or entity being authenticated
        // Here we are just creating a subject claim with "testuser" and a random unique ID for the JTI (unique identifier to prevent token reuse)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)); // Instance of secretKey wrapped in a SysmetricSecurityKey (can be used for both signing and verification)
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Instance of credentials using key and an algorithm

        // The token creation
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer, // Who is issuing the token (defined in appsettings.json)
            audience: _jwt.Audience, // Who is the token intended for (defined in appsettings.json)
            claims: claims, // specific user information (claims)
            expires: DateTime.Now.AddMinutes(_jwt.ExpiryMinutes), // Token expiration
            signingCredentials: creds // The signature for the token
        );

        return Ok(new JwtSecurityTokenHandler().WriteToken(token)); // Token serialization (a JWT string that the client receives)
    }
}
```

## User authentication
Instead of retrieving a token by sending a POST request sent to `/auth/token`. Ultimately, we want to verify user credentials against the database and return a token only if authentication succeeds.
    - So we are going to create a `User` model for this in the database with hashed passwords.

Currently, our controller endpoints query the database directly. Best practice is to have controllers communicate with services, which then handle database operations. This separation of concerns keeps controllers focused on HTTP logic while services manage business logic and data manipulation.

For simplicity in this learning project, I won’t fully refactor the architecture. Instead, I will create services for login and registration while keeping controllers mostly as-is.

For our `AuthService`, we are going to use the `Microsoft.AspNetCore.Identity` namespace, which provides some useful management of users, passwords, tokens and more [Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity?view=aspnetcore-10.0). This is the namespace that allows us to access password hashing and validation.

For our `AuthController` we are going to define a `LoginRequestDTO` and `RegisterRequestDTO`

### DTOs
Data Transfer Objects are classes with the only purpose of carrying data between different layers of the application.
- They usually just contain properties and no methods.
- They are usually used for input or output, like for example the data sent in a body from a POST request can be mapped in a DTO, or the data sent back from the API in a response can be mapped in a DTO.

For our `LoginRequestDTO`, we expect the request body to look like this

```c#
using System.ComponentModel.DataAnnotations;

public class LoginRequestDTO
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
```

And the API will respond with the following DTO

```c#
public class LoginResponseDTO
{
    public string Token { get; set; } = string.Empty;
}
```

Another benefit of using DTO's, is that ASP.NET Core will allow us to validate the request body through DTO attributes (`[Required]`, `[StringLength]`) when using `[ApiController]`. In the `LoginRequestDTO`, we can see that any request body that is missing either or the properties or violates the string length will result in a bad request.

Here is an example from the `RegisterRequestDTO`

```c#
using System.ComponentModel.DataAnnotations;

public class RegisterRequestDTO
{
    [Required]
    [StringLength(32, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
```

This protects the API from bad input even before reaching business logic.

In short: **DTO = a clean, simple object for sending or receiving data and validating input**.

# CORS
Cross-Origin Resource Sharing is a security mechanism provided by the World Wide Web Consortium (W3C) standard.

It defines the level of security when accessing resources from different domains. For example, a client running on port `5003` attempts to access a resource on port `5004`, which is a different domain, will get rejected by `CORS`.

CORS is not configured by default, so we must add a configuration for it.

Once again, Im going to create another extension method to include the configuration logic and feed it to `IServiceCollection`.

```c#
namespace movies.Cors;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Default", builder =>
            {
                builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
            });
        });

        return services;
    }
}
```

Then add it to `Program.cs`, the order matters through, Cors policies must come before `MapControllers` or any endpoint that requires `CORS`.

```c#
var app = builder.Build();

app.MapGet("/", () => "Hello world");

app.UseCors("Default");

app.UseJwt() // Enables JWT middlewares
    .UseSwaggerDoc() // Enables Swagger middlewares
    .MapControllers(); // Maps the controller service endpoints

app.Run();
```

## Multiple policies
If we want to switch between `Default` and other policies, we can use the attribute `[EnableCors("PolicyName")]` on specific endpoints.

# Technologies
- .NET 9
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Swashbuckle.AspNetCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

# Usage
- Clone the repo.
- Run it with `dotnet run`.
- Visit `/swagger` for the available endpoints. 

# Author
[Yosmel Chiang](https://github.com/yosang)