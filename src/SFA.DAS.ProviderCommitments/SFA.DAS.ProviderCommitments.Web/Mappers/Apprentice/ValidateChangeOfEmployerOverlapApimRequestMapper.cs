using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Services.Cache;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ValidateChangeOfEmployerOverlapApimRequestMapper : IMapper<TrainingDatesViewModel, ValidateChangeOfEmployerOverlapApimRequest>
    {
        public ValidateChangeOfEmployerOverlapApimRequestMapper()
        {
        }

        public async Task<ValidateChangeOfEmployerOverlapApimRequest> Map(TrainingDatesViewModel source)
        {          
            return await Task.FromResult( new ValidateChangeOfEmployerOverlapApimRequest
            {
                Uln = source.Uln,
                StartDate = source.StartDate.Date.Value.ToString("dd-MM-yyyy"),
                EndDate = source.EndDate.Date.Value.ToString("dd-MM-yyyy"),
                ProviderId = source.ProviderId
            });
        }
    }
}

