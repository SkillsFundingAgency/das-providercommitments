using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship.SelectCourseViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.DraftApprenticeship;

public class AddDraftApprenticeshipCourseViewModelMapper(
    IOuterApiClient apiClient,
    ICacheStorageService cacheStorage)
    : IMapper<ReservationsAddDraftApprenticeshipRequest, SelectCourseViewModel>
{
    public async Task<SelectCourseViewModel> Map(ReservationsAddDraftApprenticeshipRequest source)
    {
        if (source.CacheKey.HasValue && string.IsNullOrWhiteSpace(source.CourseCode))
        {
            var cacheItem = await cacheStorage.RetrieveFromCache<ReservationsAddDraftApprenticeshipRequest>(source.CacheKey.Value);
            source.CourseCode = cacheItem.CourseCode;
        }

        var apiRequest = new GetAddDraftApprenticeshipCourseRequest(source.ProviderId, source.CohortId.Value);
        var apiResponse = await apiClient.Get<GetAddDraftApprenticeshipCourseResponse>(apiRequest);

        var result = new SelectCourseViewModel
        {
            CourseCode = source.CourseCode,
            ProviderId = source.ProviderId,
            ReservationId = source.ReservationId,
            EmployerName = apiResponse.EmployerName,
            ShowManagingStandardsContent = apiResponse.IsMainProvider,
            Standards = apiResponse.Standards.Select(x => new Standard { CourseCode = x.CourseCode, Name = x.Name })
        };

        return result;
    }
}