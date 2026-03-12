# Project
In this little project we are implementing a ASP.NET Core Web Api with Controllers and connecting it to a minimalistic MySQL database to perform simple CRUD operations using JWT authentication.


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