using dotenv.net;
using Newtonsoft.Json;
using System.Web;

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

// sign-in
app.MapPost($"/{Configuration.API_PREFIX}", async (HttpRequest request) => {
  return "hey";
});

// sign-up
app.MapPost($"/{Configuration.API_PREFIX}/sign-up", async (HttpRequest request) => {
  string body = await new StreamReader(request.Body).ReadToEndAsync();
  var parsed = HttpUtility.ParseQueryString(body);
  string json = JsonConvert.SerializeObject(
    parsed.Cast<string>().ToDictionary(k => k, v => parsed[v])
  );
  SignUpDTO? data = JsonConvert.DeserializeObject<SignUpDTO>(json);
  if (data == null || data.email == null
    || data.name == null || data.password == null) {
    ServerResponse res = new ServerResponse();
    res.info = Configuration.RESPONSE_MESSAGES.missingData;
    res.status = Configuration.RESPONSE_STATUSES.badRequest;
    return JsonConvert.SerializeObject(res);
  }

  string trimmedEmail = data.email.Trim();
  string trimmedName = data.name.Trim();
  string trimmedPassword = data.password.Trim();

  // TODO: check if email address is already in use

  // TODO: create JWT

  

  return "hey";
});

app.Run();
