using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class PriceViewModelMapper : IMapper<PriceRequest, PriceViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public PriceViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public  async Task<PriceViewModel> Map(PriceRequest source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);

            return new PriceViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Price = source.Price,
                InEditMode = source.Price.HasValue,
                LegalEntityName = apprenticeship.EmployerName
            };
        }
    }
}
