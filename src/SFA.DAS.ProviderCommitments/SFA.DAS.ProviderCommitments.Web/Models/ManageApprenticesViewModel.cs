using System.Collections.Generic;
using System.Linq;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Models
{
    public class ManageApprenticesViewModel
    {
        public long ProviderId { get; set; }
        public bool AnyApprenticeships => Apprenticeships != null && Apprenticeships.Any();
        public IEnumerable<ApprenticeshipDetailsResponse> Apprenticeships { get; set; }
        public string SortField { get; set; }
        public bool ReverseSort { get; set; }
        public string SortedByHeaderClassName { get; set; }
        public const string HeaderClassName = "das-table__sort";
    }
}