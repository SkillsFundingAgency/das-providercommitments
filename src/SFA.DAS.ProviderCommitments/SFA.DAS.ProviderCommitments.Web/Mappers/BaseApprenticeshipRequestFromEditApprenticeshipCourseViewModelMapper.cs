using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Services;
using System.Threading.Tasks;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class
        BaseApprenticeshipRequestFromEditApprenticeshipCourseViewModelMapper : IMapper<EditApprenticeshipCourseViewModel
            , BaseApprenticeshipRequest>
    {
        private readonly ITempDataStorageService _tempData;
        private const string ViewModelForEdit = "ViewModelForEdit";

        public BaseApprenticeshipRequestFromEditApprenticeshipCourseViewModelMapper(ITempDataStorageService tempData) =>
            _tempData = tempData;

        public Task<BaseApprenticeshipRequest> Map(EditApprenticeshipCourseViewModel source)
        {
            var data = _tempData.RetrieveFromCache<EditApprenticeshipRequestViewModel>(ViewModelForEdit);
            data.CourseCode = source.CourseCode;
            _tempData.AddToCache(data, ViewModelForEdit);

            var request = new BaseApprenticeshipRequest
            {
                ApprenticeshipHashedId = data.ApprenticeshipHashedId,
                ProviderId = data.ProviderId
            };

            return Task.FromResult(request);
        }
    }
}