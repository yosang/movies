using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace movies.Auth;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Binds the configuration settings from appsettings to the class
        var jwtSettings = config.GetRequiredSection("JwtSettings").Get<JwtSettings>()!;

        services.AddSingleton(jwtSettings)
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = jwtSettings.TokenValidationParameters);

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseJwt(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}