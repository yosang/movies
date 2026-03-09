using Microsoft.EntityFrameworkCore;
using movies.Context;

var builder = WebApplication.CreateBuilder();

// Gets the connection string or throws an exception
var conString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not defined");

// Injects the connection string to the DbContext class here
builder.Services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString));

builder.Services.AddControllers(); // Imports the controllers
// builder.Services.AddSwaggerGen(); // Auto generates routes from controllers

var app = builder.Build();

app.MapGet("/", () => "Hello world");

app.MapControllers();

app.Run();