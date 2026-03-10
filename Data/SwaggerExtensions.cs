using System.Reflection;
using Microsoft.OpenApi.Models;

namespace movies.Swagger;

public static class SwaggerExtensions
{

    public static IServiceCollection AddSwaggerDoc(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Uses OpenAPI metadata to generate Swagger documentation
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "V1",
                Title = "Movie API",
                Description = "An ASP.NET Core API to manage CRUD operations on Movies"
            });

            /*  Allows Swagger to pickup Xml comments on controllers
                Assuming the following is enabled in .csproj under <PropertyGroup>
                <GenerateDocumentationFile>true</GenerateDocumentationFile>
            */
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));

            // Enables Bearer authentication through OpenApi
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            // Uses the OpenApiSecurityScheme created above for endpoints, the Id is "Bearer"
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
        });

        return services;
    }

    // Extension method that enables Swagger middlewares
    public static WebApplication UseSwaggerDoc(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}