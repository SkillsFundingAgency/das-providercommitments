using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class IDraftApprenticeshipDetailsViewModelMapper(
        ICommitmentsApiClient commitmentsApiClient,
        IModelMapper modelMapper)
        : IMapper<DraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
    {
        public async Task<IDraftApprenticeshipViewModel> Map(DraftApprenticeshipRequest source)
        {
            var cohort = await commitmentsApiClient.GetCohort(source.CohortId);

            if (cohort.WithParty != Party.Provider)
            {
                return await modelMapper.Map<ViewDraftApprenticeshipViewModel>(source);
            }

            return await modelMapper.Map<IDraftApprenticeshipViewModel>(new EditDraftApprenticeshipRequest
            {
                Cohort = cohort,
                Request = source
            });
        }
    }
}