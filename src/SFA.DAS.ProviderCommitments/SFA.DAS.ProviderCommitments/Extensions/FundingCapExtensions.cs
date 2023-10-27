using System;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.ProviderCommitments.Extensions
{
    
    public static class FundingCapExtension
    {
        public static int FundingCapOn(this TrainingProgramme course, DateTime date)
        {
            if (!IsActive(course.EffectiveTo, course.EffectiveFrom, date))
                return 0;

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x => IsActive(x.EffectiveTo,x.EffectiveFrom,date));

            return applicableFundingPeriod?.FundingCap ?? 0;
        }

        public static int FundingCapOn(this GetStandardResponse course, DateTime date)
        {
            if (!IsActive(course.EffectiveTo, course.EffectiveFrom, date))
                return 0;

            var applicableFundingPeriod = course.ApprenticeshipFunding.FirstOrDefault(x => IsActive(x.EffectiveTo, x.EffectiveFrom, date));

            return applicableFundingPeriod?.MaxEmployerLevyCap ?? 0;
        }

        private static bool IsActive(DateTime? effectiveTo,DateTime? effectiveFrom, DateTime date)
        {
            var dateOnly = date.Date;

            if (effectiveFrom.HasValue && effectiveFrom.Value.FirstOfMonth() > dateOnly)
                return false;
            
            if (!effectiveTo.HasValue || effectiveTo.Value >= dateOnly)
                return true;

            return false;
        }

        public static bool IsActiveOn(this TrainingProgramme course, DateTime date)
        {
            return IsActive(course.EffectiveTo, course.EffectiveFrom, date);
        }
    }
    
}