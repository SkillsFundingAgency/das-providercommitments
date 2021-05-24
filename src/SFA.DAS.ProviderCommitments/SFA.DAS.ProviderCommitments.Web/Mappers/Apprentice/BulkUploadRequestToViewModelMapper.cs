using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class BulkUploadRequestToViewModelMapper : IMapper<BulkUploadRequest, BulkUploadViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public BulkUploadRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<BulkUploadViewModel> Map(BulkUploadRequest source)
        {
         //   var accountLegalEntity = await _commitmentsApiClient.GetAccountLegalEntity(source.AccountLegalEntityId);

         return await Task.FromResult(new BulkUploadViewModel
            {
                ProviderId = source.ProviderId
            });
        }
    }
}
