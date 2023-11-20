using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;

namespace SFA.DAS.ProviderCommitments.Web.Extensions;

public static class OuterApiPriceEpisodeExtensions
{

    public static decimal GetCost(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes)
    {
        return priceEpisodes.GetCost(DateTime.UtcNow);
    }

    public static decimal GetCost(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes,
        DateTime effectiveDate)
    {
        return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).Cost;
    }

    public static decimal? GetTrainingPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes)
    {
        return priceEpisodes.GetTrainingPrice(DateTime.UtcNow);
    }

    public static decimal? GetTrainingPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes,
        DateTime effectiveDate)
    {
        return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).TrainingPrice;
    }

    public static decimal? GetEndPointAssessmentPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes)
    {
        return priceEpisodes.GetEndPointAssessmentPrice(DateTime.UtcNow);
    }

    public static decimal? GetEndPointAssessmentPrice(this IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes,
        DateTime effectiveDate)
    {
        return GetEffectivePriceEpisode(priceEpisodes, effectiveDate).EndPointAssessmentPrice;
    }

    private static GetManageApprenticeshipDetailsResponse.PriceEpisode GetEffectivePriceEpisode(IEnumerable<GetManageApprenticeshipDetailsResponse.PriceEpisode> priceEpisodes, DateTime effectiveDate)
    {
        var episodes = priceEpisodes.ToList();

        var episode = episodes.FirstOrDefault(x =>
            x.FromDate <= effectiveDate && (x.ToDate == null || x.ToDate >= effectiveDate));

        return episode ?? episodes.First();
    }
}