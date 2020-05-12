using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateViewModelMapper : IMapper<EndDateRequest, EndDateViewModel>
    {
        public Task<EndDateViewModel> Map(EndDateRequest source)
        {
            return Task.FromResult(new EndDateViewModel
            {
                ProviderId = source.ProviderId,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate,
                EndDate = new MonthYearModel(source.EndDate),
                Price = source.Price
            });
        }
    }
}