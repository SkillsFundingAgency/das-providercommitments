using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class InnerApiPriceEpisodeExtensions
    {
        public static decimal GetCost(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetCost(DateTime.UtcNow);
        }

        public static decimal GetCost(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).Cost;
        }

        private static GetPriceEpisodesResponse.PriceEpisode GetEffectivePriceEpisode(
            IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes, DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode ?? episodes.First();
        }
    }
}
