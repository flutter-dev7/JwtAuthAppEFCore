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
            ErrorType.Validation => BadRequest(result.Error),
            ErrorType.NotFound => NotFound(result.Error),
            ErrorType.Conflict => Conflict(result.Error),
            ErrorType.Unauthorized => Unauthorized(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.")
        };
    }
}
