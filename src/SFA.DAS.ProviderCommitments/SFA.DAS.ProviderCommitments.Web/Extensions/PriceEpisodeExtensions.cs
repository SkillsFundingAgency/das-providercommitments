using System;
using System.Collections.Generic;
using System.Linq;
using static SFA.DAS.CommitmentsV2.Api.Types.Responses.GetPriceEpisodesResponse;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class PriceEpisodeExtensions
    {
        public static decimal GetCost(this IEnumerable<PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetCost(DateTime.UtcNow);
        }

        public static decimal GetCost(this IEnumerable<PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).Cost;
        }

        public static decimal? GetTrainingPrice(this IEnumerable<PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetTrainingPrice(DateTime.UtcNow);
        }

        public static decimal? GetTrainingPrice(this IEnumerable<PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).TrainingPrice;
        }

        public static decimal? GetEndPointAssessmentPrice(this IEnumerable<PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetEndPointAssessmentPrice(DateTime.UtcNow);
        }

        public static decimal? GetEndPointAssessmentPrice(this IEnumerable<PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).EndPointAssessmentPrice;
        }

        private static PriceEpisode GetEffectivePriceEpisode(IEnumerable<PriceEpisode> priceEpisodes, DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode ?? episodes.First();
        }

        public static decimal GetPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetPrice(DateTime.UtcNow);
        }

        public static decimal GetPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode?.Cost ?? episodes.First().Cost;
        }


    }
}
