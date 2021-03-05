using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class PriceRequestMapper : IMapper<EndDateViewModel, PriceRequest>
    {
        public Task<PriceRequest> Map(EndDateViewModel source)
        {
            return Task.FromResult(new PriceRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate,
                EndDate = source.EndDate.MonthYear
            });
        }
    }
}