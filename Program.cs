using dotenv.net;
using Newtonsoft.Json;
using System.Web;
using Isopoh.Cryptography.Argon2;

// load dotenv
DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: false));

var builder = WebApplication.CreateBuilder(args);

// get port & launch on port
var port = Environment.GetEnvironmentVariable("PORT");
if (port == "" || port == null) {
  port = "9000";
}
builder.WebHost.UseUrls($"http://*:{port}");
var app = builder.Build();

// ping the server
app.MapGet("/", () => Results.Ok(new ServerResponse()));
app.MapGet($"/{Configuration.API_PREFIX}", () => Results.Ok(new ServerResponse()));

// sign-in
app.MapPost(
  $"/{Configuration.API_PREFIX}/sign-in", 
  () => Results.Ok(new ServerResponse())
);

// sign-up
app.MapPost($"/{Configuration.API_PREFIX}/sign-up", async (context) => {
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

  string trimmedEmail = data.email.Trim();
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

  // TODO: check if email address is already in use

  string passwordHash = Argon2.Hash(trimmedPassword);

  // TODO: store data in the database

  // TODO: create JWT

  await context.Response.WriteAsJsonAsync<ServerResponse<string>>(new ServerResponse<string>
  {
    data = "token",
  });
  return;
});

app.Run();
