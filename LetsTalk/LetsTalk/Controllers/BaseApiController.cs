using LetsTalk.Shared.Api;
using Microsoft.AspNetCore.Mvc;

namespace LetsTalk.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected new ApiResponse<T> Response<T>(string message, T? data)
    {
        return new ApiResponse<T>(true, message, data);
    }
    
    protected ApiResponse<object> ResponseError(string message)
    {
        return new ApiResponse<object>(false, message, null);
    }
}