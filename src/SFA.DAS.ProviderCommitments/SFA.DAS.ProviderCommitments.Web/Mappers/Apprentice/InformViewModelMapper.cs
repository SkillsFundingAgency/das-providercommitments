using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class InformViewModelMapper : IMapper<InformRequest, InformViewModel>
    {
        public Task<InformViewModel> Map(InformRequest source)
        {
            return Task.FromResult(new InformViewModel
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                ApprenticeshipId = source.ApprenticeshipId
            });
        }
    }
}