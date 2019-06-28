using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using Framework = SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse.Framework;
using FundingPeriod = SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse.FundingPeriod;
using Standard = SFA.DAS.Commitments.Shared.Models.ApprenticeshipCourse.Standard;

namespace SFA.DAS.ProviderCommitments.Mappers
{
    public class ApprenticeshipInfoServiceMapper : IApprenticeshipInfoServiceMapper
    {
        private readonly ICurrentDateTime _currentDateTime;

        public ApprenticeshipInfoServiceMapper(ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
        }

        public FrameworksView MapFrom(FrameworkSummary[] frameworks)
        {
            return new FrameworksView
            {
                CreatedDate = _currentDateTime.Now,
                Frameworks = frameworks.Select(x => new Framework
                {
                    Id = x.Id,
                    Title = GetTitle(x.FrameworkName.Trim() == x.PathwayName.Trim() ? x.FrameworkName : x.Title, x.Level),
                    FrameworkCode = x.FrameworkCode,
                    FrameworkName = x.FrameworkName,
                    Level = x.Level,
                    PathwayCode = x.PathwayCode,
                    PathwayName = x.PathwayName,
                    Duration = x.Duration,
                    MaxFunding = x.CurrentFundingCap,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo = x.EffectiveTo,
                    FundingPeriods = MapFundingPeriods(x.FundingPeriods)
                }).ToArray()
            };
        }

        public ProvidersView MapFrom(Apprenticeships.Api.Types.Providers.Provider provider)
        {
            return new ProvidersView
            {
                CreatedDate = _currentDateTime.Now,
                Provider = new Provider
                {
                    Ukprn = provider.Ukprn,
                    ProviderName = provider.ProviderName,
                    Email = provider.Email,
                    Phone = provider.Phone,
                    NationalProvider = provider.NationalProvider
                }
            };
        }

        public StandardsView MapFrom(StandardSummary[] standards)
        {
            return new StandardsView
            {
                CreationDate = _currentDateTime.Now,
                Standards = standards.Select(x => new Standard
                {
                    Id = x.Id,
                    Level = x.Level,
                    Title = GetTitle(x.Title, x.Level) + " (Standard)",
                    Duration = x.Duration,
                    MaxFunding = x.CurrentFundingCap,
                    EffectiveFrom = x.EffectiveFrom,
                    EffectiveTo = x.LastDateForNewStarts,
                    FundingPeriods = MapFundingPeriods(x.FundingPeriods)
                }).ToArray()
            };
        }

        private static IEnumerable<FundingPeriod> MapFundingPeriods(IEnumerable<Apprenticeships.Api.Types.FundingPeriod> source)
        {
            if (source == null)
                return Enumerable.Empty<FundingPeriod>();

            return source.Select(x => new FundingPeriod
            {
                EffectiveFrom = x.EffectiveFrom,
                EffectiveTo = x.EffectiveTo,
                FundingCap = x.FundingCap
            }).OrderBy(y => y.EffectiveFrom ?? DateTime.MinValue);
        }

        private static string GetTitle(string title, int level)
        {
            return $"{title}, Level: {level}";
        }
    }
}