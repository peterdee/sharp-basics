using JWT;

public class JWTService {
  public static string createToken(string userId)
  {
    var timeOffset = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();
    var tokenPayload = new Dictionary<string, object>
    {
      { "exp", timeOffset },
      { "id", userId },
    };

    // TODO: replace HS256 with another algorithm
    JWT.Algorithms.IJwtAlgorithm algorithm = new JWT.Algorithms.HMACSHA256Algorithm();
    JWT.Serializers.JsonNetSerializer serializer = new JWT.Serializers.JsonNetSerializer();
    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
    IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

    var tokenSecret = Environment.GetEnvironmentVariable("TOKEN_SECRET");
    if (tokenSecret == "" || tokenSecret == null) {
      tokenSecret = Configuration.DEFAULT_TOKEN_SECRET;
    }

    return encoder.Encode(tokenPayload, tokenSecret);
  }
}
