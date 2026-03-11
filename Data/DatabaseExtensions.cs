using movies.Context;
using Microsoft.EntityFrameworkCore;

namespace movies.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var conString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not defined");

        // Scoped by default
        // Builds DbContextOptions
        services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString)); // Scoped Lifestime

        return services;
    }
}