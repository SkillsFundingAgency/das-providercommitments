using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.Commitments.Shared.Models
{
    public class GetApprenticeshipsFilteredResult
    {
        public IEnumerable<ApprenticeshipDetails> Apprenticeships { get; set; }
        public uint NumberOfRecordsFound { get; set; }
    }
}
