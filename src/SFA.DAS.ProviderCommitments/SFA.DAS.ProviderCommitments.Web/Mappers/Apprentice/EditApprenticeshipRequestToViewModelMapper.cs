using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeships;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EditApprenticeshipRequestToViewModelMapper : IMapper<EditApprenticeshipRequest, EditApprenticeshipRequestViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly ICurrentDateTime _currentDateTime;
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _apiClient;

        public EditApprenticeshipRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IAcademicYearDateProvider academicYearDateProvider, ICurrentDateTime currentDateTime, IEncodingService encodingService, IOuterApiClient apiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _academicYearDateProvider = academicYearDateProvider;
            _currentDateTime = currentDateTime;
            _encodingService = encodingService;
            _apiClient = apiClient;
        }

        public async Task<EditApprenticeshipRequestViewModel> Map(EditApprenticeshipRequest source)
        {
            var apiRequest = new GetEditApprenticeshipRequest(source.ProviderId, source.ApprenticeshipId);
            
            var editApprenticeshipTask = _apiClient.Get<GetEditApprenticeshipResponse>(apiRequest);
            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId, CancellationToken.None);
            var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId, CancellationToken.None);

            await Task.WhenAll(editApprenticeshipTask, apprenticeshipTask, priceEpisodesTask);

            var apprenticeship = apprenticeshipTask.Result;
            var editApprenticeship = editApprenticeshipTask.Result;
            var priceEpisodes = priceEpisodesTask.Result;

            var courseDetailsTask = _commitmentsApiClient.GetTrainingProgramme(apprenticeship.CourseCode);
            var accountDetailsTask = _commitmentsApiClient.GetAccount(apprenticeship.EmployerAccountId);

            await Task.WhenAll(courseDetailsTask, accountDetailsTask);

            var courseDetails = courseDetailsTask.Result;
            var accountDetails = accountDetailsTask.Result;

            var courses = accountDetails.LevyStatus == ApprenticeshipEmployerType.NonLevy || editApprenticeship.IsFundedByTransfer
                ? (await _commitmentsApiClient.GetAllTrainingProgrammeStandards(CancellationToken.None)).TrainingProgrammes
                : (await _commitmentsApiClient.GetAllTrainingProgrammes(CancellationToken.None)).TrainingProgrammes;

            var isLockedForUpdate = IsLiveAndHasHadDataLockSuccess(apprenticeship)
                                    ||
                                    IsLiveAndIsNotWithInFundingPeriod(apprenticeship)
                                    ||
                                    IsPausedAndHasHadDataLockSuccess(apprenticeship)
                                    ||
                                    IsPausedAndIsNotWithInFundingPeriod(apprenticeship)
                                    ||
                                    IsPausedAndHasHadDataLockSuccessAndIsFundedByTransfer(apprenticeship, editApprenticeship.IsFundedByTransfer)
                                    ||
                                    IsWaitingToStartAndHasHadDataLockSuccessAndIsFundedByTransfer(apprenticeship, editApprenticeship.IsFundedByTransfer);

            
            var result = new EditApprenticeshipRequestViewModel(apprenticeship.DateOfBirth, apprenticeship.StartDate, apprenticeship.EndDate, apprenticeship.EmploymentEndDate)
            {
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                Email = apprenticeship.Email,
                ULN = apprenticeship.Uln,
                CourseCode = apprenticeship.CourseCode,
                Version = apprenticeship.Version,
                Option = apprenticeship.Option == string.Empty ? "TBC" : apprenticeship.Option,
                Cost = priceEpisodes.PriceEpisodes.GetCost(),
                ProviderReference = apprenticeship.ProviderReference,
                Courses = courses,
                IsContinuation = apprenticeship.IsContinuation,
                IsLockedForUpdate = isLockedForUpdate,
                IsUpdateLockedForStartDateAndCourse = editApprenticeship.IsFundedByTransfer && !apprenticeship.HasHadDataLockSuccess,
                IsEndDateLockedForUpdate = IsEndDateLocked(isLockedForUpdate, apprenticeship.HasHadDataLockSuccess, apprenticeship.Status),
                TrainingName = courseDetails.TrainingProgramme.Name,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerName = apprenticeship.EmployerName,
                ProviderId = apprenticeship.ProviderId,
                EmailAddressConfirmedByApprentice = apprenticeship.EmailAddressConfirmedByApprentice,
                EmailShouldBePresent = apprenticeship.EmailShouldBePresent,
                DeliveryModel = apprenticeship.DeliveryModel,
                EmploymentPrice = apprenticeship.EmploymentPrice,
                EmployerAccountLegalEntityPublicHashedId = _encodingService.Encode(apprenticeship.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                HasMultipleDeliveryModelOptions = editApprenticeship.HasMultipleDeliveryModelOptions,
                CourseName = editApprenticeship.CourseName
            };

            return result;
        }

        private bool IsPausedAndHasHadDataLockSuccessAndIsFundedByTransfer(GetApprenticeshipResponse apprenticeship, bool isFundedByTransfer)
        {
            if (CheckWaitingToStart(apprenticeship)) return isFundedByTransfer && HasHadDataLockSuccess(apprenticeship) && IsPaused(apprenticeship); else return false;
        }

        private bool IsPausedAndHasHadDataLockSuccess(GetApprenticeshipResponse apprenticeship)
        {
            if (!CheckWaitingToStart(apprenticeship)) return IsPaused(apprenticeship) && HasHadDataLockSuccess(apprenticeship); else return false;
        }

        private bool IsPausedAndIsNotWithInFundingPeriod(GetApprenticeshipResponse apprenticeship)
        {
            if (!CheckWaitingToStart(apprenticeship)) return IsPaused(apprenticeship) && !IsWithInFundingPeriod(apprenticeship.StartDate.Value); else return false;
        }

        private bool IsPaused(GetApprenticeshipResponse apprenticeship)
        {
            return apprenticeship.Status == ApprenticeshipStatus.Paused;
        }

        private bool CheckWaitingToStart(GetApprenticeshipResponse apprenticeship)
        {
            return apprenticeship.StartDate.Value > new DateTime(_currentDateTime.UtcNow.Year, _currentDateTime.UtcNow.Month, 1);
        }

        private bool IsWaitingToStartAndHasHadDataLockSuccessAndIsFundedByTransfer(GetApprenticeshipResponse apprenticeship, bool isFundedByTransfer)
        {
            return isFundedByTransfer
                    && HasHadDataLockSuccess(apprenticeship)
                    && IsWaitingToStart(apprenticeship);
        }

        private bool IsLiveAndHasHadDataLockSuccess(GetApprenticeshipResponse apprenticeship)
        {
            return IsLive(apprenticeship) && HasHadDataLockSuccess(apprenticeship);
        }

        private bool IsLiveAndIsNotWithInFundingPeriod(GetApprenticeshipResponse apprenticeship)
        {
            return IsLive(apprenticeship) && !IsWithInFundingPeriod(apprenticeship.StartDate.Value);
        }

        private bool IsWaitingToStart(GetApprenticeshipResponse apprenticeship)
        {
            return apprenticeship.Status == ApprenticeshipStatus.WaitingToStart;
        }

        private bool IsLive(GetApprenticeshipResponse apprenticeship)
        {
            return apprenticeship.Status == ApprenticeshipStatus.Live;
        }

        private bool HasHadDataLockSuccess(GetApprenticeshipResponse apprenticeship)
        {
            return apprenticeship.HasHadDataLockSuccess;
        }

        private bool IsEndDateLocked(bool isLockedForUpdate, bool hasHadDataLockSuccess, ApprenticeshipStatus status)
        {
            var result = isLockedForUpdate;
            if (hasHadDataLockSuccess)
            {
                result = status == ApprenticeshipStatus.WaitingToStart;
            }

            return result;
        }

        private bool IsWithInFundingPeriod(DateTime trainingStartDate)
        {
            if (trainingStartDate < _academicYearDateProvider.CurrentAcademicYearStartDate &&
                 _currentDateTime.UtcNow > _academicYearDateProvider.LastAcademicYearFundingPeriod)
            {
                return false;
            }

            return true;
        }
    }
}
