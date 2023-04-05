public class Configuration {
  public const string API_PREFIX = "api"; 
  public const string DEFAULT_PORT = "9000";

  public class RESPONSE_MESSAGES {
    public const string missingData = "MISSING_DATA";
    public const string ok = "OK";
  }

  public class RESPONSE_STATUSES {
    public const int badRequest = 400;
    public const int ok = 200;
  }
}
