using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class DownloadApprenticesRequestMapper(
    IOuterApiService outerApiService,
    ICreateCsvService createCsvService,
    ICurrentDateTime currentDateTime,
    IEncodingService encodingService) : IMapper<DownloadRequest, DownloadViewModel>
{
    public async Task<DownloadViewModel> Map(DownloadRequest request)
    {
        var downloadViewModel = new DownloadViewModel();

        var apiRequestBody = new PostApprenticeshipsCSVRequest.Body
        {
            SearchTerm = request.SearchTerm,
            EmployerName = request.SelectedEmployer,
            CourseName = request.SelectedCourse,
            Status = request.SelectedStatus,
            StartDate = request.SelectedStartDate,
            EndDate = request.SelectedEndDate,
            Alert = request.SelectedAlert,
            ApprenticeConfirmationStatus = request.SelectedApprenticeConfirmation,
            DeliveryModel = request.SelectedDeliveryModel
        };

        var getApprenticeshipsRequest = new PostApprenticeshipsCSVRequest(
            providerId: request.ProviderId,
            apiRequestBody
        );

        var csvModel = new ApprenticeshipDetailsCsvModel();
        var result = await outerApiService.GetApprenticeshipsCSV(getApprenticeshipsRequest);
        var csvContent = result.Apprenticeships.Select(detailsCsvResponse => csvModel.Map(detailsCsvResponse, encodingService)).ToList();

        downloadViewModel.Content = createCsvService.GenerateCsvContent(csvContent, true);
        downloadViewModel.Request = getApprenticeshipsRequest;
        downloadViewModel.Name = $"{"Manageyourapprentices"}_{currentDateTime.UtcNow:yyyyMMddhhmmss}.csv";
            
        return downloadViewModel;
    }
}