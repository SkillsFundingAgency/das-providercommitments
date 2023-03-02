using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class ModelStateExtensions
{
    public static void AddModelExceptionErrors(
        this ModelStateDictionary modelState,
        List<ErrorDetail> errors,
        Func<string, string> fieldNameMapper = null)
    {
        if (errors == null)
            return;
        foreach (ErrorDetail error in errors)
        {
            string key = fieldNameMapper == null ? error.Field : fieldNameMapper(error.Field);
            modelState.AddModelError(key, error.Message);
        }
    }
}