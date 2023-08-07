using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application;

public static class CustomBadRequestHandler
{
    public static bool IsRequestStateInvalid(ActionContext context) {
        return context.ModelState.IsValid;
    }

    public static AppResponse<object> ConstructAppResponse(ActionContext context)
    {
        var errorList = new List<string>();
        foreach (var keyModelStatePair in context.ModelState)
        {
            var key = keyModelStatePair.Key;
            var errors = keyModelStatePair.Value.Errors;
            if (errors is null) continue;

            for (var i = 0; i < errors.Count; i++)
            {
                errorList.Add($"{key}: {errors[i].ErrorMessage ?? "State invalid"}");
            }
        }

        return AppResponse<object>.Fail(errorList, "Invalid State");
    }
}
