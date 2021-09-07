using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class EditApprenticeshipRequestToViewModelMapper : IMapper<EditApprenticeshipRequest, EditApprenticeshipRequestViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IAcademicYearDateProvider _academicYearDateProvider;
        private readonly ICurrentDateTime _currentDateTime;

        public EditApprenticeshipRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IAcademicYearDateProvider academicYearDateProvider, ICurrentDateTime currentDateTime)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _academicYearDateProvider = academicYearDateProvider;
            _currentDateTime = currentDateTime;
        }
        public async Task<EditApprenticeshipRequestViewModel> Map(EditApprenticeshipRequest source)
        {
            var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(source.ApprenticeshipId, CancellationToken.None);
            var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(source.ApprenticeshipId, CancellationToken.None);

            var apprenticeship = await apprenticeshipTask;
            var commitmentTask = _commitmentsApiClient.GetCohort(apprenticeship.CohortId);
            var courseDetailsTask = _commitmentsApiClient.GetTrainingProgramme(apprenticeship.CourseCode);
            var accountDetailsTask = _commitmentsApiClient.GetAccount(apprenticeship.EmployerAccountId);

            var accountDetails = await accountDetailsTask;
            var priceEpisodes = await priceEpisodesTask;
            var commitment = await commitmentTask;
            var courseDetails = await courseDetailsTask;

            var courses = accountDetails.LevyStatus == ApprenticeshipEmployerType.NonLevy || commitment.IsFundedByTransfer
                ? (await _commitmentsApiClient.GetAllTrainingProgrammeStandards(CancellationToken.None)).TrainingProgrammes
                : (await _commitmentsApiClient.GetAllTrainingProgrammes(CancellationToken.None)).TrainingProgrammes;

            var isLockedForUpdate = IsLiveAndHasHadDataLockSuccess(apprenticeship)
                                    ||
                                    IsLiveAndIsNotWithInFundingPeriod(apprenticeship)
                                    ||
                                    IsWaitingToStartAndHasHadDataLockSuccessAndIsFundedByTransfer(apprenticeship, commitment);

            
            var result = new EditApprenticeshipRequestViewModel(apprenticeship.DateOfBirth, apprenticeship.StartDate, apprenticeship.EndDate)
            {
                FirstName = apprenticeship.FirstName,
                LastName = apprenticeship.LastName,
                Email = apprenticeship.Email,
                ULN = apprenticeship.Uln,
                CourseCode = apprenticeship.CourseCode,
                Cost = priceEpisodes.PriceEpisodes.GetPrice(),
                ProviderReference = apprenticeship.ProviderReference,
                Courses = courses,
                IsContinuation = apprenticeship.IsContinuation,
                IsLockedForUpdate = isLockedForUpdate,
                IsUpdateLockedForStartDateAndCourse = commitment.IsFundedByTransfer && !apprenticeship.HasHadDataLockSuccess,
                IsEndDateLockedForUpdate = IsEndDateLocked(isLockedForUpdate, apprenticeship.HasHadDataLockSuccess, apprenticeship.Status),
                TrainingName = courseDetails.TrainingProgramme.Name,
                ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                EmployerName = apprenticeship.EmployerName,
                ProviderId = apprenticeship.ProviderId,
                EmailAddressConfirmedByApprentice = apprenticeship.EmailAddressConfirmedByApprentice
            };

            return result;
        }

        private bool IsWaitingToStartAndHasHadDataLockSuccessAndIsFundedByTransfer(GetApprenticeshipResponse apprenticeship, GetCohortResponse commitment)
        {
            return commitment.IsFundedByTransfer
                    && HasHadDataLockSuccess(apprenticeship)
                    && IsWaitingToStart(apprenticeship);
        }

        private bool IsLiveAndHasHadDataLockSuccess(GetApprenticeshipResponse apprenticeship)
        {
            return IsLive(apprenticeship) && HasHadDataLockSuccess(apprenticeship);
        }

        private bool IsLiveAndIsNotWithInFundingPeriod(GetApprenticeshipResponse apprenticeship)
        {
            return IsLive(apprenticeship) && !IsWithInFundingPeriod(apprenticeship.StartDate);
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
