using System;
using JwtAuthApp.Application.Common;
using JwtAuthApp.Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthApp.API.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult HandleError<T>(Result<T> result)
    {
        return result.ErrorType switch
        {
            ErrorType.Validation => BadRequest(result),
            ErrorType.NotFound => NotFound(result),
            ErrorType.Conflict => Conflict(result),
            ErrorType.Unauthorized => Unauthorized(result),
            _ => StatusCode(StatusCodes.Status500InternalServerError,
                result)
        };
    }
}
