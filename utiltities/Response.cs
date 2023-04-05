public class ServerResponse {
  public string? info = Configuration.RESPONSE_MESSAGES.ok;
  public int? status = Configuration.RESPONSE_STATUSES.ok;
}

public class ServerResponse<T>: ServerResponse {
  public T? data;
}
