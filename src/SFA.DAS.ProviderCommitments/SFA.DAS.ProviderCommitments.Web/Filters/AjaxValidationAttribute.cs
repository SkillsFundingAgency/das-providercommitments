﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;

namespace SFA.DAS.ProviderCommitments.Web.Filters
{
    public class AjaxValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = (Controller) filterContext.Controller;
            if (!controller.ModelState.IsValid)
            {
                var errors = FindErrors(controller.ModelState);
                if (errors.Any())
                {
                    // Short circuit with a JSON response
                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                    filterContext.Result = new JsonResult(errors);
                }
            }
            base.OnActionExecuting(filterContext);
        }

        internal static IEnumerable<ErrorDetail> FindErrors(ModelStateDictionary modelState)
        {
            var result = new List<ErrorDetail>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });
            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors.Select(error => new ErrorDetail(fieldKey, error.ErrorMessage));
                result.AddRange(fieldErrors);
            }
            return result;
        }
    }
}
