using System.Net;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.ProviderCommitments.Enums;
using SFA.DAS.ProviderCommitments.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Services;
using ApprenticeshipEmailOverlap = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.ApprenticeshipEmailOverlap;
using ApprenticeshipEmployerType = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ApprenticeshipEmployerType;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;
using DraftApprenticeshipDto = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.DraftApprenticeshipDto;
using LastAction = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.LastAction;
using Party = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Party;
using TransferApprovalStatus = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.TransferApprovalStatus;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;

public class DetailsViewModelMapper(
    ICommitmentsApiClient commitmentsApiClient,
    IEncodingService encodingService,
    IOuterApiClient outerApiClient,
    ITempDataStorageService storageService,
    IConfiguration configuration)
    : IMapper<DetailsRequest, DetailsViewModel>
{
    public async Task<DetailsViewModel> Map(DetailsRequest source)
    {
        //clear leftover tempdata from add/edit
        //this solution should NOT use tempdata in this way
        storageService.RemoveFromCache<EditDraftApprenticeshipViewModel>();

        var cohortId = encodingService.Decode(source.CohortReference, EncodingType.CohortReference);
        var cohortDetailsTask = outerApiClient.Get<GetCohortDetailsResponse>(new GetCohortDetailsRequest(source.ProviderId, cohortId));
       
        var providerStatusTask = outerApiClient.Get<GetProviderDetailsResponse>(new GetProviderDetailsRequest(source.ProviderId));

        await Task.WhenAll(cohortDetailsTask, providerStatusTask);

        var providerStatus = await providerStatusTask;
        var cohortDetails = await cohortDetailsTask;        

        var emailOverlaps = cohortDetails.ApprenticeshipEmailOverlaps.ToList();

        var courses = await GroupCourses(cohortDetails.DraftApprenticeships, emailOverlaps, cohortDetails);
        var viewOrApprove = cohortDetails.WithParty == Party.Provider ? "Check" : "View";
        var isAgreementSigned = (ProviderStatusType) providerStatus.ProviderStatusTypeId == ProviderStatusType.Active;

        return new DetailsViewModel
        {
            ProviderId = source.ProviderId,
            HasNoDeclaredStandards = cohortDetails.HasNoDeclaredStandards,
            CohortReference = source.CohortReference,
            WithParty = cohortDetails.WithParty,
            AccountLegalEntityHashedId = encodingService.Encode(cohortDetails.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
            LegalEntityName = cohortDetails.LegalEntityName,
            ProviderName = cohortDetails.ProviderName,
            TransferSenderHashedId = cohortDetails.TransferSenderId == null ? null : encodingService.Encode(cohortDetails.TransferSenderId.Value, EncodingType.PublicAccountId),
            EncodedPledgeApplicationId = cohortDetails.PledgeApplicationId == null ? null : encodingService.Encode(cohortDetails.PledgeApplicationId.Value, EncodingType.PledgeApplicationId),
            Message = cohortDetails.LatestMessageCreatedByEmployer,
            Courses = courses,
            PageTitle = $"Check apprentice details",
            IsApprovedByEmployer = cohortDetails.IsApprovedByEmployer,
            IsAgreementSigned = isAgreementSigned,
            IsCompleteForProvider = cohortDetails.IsCompleteForProvider,
            HasEmailOverlaps = emailOverlaps.Any(),
            ShowAddAnotherApprenticeOption = !cohortDetails.IsLinkedToChangeOfPartyRequest,
            AllowBulkUpload = cohortDetails.LevyStatus == ApprenticeshipEmployerType.Levy
                              && cohortDetails.WithParty == Party.Provider
                              && !cohortDetails.IsLinkedToChangeOfPartyRequest,
            IsLinkedToChangeOfPartyRequest = cohortDetails.IsLinkedToChangeOfPartyRequest,
            Status = GetCohortStatus(cohortDetails, cohortDetails.DraftApprenticeships),
            ShowRofjaaRemovalBanner = cohortDetails.HasUnavailableFlexiJobAgencyDeliveryModel,
            InvalidProviderCourseCodes = cohortDetails.InvalidProviderCourseCodes.ToList(),
            RplErrorDraftApprenticeshipIds = cohortDetails.RplErrorDraftApprenticeshipIds.ToList(),
            UseLearnerData = configuration.GetValue<bool>("ILRFeaturesEnabled"),
            HasAgeRestrictedApprenticeships = cohortDetails.HasAgeRestrictedApprenticeships
        };

    }

    private static string GetCohortStatus(GetCohortDetailsResponse cohort, IReadOnlyCollection<DraftApprenticeshipDto> draftApprenticeships)
    {
        if (cohort.TransferSenderId.HasValue &&
            cohort.TransferApprovalStatus == TransferApprovalStatus.Pending)
        {
            if (cohort.WithParty == Party.TransferSender)
            {
                return "Pending - with funding employer";
            }

            if (cohort.WithParty == Party.Employer)
            {
                return GetEmployerOnlyStatus(cohort);
            }

            if (cohort.WithParty == Party.Provider)
            {
                return GetProviderOnlyStatus(cohort);
            }
        }
        else if (cohort.TransferSenderId.HasValue &&
                 cohort.TransferApprovalStatus == TransferApprovalStatus.Rejected)
        {
            return "Rejected by transfer sending employer";
        }
        else if (cohort.IsApprovedByEmployer && cohort.IsApprovedByProvider)
        {
            return "Approved";
        }
        else if (cohort.WithParty == Party.Provider)
        {
            return GetProviderOnlyStatus(cohort);
        }
        else if (cohort.WithParty == Party.Employer)
        {
            return GetEmployerOnlyStatus(cohort);
        }

        return "New request";
    }

    private static string GetProviderOnlyStatus(GetCohortDetailsResponse cohort)
    {
        if (cohort.LastAction == LastAction.None)
        {
            return "New request";
        }

        if (cohort.LastAction == LastAction.Amend)
        {
            return "Ready for review";
        }
        if (cohort.LastAction == LastAction.Approve)
        {
            if (!cohort.IsApprovedByProvider && !cohort.IsApprovedByEmployer)
                return "Ready for review";

            return "Ready for approval";
        }
        return "New request";
    }

    private static string GetEmployerOnlyStatus(GetCohortDetailsResponse cohort)
    {
        if (cohort.LastAction == LastAction.None)
        {
            return "New request";
        }

        if (cohort.LastAction == LastAction.Amend)
        {
            return "Under review with employer";
        }

        if (cohort.LastAction == LastAction.Approve)
        {
            return "With Employer for approval";
        }
        return "Under review with employer";
    }

    private async Task<IReadOnlyCollection<DetailsViewCourseGroupingModel>> GroupCourses(IEnumerable<DraftApprenticeshipDto> draftApprenticeships, List<ApprenticeshipEmailOverlap> emailOverlaps, GetCohortDetailsResponse cohortResponse)
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
                        DraftApprenticeshipHashedId = encodingService.Encode(a.Id, EncodingType.ApprenticeshipId),
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Cost = a.Cost,
                        TrainingPrice = a.TrainingPrice,
                        EndPointAssessmentPrice = a.EndPointAssessmentPrice,
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
                        HasLearnerDataChanges = a.HasLearnerDataChanges,
                        LastLearnerDataSync = a.LastLearnerDataSync,
                        IsEditable = a.LearnerDataId == null, 
                        RecognisePriorLearning = a.RecognisePriorLearning,
                        ShowRplAddLink = !cohortResponse.IsLinkedToChangeOfPartyRequest && a.RecognisePriorLearning != true
                    })
                    .ToList()
            })
            .OrderBy(c => c.CourseName)
            .ToList();

        PopulateFundingBandExcessModels(groupedByCourse);
        SetRplErrors(groupedByCourse, cohortResponse);
        PopulateEmailOverlapsModel(groupedByCourse);
        await CheckUlnOverlap(groupedByCourse);
        await CheckForPendingOverlappingTrainingDateRequest(groupedByCourse);

        return groupedByCourse;
    }

    private bool IsDraftApprenticeshipComplete(DraftApprenticeshipDto draftApprenticeship, GetCohortDetailsResponse cohortResponse)
    {
        if (string.IsNullOrWhiteSpace(draftApprenticeship.FirstName)
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

        if (draftApprenticeship.RecognisingPriorLearningExtendedStillNeedsToBeConsidered)
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
        var overlapRequestQueryResultsTasks = new List<Task<GetOverlapRequestQueryResult>>();
        foreach (var draftApprenticeship in draftApprenticeships)
        {
            if (!string.IsNullOrWhiteSpace(draftApprenticeship.ULN) && draftApprenticeship.StartDate.HasValue && draftApprenticeship.EndDate.HasValue)
            {
                var result = outerApiClient.Get<GetOverlapRequestQueryResult>(new GetOverlapRequestQueryRequest(draftApprenticeship.Id));
                overlapRequestQueryResultsTasks.Add(result);
            }
        }

        await Task.WhenAll(overlapRequestQueryResultsTasks);

        foreach (var task in overlapRequestQueryResultsTasks)
        {
            var result = task.Result;

            if (result is { DraftApprenticeshipId: not null })
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
            var numberExceedingBand = apprenticesExceedingFundingBand.Count;

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

    private static void PopulateEmailOverlapsModel(List<DetailsViewCourseGroupingModel> courseGroups)
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

    private static void SetRplErrors(List<DetailsViewCourseGroupingModel> courseGroups, GetCohortDetailsResponse cohort)
    {
        foreach (var courseGroup in courseGroups)
        {
            courseGroup.RplErrors = courseGroup.DraftApprenticeships.Count(x => cohort.RplErrorDraftApprenticeshipIds.Contains(x.Id));
        }
    }

    private async Task SetUlnOverlap(IReadOnlyCollection<CohortDraftApprenticeshipViewModel> draftApprenticeships)
    {
        foreach (var draftApprenticeship in draftApprenticeships)
        {
            if (!string.IsNullOrWhiteSpace(draftApprenticeship.ULN) && draftApprenticeship.StartDate.HasValue && draftApprenticeship.EndDate.HasValue)
            {
                var result = await commitmentsApiClient.ValidateUlnOverlap(new CommitmentsV2.Api.Types.Requests.ValidateUlnOverlapRequest
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
                course = await commitmentsApiClient.GetTrainingProgramme(courseCode);
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

    private static int? GetFundingBandCap(GetTrainingProgrammeResponse course, DateTime? startDate)
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

    private static string GetFundingBandExcessHeader(int apprenticeshipsOverCap)
    {
        if (apprenticeshipsOverCap == 1)
            return new string($"{apprenticeshipsOverCap} apprenticeship above funding band maximum");
        if (apprenticeshipsOverCap > 1)
            return new string($"{apprenticeshipsOverCap} apprenticeships above funding band maximum");
        return null;
    }

    private static string GetFundingBandExcessLabel(int apprenticeshipsOverCap)
    {
        if (apprenticeshipsOverCap == 1)
            return new string("The price for this apprenticeship is above its");
        if (apprenticeshipsOverCap > 1)
            return new string("The price for these apprenticeships is above the");
        return null;
    }
}