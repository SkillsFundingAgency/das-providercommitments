using System.Collections.Generic;

namespace SFA.DAS.ProviderCommitments.Web.Models.Shared
{
    public interface IStandardSelection
    {
        public string CourseCode { get; }
        public IEnumerable<Standard> Standards { get; }
    }

    public class Standard
    {
        public string CourseCode { get; set; }
        public string Name { get; set; }
    }
}
