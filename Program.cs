using dotenv.net;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SharpBasics.Models;
using System.Web;

// load dotenv
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

var builder = WebApplication.CreateBuilder(args);
var databaseConnection = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

// connect database
builder.Services.AddDbContext<DB>(options => options.UseNpgsql(databaseConnection));

// swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
     options.SwaggerDoc(
      "v1",
      new OpenApiInfo
      {
        Title = "SHARP BASICS",
        Description = "Basic C# backend application with ASP.NET",
        Version = "v1"
      }
    );
});

// set application port
var port = Environment.GetEnvironmentVariable("PORT");
if (port == "" || port == null) {
  port = "9000";
}
builder.WebHost.UseUrls($"http://*:{port}");
var app = builder.Build();

// use swagger middleware
app.UseSwagger();
app.UseSwaggerUI(options =>
{
   options.SwaggerEndpoint("/swagger/v1/swagger.json", "sharp-basics API v1");
});

// ping the server
app.MapGet("/", () => Results.Ok(new ServerResponse()));
app.MapGet($"/{Configuration.API_PREFIX}", () => Results.Ok(new ServerResponse()));

// sign-in
app.MapPost(
  $"/{Configuration.API_PREFIX}/sign-in", 
  () => Results.Ok(new ServerResponse())
);

// sign-up
app.MapPost($"/{Configuration.API_PREFIX}/sign-up", async (DB db, HttpContext context) => {
  string body = await new StreamReader(context.Request.Body).ReadToEndAsync();
  var parsed = HttpUtility.ParseQueryString(body);
  string json = JsonConvert.SerializeObject(
    parsed.Cast<string>().ToDictionary(k => k, v => parsed[v])
  );
  SignUpDTO? data = JsonConvert.DeserializeObject<SignUpDTO>(json);
  if (data == null || data.email == null
    || data.name == null || data.password == null)
  {
    context.Response.StatusCode = Configuration.RESPONSE_STATUSES.badRequest;
    await context.Response.WriteAsJsonAsync<ServerResponse>(new ServerResponse
    {
      info = Configuration.RESPONSE_MESSAGES.missingData,
      status = Configuration.RESPONSE_STATUSES.badRequest,
    });
    return;
  }

  string trimmedEmail = data.email.Trim().ToLower();
  string trimmedName = data.name.Trim();
  string trimmedPassword = data.password.Trim();
  if (trimmedEmail == "" || trimmedName == "" || trimmedPassword == "")
  {
    context.Response.StatusCode = Configuration.RESPONSE_STATUSES.badRequest;
    await context.Response.WriteAsJsonAsync<ServerResponse>(new ServerResponse
    {
      info = Configuration.RESPONSE_MESSAGES.missingData,
      status = Configuration.RESPONSE_STATUSES.badRequest,
    });
    return;
  }

  var existingUser = await db.Users.FirstOrDefaultAsync(
    user => user.Email == trimmedEmail
  );
  if (existingUser != null)
  {
    context.Response.StatusCode = Configuration.RESPONSE_STATUSES.badRequest;
    await context.Response.WriteAsJsonAsync<ServerResponse>(new ServerResponse
    {
      info = Configuration.RESPONSE_MESSAGES.emailAreadyInUse,
      status = Configuration.RESPONSE_STATUSES.badRequest,
    });
    return;
  }

  string passwordHash = Argon2.Hash(trimmedPassword);

  var newUser = new User
  {
    Email = trimmedEmail,
    Name = trimmedName,
  };
  await db.Users.AddAsync(newUser);
  await db.Passwords.AddAsync(new Password
  {
    Hash = passwordHash,
    UserId = newUser.Id,
  });
  await db.SaveChangesAsync();

  string token = JWTService.createToken($"{newUser.Id}");

  await context.Response.WriteAsJsonAsync<ServerResponse<Dictionary<string, object>>>(
    new ServerResponse<Dictionary<string, object>>
    {
      data = new Dictionary<string, object>
      {
        { "token", token },
        { "user", newUser },
      }
    }
  );
  return;
});

app.Run();
