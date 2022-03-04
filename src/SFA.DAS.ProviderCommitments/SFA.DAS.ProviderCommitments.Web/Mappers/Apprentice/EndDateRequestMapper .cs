using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EndDateRequestMapper : IMapper<StartDateViewModel, EndDateRequest>
    {
        public Task<EndDateRequest> Map(StartDateViewModel source)
        {
            return Task.FromResult(new EndDateRequest
            {
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                StartDate = source.StartDate.MonthYear,
                DeliveryModel = source.DeliveryModel,
            });
        }
    }
}