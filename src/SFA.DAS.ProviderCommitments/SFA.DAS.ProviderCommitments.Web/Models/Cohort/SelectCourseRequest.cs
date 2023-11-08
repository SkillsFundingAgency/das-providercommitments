using System;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public class SelectCourseRequest
    {
        public long ProviderId { get; set; }
        public Guid CacheKey { get; set; }
        public string CourseCode { get; set; }
    }
}
