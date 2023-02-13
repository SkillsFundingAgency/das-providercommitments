using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderCommitments.Web.Filters;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class ProviderCommitmentsMvcOptionsExtensions
{
    public static void AddProviderCommitmentsValidation(this MvcOptions mvcOptions)
    {
        mvcOptions.Filters.Add<CacheFriendlyCommitmentsValidationFilter>(int.MaxValue);
        mvcOptions.Filters.Add<CacheFriendlyValidateModelStateFilter>(int.MaxValue);
    }
}