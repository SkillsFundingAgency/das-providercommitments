using System;
using System.Linq;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderCommitments.Extensions
{
    public static class ITrainingCourseExtensions
    {
        public static bool IsActiveOn(this ITrainingCourse course, DateTime date)
        {
            return GetStatusOn(course.EffectiveFrom, course.EffectiveTo, date) == TrainingCourseStatus.Active;
        }

        public static TrainingCourseStatus GetStatusOn(this ITrainingCourse course, DateTime date)
        {
            return GetStatusOn(course.EffectiveFrom, course.EffectiveTo, date);
        }

        public static int FundingCapOn(this ITrainingCourse course, DateTime date)
        {
            //todo: would probably be better to return int? null or throw if out of range
            if (!course.IsActiveOn(date))
                return 0;

            var applicableFundingPeriod = course.FundingPeriods.FirstOrDefault(x => GetStatusOn(x.EffectiveFrom, x.EffectiveTo, date) == TrainingCourseStatus.Active);

            return applicableFundingPeriod?.FundingCap ?? 0;
        }

        /// <remarks>
        /// we make use of the same logic to determine ActiveOn and FundingBandOn so that if the programme is active, it should fall within a funding band
        /// </remarks>
        private static TrainingCourseStatus GetStatusOn(DateTime? effectiveFrom, DateTime? effectiveTo, DateTime date)
        {
            var dateOnly = date.Date;

            if (effectiveFrom.HasValue && effectiveFrom.Value.FirstOfMonth() > dateOnly)
                return TrainingCourseStatus.Pending;

            if (!effectiveTo.HasValue || effectiveTo.Value >= dateOnly)
                return TrainingCourseStatus.Active;

            return TrainingCourseStatus.Expired;
        }
    }
}