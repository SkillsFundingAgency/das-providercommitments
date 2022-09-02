using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class DraftApprenticeshipOverlapOptionViewModelToCreateOverlappingTrainingDateRequestMapper : IMapper<DraftApprenticeshipOverlapOptionViewModel, CreateOverlappingTrainingDateApimRequest>
    {
        public Task<CreateOverlappingTrainingDateApimRequest> Map(DraftApprenticeshipOverlapOptionViewModel source)
        {
            return Task.FromResult(new CreateOverlappingTrainingDateApimRequest
            {
                DraftApprenticeshipId = source.DraftApprenticeshipId.Value,
                ProviderId = source.ProviderId
            });
        }
    }
}
