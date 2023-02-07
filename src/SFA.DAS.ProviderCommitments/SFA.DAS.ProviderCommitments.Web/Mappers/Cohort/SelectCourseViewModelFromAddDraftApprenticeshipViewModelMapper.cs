using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectCourseViewModelFromAddDraftApprenticeshipViewModelMapper : IMapper<AddDraftApprenticeshipViewModel, SelectCourseViewModel>
    {
        private readonly ISelectCourseViewModelMapperHelper _selectCourseViewModelHelper;

        public SelectCourseViewModelFromAddDraftApprenticeshipViewModelMapper(ISelectCourseViewModelMapperHelper selectCourseViewModelHelper)
        {
            _selectCourseViewModelHelper = selectCourseViewModelHelper;
        }

        public Task<SelectCourseViewModel> Map(AddDraftApprenticeshipViewModel source)
        {
            return _selectCourseViewModelHelper.Map(source.CourseCode, source.AccountLegalEntityId, source.IsOnFlexiPaymentPilot);
        }
    }
}