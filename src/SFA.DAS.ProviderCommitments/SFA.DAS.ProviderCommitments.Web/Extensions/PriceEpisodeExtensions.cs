using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class PriceEpisodeExtensions
    {
        public static decimal GetPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetPrice(DateTime.UtcNow);
        }

        public static decimal GetPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode?.Cost ?? episodes.First().Cost;
        }

        public static decimal GetTrainingPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetTrainingPrice(DateTime.UtcNow);
        }

        public static decimal GetTrainingPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode?.TrainingPrice ?? episodes.First().TrainingPrice;
        }

        public static decimal GetEndPointAssessmentPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes)
        {
            return priceEpisodes.GetEndPointAssessmentPrice(DateTime.UtcNow);
        }

        public static decimal GetEndPointAssessmentPrice(this IEnumerable<GetPriceEpisodesResponse.PriceEpisode> priceEpisodes,
            DateTime effectiveDate)
        {
            var episodes = priceEpisodes.ToList();

            var episode = episodes.FirstOrDefault(x =>
                x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

            return episode?.EndPointAssessmentPrice ?? episodes.First().EndPointAssessmentPrice;
        }
    }
}
