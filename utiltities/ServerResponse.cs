public class ServerResponse {
  public long datetime { get; } = getTimestamp();
  public string info { get; set; } = Configuration.RESPONSE_MESSAGES.ok;
  public int status { get; set; } = Configuration.RESPONSE_STATUSES.ok;

  static long getTimestamp()
  {
      DateTime currentTime = DateTime.UtcNow;
      return ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
  }
}

public class ServerResponse<T> : ServerResponse {
  public T? data { get; set; }
}
