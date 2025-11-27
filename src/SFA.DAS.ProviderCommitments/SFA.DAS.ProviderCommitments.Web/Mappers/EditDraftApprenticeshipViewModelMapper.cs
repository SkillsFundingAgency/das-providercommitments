using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;

namespace SFA.DAS.ProviderCommitments.Web.Mappers;

public class EditDraftApprenticeshipViewModelMapper(
    IEncodingService encodingService,
    IOuterApiClient outerApiClient,
    ITempDataStorageService storageService,
    ICacheStorageService cacheStorageService)
    : IMapper<EditDraftApprenticeshipRequest, IDraftApprenticeshipViewModel>
{
    public async Task<IDraftApprenticeshipViewModel> Map(EditDraftApprenticeshipRequest source)
    {
        try
        {
            var cachedModel = storageService.RetrieveFromCache<EditDraftApprenticeshipViewModel>();

            var apiRequest = new GetEditDraftApprenticeshipRequest(source.Request.ProviderId, source.Request.CohortId, source.Request.DraftApprenticeshipId, cachedModel?.CourseCode);
            var apiResponse = await outerApiClient.Get<GetEditDraftApprenticeshipResponse>(apiRequest);

            var newModel = new EditDraftApprenticeshipViewModel(apiResponse.DateOfBirth, apiResponse.StartDate, apiResponse.ActualStartDate, apiResponse.EndDate, apiResponse.EmploymentEndDate)
            {
                AccountLegalEntityId = source.Cohort.AccountLegalEntityId,
                EmployerAccountLegalEntityPublicHashedId = encodingService.Encode(source.Cohort.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                DraftApprenticeshipId = source.Request.DraftApprenticeshipId,
                DraftApprenticeshipHashedId = source.Request.DraftApprenticeshipHashedId,
                CohortId = source.Request.CohortId,
                CohortReference = source.Request.CohortReference,
                ProviderId = source.Request.ProviderId,
                ReservationId = apiResponse.ReservationId,
                FirstName = apiResponse.FirstName,
                LastName = apiResponse.LastName,
                Email = apiResponse.Email,
                Uln = apiResponse.Uln,
                CourseCode = apiResponse.CourseCode,
                HasStandardOptions = apiResponse.HasStandardOptions,
                Cost = apiResponse.Cost,
                TrainingPrice = apiResponse.TrainingPrice,
                EndPointAssessmentPrice = apiResponse.EndPointAssessmentPrice,
                Reference = apiResponse.ProviderReference,
                IsContinuation = apiResponse.IsContinuation,
                TrainingCourseOption = apiResponse.TrainingCourseOption == string.Empty ? "-1" : apiResponse.TrainingCourseOption,
                DeliveryModel = (DeliveryModel?) apiResponse.DeliveryModel,
                EmploymentPrice = apiResponse.EmploymentPrice,
                RecognisePriorLearning = apiResponse.RecognisePriorLearning,
                DurationReducedBy = apiResponse.DurationReducedBy,
                PriceReducedBy = apiResponse.PriceReducedBy,
                RecognisingPriorLearningStillNeedsToBeConsidered = apiResponse.RecognisingPriorLearningStillNeedsToBeConsidered,
                RecognisingPriorLearningExtendedStillNeedsToBeConsidered = apiResponse.RecognisingPriorLearningExtendedStillNeedsToBeConsidered,
                HasMultipleDeliveryModelOptions = apiResponse.HasMultipleDeliveryModelOptions,
                EmployerHasEditedCost = apiResponse.EmployerHasEditedCost,
                HasUnavailableFlexiJobAgencyDeliveryModel = apiResponse.HasUnavailableDeliveryModel && apiResponse.DeliveryModel == Infrastructure.OuterApi.Types.DeliveryModel.FlexiJobAgency,
                EmailAddressConfirmed = apiResponse.EmailAddressConfirmed,
                LearnerDataId = apiResponse.LearnerDataId,
                HasLearnerDataChanges = apiResponse.HasLearnerDataChanges,
                LastLearnerDataSync = apiResponse.LastLearnerDataSync,
                TrainingTotalHours = apiResponse.TrainingTotalHours,
                DurationReducedByHours = apiResponse.DurationReducedByHours,
                IsDurationReducedByRpl = apiResponse.IsDurationReducedByRpl,
                RplUpdated = source.Request.RplUpdated
            };

            if (cachedModel != null)
            {
                cachedModel.HasMultipleDeliveryModelOptions = newModel.HasMultipleDeliveryModelOptions;
                cachedModel.HasChangedDeliveryModel = cachedModel.DeliveryModel != (DeliveryModel?)apiResponse.DeliveryModel;
                
                if (!string.IsNullOrEmpty(source.LearnerDataSyncKey))
                {
                    await ApplyLearnerDataSyncUpdates(cachedModel, source.LearnerDataSyncKey);
                }
                
                return cachedModel;
            }
            
            if (!string.IsNullOrEmpty(source.LearnerDataSyncKey))
            {
                await ApplyLearnerDataSyncUpdates(newModel, source.LearnerDataSyncKey);
            }

            return newModel;
        }
        catch (RestHttpClientException restEx)
        {
            if (restEx.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new DraftApprenticeshipNotFoundException(
                    $"DraftApprenticeship Id: {source.Request.DraftApprenticeshipId} not found", restEx);
            }
            throw;
        }
    }

    public async Task ApplyLearnerDataSyncUpdates(EditDraftApprenticeshipViewModel editModel, string learnerDataSyncKey)
    {
        if (string.IsNullOrEmpty(learnerDataSyncKey)) return;
            
        var updatedDraftApprenticeship = await cacheStorageService.SafeRetrieveFromCache<GetDraftApprenticeshipResponse>(learnerDataSyncKey);
        if (updatedDraftApprenticeship == null) return;

        editModel.FirstName = updatedDraftApprenticeship.FirstName;
        editModel.LastName = updatedDraftApprenticeship.LastName;
            
        if (updatedDraftApprenticeship.DateOfBirth.HasValue)
        {
            editModel.BirthDay = updatedDraftApprenticeship.DateOfBirth.Value.Day;
            editModel.BirthMonth = updatedDraftApprenticeship.DateOfBirth.Value.Month;
            editModel.BirthYear = updatedDraftApprenticeship.DateOfBirth.Value.Year;
        }
            
        if (updatedDraftApprenticeship.StartDate.HasValue)
        {
            editModel.StartMonth = updatedDraftApprenticeship.StartDate.Value.Month;
            editModel.StartYear = updatedDraftApprenticeship.StartDate.Value.Year;
        }
            
        if (updatedDraftApprenticeship.EndDate.HasValue)
        {
            editModel.EndDay = updatedDraftApprenticeship.EndDate.Value.Day;
            editModel.EndMonth = updatedDraftApprenticeship.EndDate.Value.Month;
            editModel.EndYear = updatedDraftApprenticeship.EndDate.Value.Year;
        }
            
        editModel.Cost = updatedDraftApprenticeship.Cost;
        editModel.HasLearnerDataChanges = false;
        editModel.LastLearnerDataSync = DateTime.UtcNow;
            
        await cacheStorageService.DeleteFromCache(learnerDataSyncKey);
    }
}