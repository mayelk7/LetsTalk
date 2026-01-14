using System.Text;
using System.Text.Json;
using LetsTalk.Shared.Enum;
using LetsTalk.Shared.ModelsDto;

namespace LetsTalk.Client.Services;

public static class ApiManagerService
{
    /// <summary>
    ///     HttpClient instance with predefined BaseAddress for API requests.
    /// </summary>
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5278/api/"),
    };

    /// <summary>
    ///     Makes a GET request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// 
    /// <param name="endpoint">
    ///     The API endpoint to send the GET request to.
    /// </param>
    /// 
    /// <typeparam name="T">
    ///     The type to deserialize the response into.
    /// </typeparam>
    /// 
    /// <returns>
    ///     The deserialized response of type T, or null if the response content is empty.
    /// </returns>
    public static async Task<T?> MakeGetRequest<T>(string endpoint)
    {
        HttpClient.DefaultRequestHeaders.Add("X-ApiManager", endpoint);
        var response = await HttpClient.GetAsync(endpoint);
        
        var responseContent = await response.Content.ReadAsStringAsync();

        return responseContent == string.Empty ? default : JsonSerializer.Deserialize<T>(responseContent, JsonSerializerOptions.Web);
    }

    /// <summary>
    ///     Makes a POST request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// 
    /// <param name="endpoint">
    ///     The API endpoint to send the GET request to.
    /// </param>
    /// <param name="dto">
    ///     The data transfer object to be sent in the POST request.
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize the response into.
    /// </typeparam>
    /// <typeparam name="TD">
    ///     The type of the data transfer object to be sent in the POST request.
    /// </typeparam>
    /// 
    /// <returns>
    ///     The deserialized response of type T, or null if the response content is empty.
    /// </returns>
    public static async Task<T?> MakePostRequest<T, TD>(string endpoint, TD dto)
    {
        var json = JsonSerializer.Serialize(dto);
        
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        HttpClient.DefaultRequestHeaders.Add("X-ApiManager", endpoint);
        var response = await HttpClient.PostAsync(endpoint, content);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return responseContent == string.Empty ? default : JsonSerializer.Deserialize<T>(responseContent, JsonSerializerOptions.Web);
    }
    
    /// <summary>
    ///     Makes a DELETE request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// 
    /// <param name="endpoint">
    ///     The API endpoint to send the GET request to.
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize the response into.
    /// </typeparam>
    /// 
    /// <returns>
    ///     The deserialized response of type T, or null if the response content is empty.
    /// </returns>
    public static async Task<T?> MakeDeleteRequest<T>(string endpoint)
    {
        HttpClient.DefaultRequestHeaders.Add("X-ApiManager", endpoint);
        var response = await HttpClient.DeleteAsync(endpoint);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return responseContent == string.Empty ? default : JsonSerializer.Deserialize<T>(responseContent, JsonSerializerOptions.Web);
    }
    
    /// <summary>
    ///     Makes a PUT request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// 
    /// <param name="endpoint">
    ///     The API endpoint to send the GET request to.
    /// </param>
    /// <param name="dto">
    ///     The data transfer object to be sent in the POST request.
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize the response into.
    /// </typeparam>
    /// <typeparam name="TD">
    ///     The type of the data transfer object to be sent in the POST request.
    /// </typeparam>
    /// 
    /// <returns>
    ///     The deserialized response of type T, or null if the response content is empty.
    /// </returns>
    public static async Task<T?> MakePutRequest<T, TD>(string endpoint, TD dto)
    {
        var json = JsonSerializer.Serialize(dto);
        
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        HttpClient.DefaultRequestHeaders.Add("X-ApiManager", endpoint);
        var response = await HttpClient.PutAsync(endpoint, content);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return responseContent == string.Empty ? default : JsonSerializer.Deserialize<T>(responseContent, JsonSerializerOptions.Web);
    }
    
    /// <summary>
    ///     Makes a PATCH request to the specified endpoint and deserializes the response into the specified type.
    /// </summary>
    /// 
    /// <param name="endpoint">
    ///     The API endpoint to send the GET request to.
    /// </param>
    /// <param name="dto">
    ///     The data transfer object to be sent in the POST request.
    /// </param>
    /// <typeparam name="T">
    ///     The type to deserialize the response into.
    /// </typeparam>
    /// <typeparam name="TD">
    ///     The type of the data transfer object to be sent in the POST request.
    /// </typeparam>
    /// 
    /// <returns>
    ///     The deserialized response of type T, or null if the response content is empty.
    /// </returns>
    public static async Task<T?> MakePatchRequest<T, TD>(string endpoint, TD dto)
    {
        var json = JsonSerializer.Serialize(dto);
        
        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        HttpClient.DefaultRequestHeaders.Add("X-ApiManager", endpoint);
        var response = await HttpClient.PatchAsync(endpoint, content);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        return responseContent == string.Empty ? default : JsonSerializer.Deserialize<T>(responseContent, JsonSerializerOptions.Web);
    }
}
