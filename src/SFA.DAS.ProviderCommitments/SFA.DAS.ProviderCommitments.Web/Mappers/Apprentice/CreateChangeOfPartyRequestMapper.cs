using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class CreateChangeOfPartyRequestMapper : IMapper<ConfirmViewModel, CreateChangeOfPartyRequestRequest>
    {
        public Task<CreateChangeOfPartyRequestRequest> Map(ConfirmViewModel source)
        {
            return Task.FromResult(new CreateChangeOfPartyRequestRequest
            {
                ChangeOfPartyRequestType = ChangeOfPartyRequestType.ChangeEmployer,
                NewPartyId = source.AccountLegalEntityId,
                NewPrice = source.NewPrice,
                NewStartDate = source.NewStartDateTime,
                EndDate = source.NewEndDateTime
            });
        }
    }
}
