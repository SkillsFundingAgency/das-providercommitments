using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ChangePriceViewModelMapper : IMapper<ChangePriceRequest, ChangePriceViewModel>
    {
        public Task<ChangePriceViewModel> Map(ChangePriceRequest source)
        {
            return Task.FromResult(new ChangePriceViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                NewStartDate = source.NewStartDate
            });
        }
    }
}
