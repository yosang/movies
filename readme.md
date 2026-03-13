- [Project](#project)
- [Concepts](#concepts)
- [Dependency Injection](#dependency-injection)
- [JWT Settings](#jwt-settings)
- [Technologies](#technologies)
- [Usage](#usage)
- [Author](#author)
# Project
In this little project we are implementing a ASP.NET Core Web Api with Controllers and connecting it to a minimalistic MySQL database to perform simple CRUD operations using JWT authentication.

# Concepts
- [Dependency Injection](#dependency-injection)
  - [The problem](#the-problem)
  - [The solution](#the-solution)
    - [The constructor](#the-constructor)
    - [Injection](#injection)
    - [Program.cs](#programcs)
  - [JWT Settings](#jwt-settings)
    - [Basic properties](#basic-properties)
    - [SymmetricSecurityKey](#symmetricsecuritykey)
    - [TokenValidationParameters](#tokenvalidationparameters)
    - [Extension method](#extension-method)
    - [Program.cs](#programcs-1)
    - [Behind the scenes](#behind-the-scenes)
    - [Authentication](#authentication)
    - [Authorization](#authorization)

# Dependency Injection

## The problem
Looking back at a demo project like [efcore-books](https://github.com/yosang/efcore-books) which uses a code-first approach using Entity Framework Core with a MySQL database. The data access layer (`DbContexct`) is configued using an overriden method of the `OnConfiguring` method, we can see that [here](https://github.com/yosang/efcore-books/blob/main/Models/BooksContext.cs). We can also see that in order to use this context with any services that are dependant on it, we need to instantiate and pass it the context class, we can see that [here](https://github.com/yosang/efcore-books/blob/main/Program.cs).

Here is a little snippet of `Program.cs`
```c#
    public static void Main()
    {
        var service = new BookService(new BookContext());

        var categoryBooksCounts = service.categoryBooksCounts();
        foreach (var c in categoryBooksCounts)
        {
            Console.WriteLine(c);
        }
    }
```

We can clearly see that for future services, we have to do the same thing, leading in a lot of manual instantiation through with the `new()` keyword and introducing more `tight coupling` of our classes.

There is a way we can implement architecture in our application to leverage the built in `Dependency Inject` capabalities of ASP.NET Core which promotes `loose coupling`.

## The solution
Dependency Injection simply means, any class that asks for a certain instance, can receive it, as long as its registered in the application collection of services.

With that being said, we should register our data access layer (`context`) in the application collection of services for any class that needs it.

The first step is to configure our `context` class so it itself can be injected with the necessary configurations. At first glance, the necessary configuration for a database context is the connection string, which I previously hard coded, but now we are going to get it from `appsettings.json` as a security measure.

### The constructor
Instead of building our context class by overriding the `OnConfiguring` method, we are going to design a constructor. Which one of the ways we can inject instances.

```c#
    // Constructor
    public MoviesContext(DbContextOptions<MoviesContext> options) : base(options) { }
```
- Earlier we were configuring `DbContextOptionsBuilder` ourselves through the `OnConfiguring` method, now instead our context class asks for `DbContextOptions` through the constructor which is going to be delivered through `Dependency Injection`
- We are also passing the configuration `options` to the base class, which contains the connection string and other configurations.

Now our `context` class is ready to be injected.

### Injection
We have two options for injecting the connection string to our `context` class:
- Directly in `Program.cs`, or
- Using an extension method

Creating an extension method involves:
- Defining a static class.
- Providing a method that accepts `IServiceCollection`.
- Returns `IServiceCollection` after adding the connection string.

```c#
public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var conString = config.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not defined");

        services.AddDbContext<MoviesContext>(options => options.UseMySQL(conString)); // Scoped Lifestime

        return services;
    }
}
```
- `IServiceCollection` is by itself part of the `Dependency Injection` system in .NET and defines the service registration contract.
- `AddDbContext<TContext>` is by itself a `Dependency Injection` method, it does two things:
    - It takes `DbContextOptionsBuilder`, as seen previously used in the `OnConfiguring` method, which our `context` constructor requires.
    - It adds `MovieContext` as a `scoped` dependency, which means, any class that wants `MovieContext`, gets an instance of it (per request), and that instance is disposed when the request ends. This prevents conflicts between requests when pinging the database for data.

Here is what is happening behind the scenes:
- The `context` class wants `DbContextOptionsBuilder`, which is delivered by `AddDbContext`.
- Any class that wants `MovieContext`, now gets it, since `MovieContext` was registered by `AddDbContext`.

### Program.cs
Finally, since all the `Dependency Injection` logic happens inside our extension method, we can simply apply it to `Program.cs`.

```c#
builder.Services.AddAddDatabase(builder.Configuration)
```

Here we are passing `builder.Configuration`, because thats the type needed for retrieving the connection string.

# Authentication / Authorization
For this application we are implementing authentication through JWT. Like the connection string, the required settings are going to live in `appsettings.json` and so the approach to retrieve and inject is going to be very similar to the one of the `DbContext`.

## JWT Settings
In order to bring in those settings to programmable code, we need a class, we are defining our own settings class named `JwtSettings.cs` which holds the same properties as those of `appsettings.json`. and a few more things.

### Basic properties
The properties brought in can be accessed from any class, but they can only be set once, when the app configures them, hence why they have `init` in them.

### SymmetricSecurityKey
We need to wrap our `SecurityKey` in a type of bytes, that a cryptographic mechanism can work with, such as one that uses the `HMAC SHA256 algorithm` when signing tokens.

We have defined a custom getter that takes the `SecretKey` and returns a `SymmetricSecurityKey` on demand.

### TokenValidationParameters
The TokenValidationParemeters are used upon configuration of the `JWTBearer` options. Here we are defining what validations should be made to the tokens received/created.

### Extension method
The extension method is designed to retrieve the `JwtSettings` section from `appsettings.json` and bind matching properties with the ones of our `jwtSettings` class.

Later we are adding this class to `Dependency Injection` as a singleton, which means its lifestime will last for as long as the application is running and is shared among whoever requests/uses it. It makes sense to use this class as a singleton because the settings will never change during runtime, they are set once and are mostly meant for reading, although its considered good practice to rotate security keys every now and then.

```c#
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

    public static WebApplication UseAuthMiddlewares(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
```

After configuring JWT, we apply `AddAuthorization` policies to the services, which we will need when for example using `[Authorize]` on controllers.

The `UseAuthMiddlewares` static method enables `Authentication` and `Authorization`, these are part of the `Microsoft.AspNetCore` namespace and not the `JWT` package. The order matters though, authentication should always come before authorization.

Obviously we could add more options here, like `RequireHttpsMetadata` which enforces `HTTPS` for production, but since this is a minimal practice app, im not bothering with bloated configurations.

### Program.cs
Finally we just add the extension method to `Program.cs` along with the middlewares.

```c#
builder.Services.AddDatabase(builder.Configuration) // Database context DI
                .AddJwtAuthentication(builder.Configuration) // JWT DI

app.UseAuthMiddlewares() // Enables Auth/Authorization middlewares
```

### Behind the scenes
There is a lot of moving parts here, so it can be confusing as of what `Authentication` and what `Authorization` is, to add context:

### Authentication
The services `AddAuthentication`, `AddJwtBearer` and `UseAuthentication` are responsible for user validation and ensuring token is generated.

### Authorization
The services `AddAuthorization` and `UseAuthorization` are responsible for giving access to the right user asking for it. In this application there is no `RBA` policies as im just using the `[Authorize]` attribute, however these services provide default policies, which are required in order to be able to use that attribute at all.

If I wanted to add `RBA`, such as `[Authorize(Roles = "Admin")]`, we wont need to configure much on the domain layer.

# Technologies
- .NET 9
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Swashbuckle.AspNetCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- System.IdentityModel.Tokens.Jwt

# Usage
- Clone the repo.
- Create `appsettings.json` and apply necessary details:
    ```json
    {
    "ConnectionStrings": {
        "Default": "server=<host>;database=<dbname>;user=<username>;password=<userpassword>"
    },
    "JwtSettings": {
        "SecretKey": "<ReplaceWithAAReallyLongKeyOfAtLesat40Character>",
        "Issuer": "<MyIssuer>",
        "Audience": "<MyAudience>",
        "ExpiryMinutes": 60
        }
    }
    ```
- Apply migrations with `dotnet ef database update`
- Run it with `dotnet run`.
- Visit `/swagger` for the available endpoints. 

# Author
[Yosmel Chiang](https://github.com/yosang)