using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using movies.Context;

var builder = WebApplication.CreateBuilder();

// Gets the connection string or throws an exception
var conString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not defined");

// Dependency Injection of Database Context
builder.Services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString));

builder.Services.AddControllers(); // Imports the controllers

// Adds SwaggerGen with OpenApi to define documentation info
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Movie API",
        Description = "An ASP.NET Core API to manage CRUD operations on Movies"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

Console.WriteLine(AppContext.BaseDirectory);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello world");
app.MapControllers();

app.Run();