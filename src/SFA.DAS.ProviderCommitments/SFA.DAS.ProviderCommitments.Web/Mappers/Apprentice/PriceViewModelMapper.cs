using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class PriceViewModelMapper : IMapper<PriceRequest, PriceViewModel>
    {
        public Task<PriceViewModel> Map(PriceRequest source)
        {
            return Task.FromResult(new PriceViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                Price = source.Price,
                InEditMode = source.Price.HasValue
            });
        }
    }
}
