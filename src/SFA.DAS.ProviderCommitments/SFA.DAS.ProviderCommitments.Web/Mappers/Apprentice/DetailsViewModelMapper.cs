using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IOuterApiClient _apiClient;
        private readonly ILogger<DetailsViewModelMapper> _logger;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService, IOuterApiClient apiClient,
             ILogger<DetailsViewModelMapper> logger)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _apiClient = apiClient;
            _logger = logger;            
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            try
            {                
                var data = await GetApprenticeshipData(source.ApprenticeshipId, source.ProviderId);
                var hasProviderUpdates = data.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider);
                var hasEmployerUpdates = data.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Employer);

                var dataLockSummaryStatus = data.DataLocks.GetDataLockSummaryStatus();

                var allowEditApprentice = 
                    (data.Apprenticeship.Status == ApprenticeshipStatus.Live ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.WaitingToStart ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.Paused) &&
                    !hasProviderUpdates && 
                    !hasEmployerUpdates &&
                    dataLockSummaryStatus == DetailsViewModel.DataLockSummaryStatus.None &&
                    !data.Apprenticeship.IsOnFlexiPaymentPilot.GetValueOrDefault();

                // If It's completed or stopped and option is null, dont show options as it could predate standard versioning
                // even if the version has options
                var apprenticeshipStopped = data.Apprenticeship.Status == ApprenticeshipStatus.Completed || data.Apprenticeship.Status == ApprenticeshipStatus.Stopped;
                var preDateStandardVersioning = apprenticeshipStopped && data.Apprenticeship.Option == null;
                (var singleOption, var hasOptions) = await HasOptions(data.Apprenticeship.StandardUId);
                var showOptions = hasOptions && !preDateStandardVersioning;

                var pendingChangeOfPartyRequest = data.ChangeOfPartyRequests.SingleOrDefault(x =>
                    x.OriginatingParty == Party.Provider && x.Status == ChangeOfPartyRequestStatus.Pending);

                return new DetailsViewModel
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    Email = data.Apprenticeship.Email,
                    Employer = data.Apprenticeship.EmployerName,
                    Reference = _encodingService.Encode(data.Apprenticeship.CohortId, EncodingType.CohortReference),
                    Status = data.Apprenticeship.Status,
                    ConfirmationStatus = data.Apprenticeship.ConfirmationStatus,
                    StopDate = data.Apprenticeship.StopDate,
                    AgreementId = _encodingService.Encode(data.Apprenticeship.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                    DateOfBirth = data.Apprenticeship.DateOfBirth,
                    Uln = data.Apprenticeship.Uln,
                    CourseName = data.Apprenticeship.CourseName,
                    Version = data.Apprenticeship.Version,
                    Option = data.Apprenticeship.Option,
                    StartDate = data.Apprenticeship.StartDate,
                    ActualStartDate = data.Apprenticeship.ActualStartDate,
                    EndDate = data.Apprenticeship.EndDate,
                    ProviderRef = data.Apprenticeship.ProviderReference,
                    Cost = data.PriceEpisodes.GetPrice(),
                    AllowEditApprentice = allowEditApprentice,
                    HasProviderPendingUpdate = hasProviderUpdates,
                    HasEmployerPendingUpdate = hasEmployerUpdates,
                    DataLockStatus = dataLockSummaryStatus,
                    AvailableTriageOption = CalcTriageStatus(data.Apprenticeship.HasHadDataLockSuccess, data.DataLocks),
                    PauseDate = data.Apprenticeship.PauseDate,
                    CompletionDate = data.Apprenticeship.CompletionDate,
                    HasPendingChangeOfPartyRequest = pendingChangeOfPartyRequest != null,
                    PendingChangeOfPartyRequestWithParty = pendingChangeOfPartyRequest?.WithParty,
                    HasContinuation = data.Apprenticeship.HasContinuation,
                    ShowChangeVersionLink = await HasNewerVersions(data.Apprenticeship),
                    HasOptions = showOptions,
                    SingleOption = singleOption,
                    EmployerHistory = data.ChangeOfEmployerChain?
                        .Select(coe => new EmployerHistory
                        {
                            EmployerName = coe.EmployerName,
                            FromDate = coe.StartDate.Value,
                            ToDate = coe.StopDate.HasValue ? coe.StopDate.Value : coe.EndDate.Value,
                            HashedApprenticeshipId = _encodingService.Encode(coe.ApprenticeshipId, EncodingType.ApprenticeshipId),
                            ShowLink = source.ApprenticeshipId != coe.ApprenticeshipId
                        }).ToList(),
                    EmailShouldBePresent = data.Apprenticeship.EmailShouldBePresent,
                    EmailAddressConfirmedByApprentice = data.Apprenticeship.EmailAddressConfirmedByApprentice,
                    DeliveryModel = data.Apprenticeship.DeliveryModel,
                    EmploymentEndDate = data.Apprenticeship.EmploymentEndDate,
                    EmploymentPrice = data.Apprenticeship.EmploymentPrice,
                    RecognisePriorLearning = data.Apprenticeship.RecognisePriorLearning,
                    DurationReducedByHours = data.Apprenticeship.DurationReducedByHours,
                    DurationReducedBy = data.Apprenticeship.DurationReducedBy,
                    PriceReducedBy = data.Apprenticeship.PriceReducedBy,
                    HasMultipleDeliveryModelOptions = data.HasMultipleDeliveryModelOptions,
                    IsOnFlexiPaymentPilot = data.Apprenticeship.IsOnFlexiPaymentPilot
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error mapping apprenticeship {source.ApprenticeshipId} to DetailsViewModel");
                throw;
            }
        }

        private static DetailsViewModel.TriageOption CalcTriageStatus(bool hasHadDataLockSuccess, IReadOnlyCollection<GetManageApprenticeshipDetailsResponse.DataLock> dataLocks)
        {
            if (!hasHadDataLockSuccess)
            {
                return DetailsViewModel.TriageOption.Update;
            }

            var dataLockErrors = dataLocks.Where(x => x.IsUnresolvedError()).ToList();

            if (dataLockErrors.All(x => x.IsPrice()))
                return  DetailsViewModel.TriageOption.Update;

            if (dataLockErrors.Any(x => x.IsCourseAndPrice()))
                return DetailsViewModel.TriageOption.Restart;

            if (dataLockErrors.All(x => x.IsCourse()))
                return DetailsViewModel.TriageOption.Restart;

            if (dataLockErrors.All(x => x.IsCourseOrPrice()))
                return DetailsViewModel.TriageOption.Both;

            return DetailsViewModel.TriageOption.Update;
        }

        private async Task<GetManageApprenticeshipDetailsResponse> GetApprenticeshipData(long apprenticeshipId, long providerId)
        {
            var apiRequest = new GetManageApprenticeshipDetailsRequest(providerId, apprenticeshipId);
            var apprenticeshipDetails = await _apiClient.Get<GetManageApprenticeshipDetailsResponse>(apiRequest);

            return apprenticeshipDetails;
        }

        private async Task<bool> HasNewerVersions(GetManageApprenticeshipDetailsResponse.ApprenticeshipDetails apprenticeship)
        {
            if (apprenticeship.StandardUId != null)
            {
                var newerVersions = await _commitmentApiClient.GetNewerTrainingProgrammeVersions(apprenticeship.StandardUId);

                if (newerVersions?.NewerVersions != null && newerVersions.NewerVersions.Count() > 0)
                    return true;
            }

            return false;
        }

        private async Task<(bool singleOption, bool hasOptions)> HasOptions(string standardUId)
        {
            if (!string.IsNullOrEmpty(standardUId))
            {
                var trainingProgrammeVersionResponse = await _commitmentApiClient.GetTrainingProgrammeVersionByStandardUId(standardUId);

                var optionsCount = trainingProgrammeVersionResponse?.TrainingProgramme?.Options.Count();
                return (optionsCount == 1, optionsCount > 0);
            }

            return (false,false);
        }
    }
}