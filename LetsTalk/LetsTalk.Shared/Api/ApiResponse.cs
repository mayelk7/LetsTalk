using System.Text.Json.Serialization;

namespace LetsTalk.Shared.Api;

public class ApiResponse<T>(bool success, string message, T? data, string? token = null, bool requires2FA = false)
{
    [JsonInclude]
    public bool Success { get; init; } = success;

    [JsonInclude]
    public string Message { get; init; } = message;

    [JsonInclude]
    public T? Data { get; init; } = data;

    [JsonInclude]
    public string? Token { get; init; } = token;

    [JsonInclude]
    public bool Requires2FA { get; init; } = requires2FA;
}