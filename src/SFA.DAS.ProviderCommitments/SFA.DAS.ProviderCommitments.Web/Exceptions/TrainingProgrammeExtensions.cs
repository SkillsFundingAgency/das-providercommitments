using SFA.DAS.CommitmentsV2.Types;
using System;
using SFA.DAS.ProviderCommitments.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Exceptions
{
    public static class TrainingProgrammeExtensions
    {
        public static int? GetFundingBandCap(this TrainingProgramme course, DateTime? startDate)
        {
            if (course == null || !startDate.HasValue) return null;

            var cap = course.FundingCapOn(startDate.Value);

            return cap > 0 ? cap : null;
        }
    }
}
