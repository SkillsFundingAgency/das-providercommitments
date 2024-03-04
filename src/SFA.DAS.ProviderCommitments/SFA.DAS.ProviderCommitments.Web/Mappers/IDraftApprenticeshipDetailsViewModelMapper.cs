using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class IDraftApprenticeshipDetailsViewModelMapper : IMapper<DraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IModelMapper _modelMapper;

        public IDraftApprenticeshipDetailsViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IModelMapper modelMapper)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _modelMapper = modelMapper;
        }

        public async Task<IDraftApprenticeshipViewModel> Map(DraftApprenticeshipRequest source)
        {
            var cohort = await _commitmentsApiClient.GetCohort(source.CohortId);

            if (cohort.WithParty != Party.Provider)
            {
                return await _modelMapper.Map<ViewDraftApprenticeshipViewModel>(source);
            }

            return await _modelMapper.Map<IDraftApprenticeshipViewModel>(new EditDraftApprenticeshipRequest
            {
                Cohort = cohort,
                Request = source
            });
        }
    }
}