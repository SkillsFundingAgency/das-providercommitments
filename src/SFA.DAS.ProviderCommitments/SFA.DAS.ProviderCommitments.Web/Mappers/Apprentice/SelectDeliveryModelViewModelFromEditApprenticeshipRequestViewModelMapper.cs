using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper : IMapper<EditApprenticeshipRequestViewModel, SelectDeliveryModelViewModel>    {
        private readonly ISelectDeliveryModelMapperHelper _helper;

        public SelectDeliveryModelViewModelFromEditApprenticeshipRequestViewModelMapper(ISelectDeliveryModelMapperHelper helper)
        {
            _helper = helper;
        }

        public Task<SelectDeliveryModelViewModel> Map(EditApprenticeshipRequestViewModel source)
        {
            return _helper.Map(source.ProviderId, source.CourseCode, source.DeliveryModel);
        }
    }
}