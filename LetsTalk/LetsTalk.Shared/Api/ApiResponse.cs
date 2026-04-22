using System.Text.Json.Serialization;

namespace LetsTalk.Shared.Api;

public class ApiResponse<T>(bool success, string message, T? data)
{
    [JsonInclude]
    public bool Success { get; init; } = success;

    [JsonInclude]
    public string Message { get; init; } = message;

    [JsonInclude]
    public T? Data { get; init; } = data;
}
