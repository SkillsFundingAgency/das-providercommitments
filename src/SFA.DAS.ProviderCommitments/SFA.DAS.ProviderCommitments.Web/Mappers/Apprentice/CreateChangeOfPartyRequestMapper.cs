using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class CreateChangeOfPartyRequestMapper : IMapper<ConfirmViewModel, ChangeOfPartyRequestRequest>
    {
        public Task<ChangeOfPartyRequestRequest> Map(ConfirmViewModel source)
        {
            return Task.FromResult(new ChangeOfPartyRequestRequest
            {
                ChangeOfPartyRequestType = ChangeOfPartyRequestType.ChangeEmployer,
                NewPartyId = source.AccountLegalEntityId,
                NewPrice = source.NewPrice,
                NewStartDate = source.NewStartDateTime
            });
        }
    }
}
