using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<DetailsViewModelMapper> _logger;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentApiClient, IEncodingService encodingService, ILogger<DetailsViewModelMapper> logger)
        {
            _commitmentApiClient = commitmentApiClient;
            _encodingService = encodingService;
            _logger = logger;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            try
            {
                var data = await GetApprenticeshipData(source.ApprenticeshipId);
                var dataLockSummaryStatus = data.DataLocks.DataLocks.GetDataLockSummaryStatus();
                
                var allowEditApprentice =
                    (data.Apprenticeship.Status == ApprenticeshipStatus.Live ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.WaitingToStart ||
                     data.Apprenticeship.Status == ApprenticeshipStatus.Paused) &&
                    !data.HasProviderUpdates && 
                    !data.HasEmployerUpdates &&
                    dataLockSummaryStatus == DetailsViewModel.DataLockSummaryStatus.None;

                return new DetailsViewModel
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeName = $"{data.Apprenticeship.FirstName} {data.Apprenticeship.LastName}",
                    Employer = data.Apprenticeship.EmployerName,
                    Reference = _encodingService.Encode(data.Apprenticeship.CohortId, EncodingType.CohortReference),
                    Status = data.Apprenticeship.Status,
                    StopDate = data.Apprenticeship.StopDate,
                    AgreementId = _encodingService.Encode(data.Apprenticeship.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                    DateOfBirth = data.Apprenticeship.DateOfBirth,
                    Uln = data.Apprenticeship.Uln,
                    CourseName = data.Apprenticeship.CourseName,
                    StartDate = data.Apprenticeship.StartDate,
                    EndDate = data.Apprenticeship.EndDate,
                    ProviderRef = data.Apprenticeship.Reference,
                    Cost = data.PriceEpisodes.PriceEpisodes.GetPrice(),
                    AllowEditApprentice = allowEditApprentice,
                    HasProviderPendingUpdate = data.HasProviderUpdates,
                    HasEmployerPendingUpdate = data.HasEmployerUpdates,
                    DataLockStatus = dataLockSummaryStatus,
                    AvailableTriageOption = CalcTriageStatus(data.Apprenticeship.HasHadDataLockSuccess, data.DataLocks.DataLocks)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error mapping apprenticeship {source.ApprenticeshipId} to DetailsViewModel");
                throw;
            }
        }

        private static DetailsViewModel.TriageOption CalcTriageStatus(bool hasHadDataLockSuccess, IReadOnlyCollection<GetDataLocksResponse.DataLock> dataLocks)
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

        private async Task<(GetApprenticeshipResponse Apprenticeship, 
            GetPriceEpisodesResponse PriceEpisodes, 
            bool HasProviderUpdates, 
            bool HasEmployerUpdates,
            GetDataLocksResponse DataLocks)> 
            GetApprenticeshipData(long apprenticeshipId)
        {
            var detailsResponseTask = _commitmentApiClient.GetApprenticeship(apprenticeshipId);
            var priceEpisodesTask = _commitmentApiClient.GetPriceEpisodes(apprenticeshipId);
            var pendingUpdatesTask = _commitmentApiClient.GetApprenticeshipUpdates(apprenticeshipId,
                new CommitmentsV2.Api.Types.Requests.GetApprenticeshipUpdatesRequest
                    { Status = ApprenticeshipUpdateStatus.Pending });
            var dataLocksTask = _commitmentApiClient.GetApprenticeshipDatalocksStatus(apprenticeshipId);

            await Task.WhenAll(detailsResponseTask, priceEpisodesTask, pendingUpdatesTask, dataLocksTask);

            var detailsResponse = await detailsResponseTask;
            var priceEpisodes = await priceEpisodesTask;
            var pendingUpdates = await pendingUpdatesTask;
            var dataLocks = await dataLocksTask;

            return (detailsResponse, 
                priceEpisodes, 
                pendingUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider),
                pendingUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Employer),
                dataLocks);
        }
    }
}