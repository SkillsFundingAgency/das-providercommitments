using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper : IMapper<EditApprenticeshipRequestViewModel, SelectDeliveryModelViewModel>
    {
        private readonly ISelectDeliveryModelMapperHelper _helper;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper(ISelectDeliveryModelMapperHelper helper, ICommitmentsApiClient commitmentsApiClient)
        {
            _helper = helper;
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<SelectDeliveryModelViewModel> Map(EditApprenticeshipRequestViewModel source)
        {
            var apprenticeship = await _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId);
            return await _helper.Map(source.ProviderId, source.CourseCode, apprenticeship.AccountLegalEntityId, source.DeliveryModel);
        }
    }
}