using movies.Auth;
using movies.Database;
using movies.Swagger;

var builder = WebApplication.CreateBuilder();

// Services
builder.Services.AddDatabase(builder.Configuration) // Database context DI
                .AddJwtAuthentication(builder.Configuration) // JWT DI
                .AddAuthorization() // Adds authorization
                .AddSwaggerDoc() // Swagger documentation generator
                .AddControllers(); // Finally scans and adds the controller services


var app = builder.Build();

app.MapGet("/", () => "Hello world");

app.UseJwt() // Enables JWT middlewares
    .UseSwaggerDoc() // Enables Swagger middlewares
    .MapControllers(); // Maps the controller service endpoints

app.Run();