- [Project](#project)
- [Concepts](#concepts)
- [Dependency Injection](#dependency-injection)
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