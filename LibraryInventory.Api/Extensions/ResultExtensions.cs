using FluentValidation.Results;
using LibraryInventory.Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace LibraryInventory.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToFailure(this ControllerBase c, ErrorType e, string m)
    {
        return e switch
        {
            ErrorType.NotFound => c.NotFound(new { error = m }),
            ErrorType.Conflict => c.Conflict(new { error = m }),
            ErrorType.RuleViolation => c.UnprocessableEntity(new { error = m }),
            _ => c.BadRequest(new { error = m })
        };
    }

    public static IActionResult ToValidationProblem(this ControllerBase c, ValidationResult v)
    {
        return c.ValidationProblem(new ValidationProblemDetails(v.Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())));
    }
}