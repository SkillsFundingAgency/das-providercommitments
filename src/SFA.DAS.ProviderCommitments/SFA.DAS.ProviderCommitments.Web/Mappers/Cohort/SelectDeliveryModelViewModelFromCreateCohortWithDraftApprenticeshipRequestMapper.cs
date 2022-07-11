using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper : IMapper<CreateCohortWithDraftApprenticeshipRequest, SelectDeliveryModelViewModel>    {
        private readonly ISelectDeliveryModelMapperHelper _helper;

        public SelectDeliveryModelViewModelFromCreateCohortWithDraftApprenticeshipRequestMapper(ISelectDeliveryModelMapperHelper helper)
        {
            _helper = helper;
        }

        public Task<SelectDeliveryModelViewModel> Map(CreateCohortWithDraftApprenticeshipRequest source)
        {
            return _helper.Map(source.ProviderId, source.CourseCode, source.AccountLegalEntityId, source.DeliveryModel);
        }
    }
}