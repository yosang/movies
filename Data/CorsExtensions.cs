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