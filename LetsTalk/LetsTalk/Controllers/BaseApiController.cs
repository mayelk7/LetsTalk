using LetsTalk.Shared.Api;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected new static ApiResponse<T> Response<T>(string message, T? data)
    {
        return new ApiResponse<T>(true, message, data);
    }
    
    protected static ApiResponse<object> ResponseError(string message)
    {
        return new ApiResponse<object>(false, message, null);
    }
}