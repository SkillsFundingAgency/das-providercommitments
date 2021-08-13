using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class ViewStandardOptionsViewModelMapper : IMapper<SelectOptionsRequest, ViewSelectOptionsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public ViewStandardOptionsViewModelMapper (ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }
        public async Task<ViewSelectOptionsViewModel> Map(SelectOptionsRequest source)
        {
            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);
            var options = await _commitmentsApiClient.GetStandardOptions(draftApprenticeship.StandardUId);

            return new ViewSelectOptionsViewModel
            {
                CohortId = source.CohortId,
                CohortReference = source.CohortReference,
                ProviderId = source.ProviderId,
                DraftApprenticeshipId = source.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                TrainingCourseName = draftApprenticeship.TrainingCourseName,
                TrainingCourseVersion = draftApprenticeship.TrainingCourseVersion,
                Options = options.Options != null ? options.Options.ToList() : new List<string>(),
                StandardIFateLink = ""
            };
        }
    }
}