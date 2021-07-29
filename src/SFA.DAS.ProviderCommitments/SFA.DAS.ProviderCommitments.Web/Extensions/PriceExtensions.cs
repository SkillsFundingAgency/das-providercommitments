using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class PriceExtensions
    {
        public static string FormatCost(this decimal? cost)
        {
            if (!cost.HasValue) return string.Empty;
            return $"£{cost.Value:n0}";
        }

        public static string FormatCost(this decimal value)
        {
            return $"£{value:n0}";
        }
    }
}
