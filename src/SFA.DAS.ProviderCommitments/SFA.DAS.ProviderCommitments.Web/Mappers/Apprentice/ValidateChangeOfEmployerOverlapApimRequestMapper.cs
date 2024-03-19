using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class
        ValidateChangeOfEmployerOverlapApimRequestMapper : IMapper<TrainingDatesViewModel,
            ValidateChangeOfEmployerOverlapApimRequest>
    {
        public async Task<ValidateChangeOfEmployerOverlapApimRequest> Map(TrainingDatesViewModel source)
        {
            return await Task.FromResult(new ValidateChangeOfEmployerOverlapApimRequest
            {
                Uln = source.Uln,
                StartDate = source.StartDate.Date.Value.ToString("dd-MM-yyyy"),
                EndDate = source.EndDate.Date.Value.ToString("dd-MM-yyyy"),
                ProviderId = source.ProviderId
            });
        }
    }
}