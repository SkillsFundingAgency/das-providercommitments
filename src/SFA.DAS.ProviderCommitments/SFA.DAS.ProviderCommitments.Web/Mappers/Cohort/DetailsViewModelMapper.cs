using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Services;


namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly IOuterApiClient _outerApiClient;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;
        private readonly IPasAccountApiClient _pasAccountsApiClient;
        private readonly ITempDataStorageService _storageService;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService,
            IPasAccountApiClient pasAccountApiClient, IOuterApiClient outerApiClient, ITempDataStorageService storageService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
            _pasAccountsApiClient = pasAccountApiClient;
            _outerApiClient = outerApiClient;
            _storageService = storageService;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            //clear leftover tempdata from add/edit
            //this solution should NOT use tempdata in this way
            _storageService.RemoveFromCache<EditDraftApprenticeshipViewModel>();

            var cohortDetailsTask = _outerApiClient.Get<GetCohortDetailsResponse>(new GetCohortDetailsRequest(source.ProviderId, source.CohortId));
            var cohortTask = _commitmentsApiClient.GetCohort(source.CohortId);
            var draftApprenticeshipsTask = _commitmentsApiClient.GetDraftApprenticeships(source.CohortId);
            var agreementStatusTask = _pasAccountsApiClient.GetAgreement(source.ProviderId);
            var emailOverlapsTask = _commitmentsApiClient.GetEmailOverlapChecks(source.CohortId);

            await Task.WhenAll(cohortDetailsTask, cohortTask, draftApprenticeshipsTask, agreementStatusTask, emailOverlapsTask);
           
            var cohort = cohortTask.Result;
            var cohortDetails = cohortDetailsTask.Result;
            var draftApprenticeships = (await draftApprenticeshipsTask).DraftApprenticeships;
            var agreementStatus = agreementStatusTask.Result;
            var emailOverlaps = (await emailOverlapsTask).ApprenticeshipEmailOverlaps.ToList();

            var courses = await GroupCourses(draftApprenticeships, emailOverlaps, cohort);
            var viewOrApprove = cohort.WithParty == CommitmentsV2.Types.Party.Provider ? "Approve" : "View";
            var isAgreementSigned = agreementStatus.Status == PAS.Account.Api.Types.ProviderAgreementStatus.Agreed;

            return new DetailsViewModel
            {
                ProviderId = source.ProviderId,
                CohortReference = source.CohortReference,
                WithParty = cohort.WithParty,
                AccountLegalEntityHashedId = _encodingService.Encode(cohort.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                LegalEntityName = cohortDetails.LegalEntityName,
                ProviderName = cohortDetails.ProviderName,
                TransferSenderHashedId = cohort.TransferSenderId == null ? null : _encodingService.Encode(cohort.TransferSenderId.Value, EncodingType.PublicAccountId),
                EncodedPledgeApplicationId = cohort.PledgeApplicationId == null ? null : _encodingService.Encode(cohort.PledgeApplicationId.Value, EncodingType.PledgeApplicationId),
                Message = cohort.LatestMessageCreatedByEmployer,
                Courses = courses,
                PageTitle = draftApprenticeships.Count > 1
                    ? $"{viewOrApprove} {draftApprenticeships.Count} apprentices' details"
                    : $"{viewOrApprove} apprentice details",
                IsApprovedByEmployer = cohort.IsApprovedByEmployer,
                IsAgreementSigned = isAgreementSigned,
                IsCompleteForProvider = cohort.IsCompleteForProvider,
                HasEmailOverlaps = emailOverlaps.Any(),
                ShowAddAnotherApprenticeOption = !cohort.IsLinkedToChangeOfPartyRequest,
                AllowBulkUpload = cohort.LevyStatus == CommitmentsV2.Types.ApprenticeshipEmployerType.Levy 
                && cohort.WithParty == CommitmentsV2.Types.Party.Provider 
                && !cohort.IsLinkedToChangeOfPartyRequest,
                IsLinkedToChangeOfPartyRequest = cohort.IsLinkedToChangeOfPartyRequest,
                Status = GetCohortStatus(cohort, draftApprenticeships),
                ShowRofjaaRemovalBanner = cohortDetails.HasUnavailableFlexiJobAgencyDeliveryModel
            };
        }

        private string GetCohortStatus(GetCohortResponse cohort, IReadOnlyCollection<DraftApprenticeshipDto> draftApprenticeships)
        {
            if (cohort.TransferSenderId.HasValue &&
                cohort.TransferApprovalStatus == CommitmentsV2.Types.TransferApprovalStatus.Pending)
            {
                if (cohort.WithParty == CommitmentsV2.Types.Party.TransferSender)
                {
                    return "Pending - with funding employer";
                }
                else if (cohort.WithParty == CommitmentsV2.Types.Party.Employer)
                {
                    return GetEmployerOnlyStatus(cohort);
                }
                else if (cohort.WithParty == CommitmentsV2.Types.Party.Provider)
                {
                    return GetProviderOnlyStatus(cohort);
                }
            }
            else if (cohort.IsApprovedByEmployer && cohort.IsApprovedByProvider)
            {
                return "Approved";
            }
            else if (cohort.WithParty == CommitmentsV2.Types.Party.Provider)
            {
                return GetProviderOnlyStatus(cohort);
            }
            else if (cohort.WithParty == CommitmentsV2.Types.Party.Employer)
            {
                return GetEmployerOnlyStatus(cohort);
            }

            return "New request";
        }

        private static string GetProviderOnlyStatus(GetCohortResponse cohort)
        {
            if (cohort.LastAction == CommitmentsV2.Types.LastAction.None)
            {
                return "New request";
            }
            else if (cohort.LastAction == CommitmentsV2.Types.LastAction.Amend)
            {
                return "Ready for review";
            }
            else if (cohort.LastAction == CommitmentsV2.Types.LastAction.Approve)
            {
                if (!cohort.IsApprovedByProvider && !cohort.IsApprovedByEmployer)
                    return "Ready for review";

                return "Ready for approval";
            }
            else
            {
                return "New request";
            }
        }

        private static string GetEmployerOnlyStatus(GetCohortResponse cohort)
        {
            if (cohort.LastAction == CommitmentsV2.Types.LastAction.None)
            {
                return "New request";
            }
            else if (cohort.LastAction == CommitmentsV2.Types.LastAction.Amend)
            {
                return "Under review with employer";
            }
            else if (cohort.LastAction == CommitmentsV2.Types.LastAction.Approve)
            {
                return "With Employer for approval";
            }
            else
            {
                return "Under review with employer";
            }
        }

        private async Task<IReadOnlyCollection<DetailsViewCourseGroupingModel>> GroupCourses(IEnumerable<DraftApprenticeshipDto> draftApprenticeships, List<ApprenticeshipEmailOverlap> emailOverlaps, GetCohortResponse cohortResponse)
        {
            var groupedByCourse = draftApprenticeships
                .GroupBy(a => new { a.CourseCode, a.CourseName, a.DeliveryModel })
                .Select(course => new DetailsViewCourseGroupingModel
                {
                    CourseCode = course.Key.CourseCode,
                    CourseName = course.Key.CourseName,
                    DeliveryModel = course.Key.DeliveryModel,
                    DraftApprenticeships = course
                        // Sort before on raw properties rather than use displayName property post select for performance reasons
                        .OrderBy(a => a.FirstName)
                        .ThenBy(a => a.LastName)
                        .Select(a => new CohortDraftApprenticeshipViewModel
                        {
                            Id = a.Id,
                            DraftApprenticeshipHashedId = _encodingService.Encode(a.Id, EncodingType.ApprenticeshipId),
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            Cost = a.Cost,
                            DateOfBirth = a.DateOfBirth,
                            EndDate = a.EndDate,
                            StartDate = a.StartDate,
                            ActualStartDate = a.ActualStartDate,
                            OriginalStartDate = a.OriginalStartDate,
                            ULN = a.Uln,
                            HasOverlappingEmail = emailOverlaps.Any(x => x.Id == a.Id),
                            IsComplete = IsDraftApprenticeshipComplete(a, cohortResponse),
                            EmploymentPrice = a.EmploymentPrice,
                            EmploymentEndDate = a.EmploymentEndDate,
                            IsOnFlexiPaymentPilot = a.IsOnFlexiPaymentPilot
                        })
                .ToList()
                })
            .OrderBy(c => c.CourseName)
                .ToList();

            PopulateFundingBandExcessModels(groupedByCourse);
            PopulateEmailOverlapsModel(groupedByCourse);
            await CheckUlnOverlap(groupedByCourse);
            await CheckForPendingOverlappingTrainingDateRequest(groupedByCourse);

            return groupedByCourse;
        }

        private bool IsDraftApprenticeshipComplete(DraftApprenticeshipDto draftApprenticeship, GetCohortResponse cohortResponse)
        {
            if(string.IsNullOrWhiteSpace(draftApprenticeship.FirstName)
                || string.IsNullOrWhiteSpace(draftApprenticeship.LastName)
                || string.IsNullOrWhiteSpace(draftApprenticeship.CourseName)
                || string.IsNullOrWhiteSpace(draftApprenticeship.Uln))
            {
                return false;
            }

            if (draftApprenticeship.DateOfBirth == null
                || draftApprenticeship.Uln == null
                || (draftApprenticeship.ActualStartDate == null && draftApprenticeship.StartDate == null)
                || draftApprenticeship.EndDate == null
                || draftApprenticeship.Cost == null)
            {
                return false;
            }

            if (cohortResponse.ApprenticeEmailIsRequired
                && string.IsNullOrWhiteSpace(draftApprenticeship.Email)
                && !cohortResponse.IsLinkedToChangeOfPartyRequest)
            {
                return false;
            }

            if (draftApprenticeship.DeliveryModel == DeliveryModel.PortableFlexiJob
                && (draftApprenticeship.EmploymentPrice == null
                || draftApprenticeship.EmploymentEndDate == null))
            {
                return false;
            }

            if (draftApprenticeship.RecognisingPriorLearningStillNeedsToBeConsidered)
            {
                return false;
            }

            return true;
        }

        private Task CheckUlnOverlap(List<DetailsViewCourseGroupingModel> courseGroups)
        {
            var results = courseGroups.Select(courseGroup => SetUlnOverlap(courseGroup.DraftApprenticeships));
            return Task.WhenAll(results);
        }

        private Task CheckForPendingOverlappingTrainingDateRequest(List<DetailsViewCourseGroupingModel> courseGroups)
        {
            var results = courseGroups.Select(courseGroup => SetOverlappingTrainingDateRequest(courseGroup.DraftApprenticeships));
            return Task.WhenAll(results);
        }

        private async Task SetOverlappingTrainingDateRequest(IReadOnlyCollection<CohortDraftApprenticeshipViewModel> draftApprenticeships)
        {
            List<Task<GetOverlapRequestQueryResult>> overlapRequestQueryResultsTasks = new List<Task<GetOverlapRequestQueryResult>>();
            foreach (var draftApprenticeship in draftApprenticeships) 
            {
                if (!string.IsNullOrWhiteSpace(draftApprenticeship.ULN) && draftApprenticeship.StartDate.HasValue && draftApprenticeship.EndDate.HasValue)
                {
                    var result = _outerApiClient.Get<GetOverlapRequestQueryResult>(new GetOverlapRequestQueryRequest(draftApprenticeship.Id));
                    overlapRequestQueryResultsTasks.Add(result);
                }
            }

            await Task.WhenAll(overlapRequestQueryResultsTasks);
            foreach (var task in overlapRequestQueryResultsTasks)
            {
                 var result = task.Result;

                if (result != null && result.DraftApprenticeshipId.HasValue)
                {
                    var draftApprenticeship = draftApprenticeships.First(x => x.Id == result.DraftApprenticeshipId);
                    draftApprenticeship.OverlappingTrainingDateRequest = new CohortDraftApprenticeshipViewModel.OverlappingTrainingDateRequestViewModel
                    {
                        CreatedOn = result.CreatedOn
                    };
                }
            }
        }

        private void PopulateFundingBandExcessModels(List<DetailsViewCourseGroupingModel> courseGroups)
        {
            var results = courseGroups.Select(courseGroup => SetFundingBandCap(courseGroup.CourseCode, courseGroup.DraftApprenticeships)).ToList();
            Task.WhenAll(results).Wait();

            foreach (var courseGroup in courseGroups)
            {
                var apprenticesExceedingFundingBand = courseGroup.DraftApprenticeships.Where(x => x.ExceedsFundingBandCap).ToList();
                int numberExceedingBand = apprenticesExceedingFundingBand.Count;

                if (numberExceedingBand > 0)
                {
                    var fundingExceededValues = apprenticesExceedingFundingBand.GroupBy(x => x.FundingBandCap).Select(fundingBand => fundingBand.Key);
                    var fundingBandCapExcessHeader = GetFundingBandExcessHeader(apprenticesExceedingFundingBand.Count);
                    var fundingBandCapExcessLabel = GetFundingBandExcessLabel(apprenticesExceedingFundingBand.Count);

                    courseGroup.FundingBandExcess =
                        new FundingBandExcessModel(apprenticesExceedingFundingBand.Count, fundingExceededValues, fundingBandCapExcessHeader, fundingBandCapExcessLabel);
                }
            }
        }

        private void PopulateEmailOverlapsModel(List<DetailsViewCourseGroupingModel> courseGroups)
        {
            foreach (var courseGroup in courseGroups)
            {
                var numberOfEmailOverlaps = courseGroup.DraftApprenticeships.Count(x => x.HasOverlappingEmail);
                if (numberOfEmailOverlaps > 0)
                {
                    courseGroup.EmailOverlaps = new EmailOverlapsModel(numberOfEmailOverlaps);
                }

            }
        }

        private async Task SetUlnOverlap(IReadOnlyCollection<CohortDraftApprenticeshipViewModel> draftApprenticeships)
        {
           foreach (var draftApprenticeship in draftApprenticeships)
            {
                if (!string.IsNullOrWhiteSpace(draftApprenticeship.ULN) && draftApprenticeship.StartDate.HasValue && draftApprenticeship.EndDate.HasValue)
                {
                        var result = await _commitmentsApiClient.ValidateUlnOverlap(new CommitmentsV2.Api.Types.Requests.ValidateUlnOverlapRequest
                        {
                            EndDate = draftApprenticeship.EndDate.Value,
                            StartDate = draftApprenticeship.StartDate.Value,
                            ULN = draftApprenticeship.ULN,
                            ApprenticeshipId = draftApprenticeship.Id
                        });

                    draftApprenticeship.HasOverlappingUln = result.HasOverlappingEndDate || result.HasOverlappingStartDate;
                }
            }
        }

        private async Task SetFundingBandCap(string courseCode, IEnumerable<CohortDraftApprenticeshipViewModel> draftApprenticeships)
        {
            GetTrainingProgrammeResponse course = null;
            if (!string.IsNullOrEmpty(courseCode))
            {
                try
                {
                    course = await _commitmentsApiClient.GetTrainingProgramme(courseCode);
                }
                catch (RestHttpClientException e)
                {
                    if (e.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }

            foreach (var draftApprenticeship in draftApprenticeships)
            {
                draftApprenticeship.FundingBandCap = GetFundingBandCap(course, draftApprenticeship.OriginalStartDate ?? draftApprenticeship.StartDate);
            }
        }

        private int? GetFundingBandCap(GetTrainingProgrammeResponse course, DateTime? startDate)
        {
            if (startDate == null)
            {
                return null;
            }

            if (course == null)
            {
                return null;
            }

            var cap = course.TrainingProgramme.FundingCapOn(startDate.Value);

            if (cap > 0)
            {
                return cap;
            }

            return null;
        }

        private string GetFundingBandExcessHeader(int apprenticeshipsOverCap)
        {
            if (apprenticeshipsOverCap == 1)
                return new string($"{apprenticeshipsOverCap} apprenticeship above funding band maximum");
            if (apprenticeshipsOverCap > 1)
                return new string($"{apprenticeshipsOverCap} apprenticeships above funding band maximum");
            return null;
        }

        private string GetFundingBandExcessLabel(int apprenticeshipsOverCap)
        {
            if (apprenticeshipsOverCap == 1)
                return new string("The price for this apprenticeship is above its");
            if (apprenticeshipsOverCap > 1)
                return new string("The price for these apprenticeships is above the");
            return null;
        }
    }
}
