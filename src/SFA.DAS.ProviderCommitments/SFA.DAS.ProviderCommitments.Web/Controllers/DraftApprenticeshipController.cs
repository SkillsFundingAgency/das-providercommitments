using Microsoft.AspNetCore.Authorization;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.OverlappingTrainingDateRequest;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Filters;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using SFA.DAS.ProviderUrlHelper;
using ApprenticeshipEmployerType = SFA.DAS.CommitmentsV2.Types.ApprenticeshipEmployerType;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;
using IAuthorizationService = SFA.DAS.ProviderCommitments.Interfaces.IAuthorizationService;
using RecognisePriorLearningRequest = SFA.DAS.ProviderCommitments.Web.Models.RecognisePriorLearningRequest;
using SelectCourseViewModel = SFA.DAS.ProviderCommitments.Web.Models.SelectCourseViewModel;
using SelectDeliveryModelViewModel = SFA.DAS.ProviderCommitments.Web.Models.SelectDeliveryModelViewModel;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved/{cohortReference}/apprentices")]
    [Authorize(Policy = nameof(PolicyNames.AccessCohort))]
    public class DraftApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IModelMapper _modelMapper;
        private readonly IEncodingService _encodingService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IOuterApiService _outerApiService;
        private readonly IAuthenticationService _authenticationService;

        public const string DraftApprenticeDeleted = "Apprentice record deleted";

        public DraftApprenticeshipController(IMediator mediator,
            ICommitmentsApiClient commitmentsApiClient,
            IModelMapper modelMapper,
            IEncodingService encodingService,
            IAuthorizationService authorizationService,
            IOuterApiService outerApiService,
            IAuthenticationService authenticationService)
        {
            _mediator = mediator;
            _commitmentsApiClient = commitmentsApiClient;
            _modelMapper = modelMapper;
            _encodingService = encodingService;
            _authorizationService = authorizationService;
            _outerApiService = outerApiService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        [Route("add")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddNewDraftApprenticeship(BaseReservationsAddDraftApprenticeshipRequest request, [FromServices] IConfiguration configuration)
        {
            var redirectModel = await _modelMapper.Map<AddAnotherApprenticeshipRedirectModel>(request);

            request.CacheKey = redirectModel.CacheKey;

            var route = redirectModel.UseLearnerData && configuration.GetValue<bool>("ILRFeaturesEnabled") == true
                ? RouteNames.SelectLearnerRecord
                : RouteNames.SelectCourse;
            return RedirectToRoute(route, request.CloneBaseValues());
        }

        [HttpGet]
        [Route("add/select-how")]
        public IActionResult AddAnotherSelectMethod(GetReservationIdForAddAnotherApprenticeRequest request)
        {
            if (request.UseLearnerData == false)
            {
                return RedirectToAction("GetReservationId", request);
            }

            var model = new SelectAddAnotherDraftApprenticeshipJourneyViewModel
            {
                ProviderId = request.ProviderId,
                CohortReference = request.CohortReference,
                AccountLegalEntityHashedId = request.AccountLegalEntityHashedId,
                TransferSenderHashedId = request.TransferSenderHashedId,
                EncodedPledgeApplicationId = request.EncodedPledgeApplicationId,
                UseLearnerData = request.UseLearnerData
            };

            return View(model);
        }

        [HttpPost]
        [Route("add/select-how")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public ActionResult AddAnotherSelectMethod(SelectAddAnotherDraftApprenticeshipJourneyViewModel model)
        {
            var redirectModel = new GetReservationIdForAddAnotherApprenticeRequest
            {
                ProviderId = model.ProviderId,
                CohortReference = model.CohortReference,
                AccountLegalEntityHashedId = model.AccountLegalEntityHashedId,
                TransferSenderHashedId = model.TransferSenderHashedId,
                EncodedPledgeApplicationId = model.EncodedPledgeApplicationId,
                UseLearnerData = (model.Selection == AddAnotherDraftApprenticeshipJourneyOptions.Ilr)
            };

            return RedirectToAction("GetReservationId", redirectModel);
        }

        [HttpGet]
        [Route("add/reservation")]
        public IActionResult GetReservationId(GetReservationIdForAddAnotherApprenticeRequest request, [FromServices] ILinkGenerator urlHelper)
        {
            var reservationUrl = $"{request.ProviderId}/reservations/{request.AccountLegalEntityHashedId}/select?cohortReference={request.CohortReference}&encodedPledgeApplicationId={request.EncodedPledgeApplicationId}&useLearnerData={request.UseLearnerData}";
            if (!string.IsNullOrWhiteSpace(request.TransferSenderHashedId))
            {
                reservationUrl += $"&transferSenderId={request.TransferSenderHashedId}";
            }
            return Redirect(urlHelper.ReservationsLink(reservationUrl));
        }

        [HttpGet]
        [Route("add/select-course", Name = RouteNames.SelectCourse)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeshipCourse(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<Models.DraftApprenticeship.SelectCourseViewModel>(request);
            return View(model);
        }

        [HttpPost]
        [Route("add/select-course", Name = RouteNames.SelectCourse)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> SetCourse(SelectCourseViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var request = await _modelMapper.Map<ReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModel), "DraftApprenticeship", request);
        }

        [HttpGet]
        [Route("add/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModel(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(request);

            if (model.DeliveryModels.Length > 1)
            {
                return View("SelectDeliveryModel", model);
            }

            request.DeliveryModel = model.DeliveryModels.FirstOrDefault();
            return RedirectToAction("AddDraftApprenticeship", "DraftApprenticeship", request.CloneBaseValues());
        }

        [HttpPost]
        [Route("add/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SetDeliveryModel(SelectDeliveryModelViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
            }

            var request = await _modelMapper.Map<ReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction("AddDraftApprenticeship", "DraftApprenticeship", request);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> EditDraftApprenticeshipCourse(DraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<EditDraftApprenticeshipCourseViewModel>(request);
            return View(model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> EditDraftApprenticeshipCourse(EditDraftApprenticeshipCourseViewModel model)
        {
            var request = await _modelMapper.Map<BaseDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModelForEdit), "DraftApprenticeship", request);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModelForEdit(DraftApprenticeshipRequest request)
        {
            var draft = PeekStoredEditDraftApprenticeshipState();
            var model = await _modelMapper.Map<SelectDeliveryModelForEditViewModel>(request);
            model.DeliveryModel = (DeliveryModel?)draft.DeliveryModel;

            if (model.DeliveryModels.Count > 1 || model.HasUnavailableFlexiJobAgencyDeliveryModel)
            {
                return View(model);
            }
            draft.DeliveryModel = (CommitmentsV2.Types.DeliveryModel)model.DeliveryModels.FirstOrDefault();
            StoreEditDraftApprenticeshipState(draft);

            return RedirectToAction("EditDraftApprenticeship", "DraftApprenticeship", new
            {
                request.ProviderId,
                request.DraftApprenticeshipHashedId,
                request.CohortReference
            });
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult SetDeliveryModelForEdit(SelectDeliveryModelForEditViewModel model)
        {
            var draft = PeekStoredEditDraftApprenticeshipState();
            draft.DeliveryModel = (CommitmentsV2.Types.DeliveryModel)model.DeliveryModel;
            draft.CourseCode = model.CourseCode;
            StoreEditDraftApprenticeshipState(draft);

            var request = new BaseDraftApprenticeshipRequest
            {
                CohortReference = draft.CohortReference,
                DraftApprenticeshipHashedId = draft.DraftApprenticeshipHashedId,
                ProviderId = draft.ProviderId,
            };

            return RedirectToAction("EditDraftApprenticeship", "DraftApprenticeship", request);
        }

        [HttpGet]
        [Route("add/details", Name = RouteNames.DraftApprenticeshipAddAnother)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(UseCacheForValidationAttribute))]
        public async Task<IActionResult> AddDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = GetStoredAddDraftApprenticeshipState();
            if (model == null)
            {
                model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            }
            else
            {
                model.CourseCode = request.CourseCode;
                model.DeliveryModel = request.DeliveryModel;
                model.CohortId = request.CohortId;
            }

            await AddLegalEntityAndCoursesToModel(model);
            return View(model);
        }

        [HttpPost]
        [Route("add/details")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(UseCacheForValidationAttribute))]
        public async Task<IActionResult> AddDraftApprenticeship(string changeCourse, string changeDeliveryModel, AddDraftApprenticeshipViewModel model)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                StoreAddDraftApprenticeshipState(model);
                var req = await _modelMapper.Map<BaseReservationsAddDraftApprenticeshipRequest>(model);
                var redirectAction = changeCourse == "Edit" ? nameof(AddDraftApprenticeshipCourse) : nameof(SelectDeliveryModel);

                return RedirectToAction(redirectAction, "DraftApprenticeship", req);
            }

            var overlapResult = await HasStartDateOverlap(model);

            if (overlapResult != null && overlapResult.HasStartDateOverlap && overlapResult.HasOverlapWithApprenticeshipId.HasValue)
            {
                StoreAddDraftApprenticeshipState(model);
                var hashedApprenticeshipId = _encodingService.Encode(overlapResult.HasOverlapWithApprenticeshipId.Value, EncodingType.ApprenticeshipId);

                return RedirectToAction("DraftApprenticeshipOverlapAlert", "OverlappingTrainingDateRequest", new
                {
                    OverlapApprenticeshipHashedId = hashedApprenticeshipId,
                    model.ReservationId,
                    StartMonthYear = model.StartDate.MonthYear,
                    model.CourseCode,
                    model.DeliveryModel,
                    model.ProviderId,
                });
            }

            model.ActualStartDate = new DateModel();

            var request = await _modelMapper.Map<AddDraftApprenticeshipApimRequest>(model);
            request.UserId = _authenticationService.UserId;

            var response = await _outerApiService.AddDraftApprenticeship(model.CohortId.Value, request);

            if (string.IsNullOrEmpty(model.CourseCode))
            {
                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
            }

            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, response.DraftApprenticeshipId);
            var draftApprenticeshipHashedId = _encodingService.Encode(draftApprenticeship.Id, EncodingType.ApprenticeshipId);

            if (draftApprenticeship.HasStandardOptions)
            {
                return RedirectToAction("SelectOptions", new { model.ProviderId, draftApprenticeshipHashedId, model.CohortReference });
            }

            return RedirectToAction("RecognisePriorLearning", "DraftApprenticeship", new
            {
                model.ProviderId,
                model.CohortReference,
                draftApprenticeshipHashedId
            });
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(UseCacheForValidationAttribute))]
        public async Task<IActionResult> EditDraftApprenticeship(string changeCourse, string changeDeliveryModel, EditDraftApprenticeshipViewModel model)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                StoreEditDraftApprenticeshipState(model);
                var req = await _modelMapper.Map<BaseDraftApprenticeshipRequest>(model);

                var redirectAction = changeCourse == "Edit" ? nameof(EditDraftApprenticeshipCourse) : nameof(SelectDeliveryModelForEdit);
                return RedirectToAction(redirectAction, "DraftApprenticeship", req);
            }

            var overlapResult = await HasStartDateOverlap(model);
            if (overlapResult != null && overlapResult.HasStartDateOverlap && overlapResult.HasOverlapWithApprenticeshipId.HasValue)
            {
                StoreEditDraftApprenticeshipState(model);
                var hashedApprenticeshipId = _encodingService.Encode(overlapResult.HasOverlapWithApprenticeshipId.Value, EncodingType.ApprenticeshipId);
                return RedirectToAction("DraftApprenticeshipOverlapAlert", "OverlappingTrainingDateRequest", new
                {
                    DraftApprenticeshipHashedId = model.DraftApprenticeshipHashedId,
                    OverlapApprenticeshipHashedId = hashedApprenticeshipId,
                    model.ProviderId,
                });
            }

            model.ActualStartDate = new DateModel();
            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipApimRequest>(model);
            await _outerApiService.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);

            
            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value);

            if (draftApprenticeship.HasStandardOptions)
            {
                return RedirectToOptionalPages(
                    draftApprenticeship.HasStandardOptions,
                    model.ProviderId,
                    model.DraftApprenticeshipHashedId,
                    model.CohortReference);
            }

            return RedirectToAction("RecognisePriorLearning", "DraftApprenticeship", new
            {
                model.ProviderId,
                model.CohortReference,
                model.DraftApprenticeshipHashedId,
            });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit", Name = RouteNames.DraftApprenticeshipEdit)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        [ServiceFilter(typeof(UseCacheForValidationAttribute))]
        public async Task<IActionResult> EditDraftApprenticeship(DraftApprenticeshipRequest request)
        {
            try
            {
                var model = await _modelMapper.Map<IDraftApprenticeshipViewModel>(request);

                if (model is EditDraftApprenticeshipViewModel editModel)
                {
                    await AddLegalEntityAndCoursesToModel(editModel);
                    PrePopulateDates(editModel);
                    return View("EditDraftApprenticeship", editModel);
                }

                return View("ViewDraftApprenticeship", model as ViewDraftApprenticeshipViewModel);
            }
            catch (Exception e) when (e is DraftApprenticeshipNotFoundException)
            {
                return RedirectToAction("Details", "Cohort", new { request.ProviderId, request.CohortReference });
            }
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning", Name = RouteNames.RecognisePriorLearning)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearning(RecognisePriorLearningRequest request)
        {
            var model = await _modelMapper.Map<RecognisePriorLearningViewModel>(request);
            if (!model.RplNeedsToBeConsidered)
            {
                return RedirectToAction("Details", "Cohort", new { request.ProviderId, request.CohortReference });
            }
            
            if (!model.IsRplRequired)
            {
                model.IsTherePriorLearning = false;
                return await RecognisePriorLearning(model);
            }
            
            return View("RecognisePriorLearning", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning", Name = RouteNames.RecognisePriorLearning)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearning(RecognisePriorLearningViewModel request)
        {
            _ = await _modelMapper.Map<RecognisePriorLearningResult>(request);

            if (request.IsTherePriorLearning == true)
            {
                return RedirectToAction("RecognisePriorLearningData", "DraftApprenticeship", new
                {
                    request.ProviderId,
                    request.DraftApprenticeshipHashedId,
                    request.CohortReference,
                });
            }

            return RedirectToAction("Details", "Cohort", new {request.ProviderId, request.CohortReference } );
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-data", Name = RouteNames.RecognisePriorLearningData)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearningData(RecognisePriorLearningRequest request)
        {
            var model = await _modelMapper.Map<PriorLearningDataViewModel>(request);
            return View("RecognisePriorLearningData", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-data")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearningData(PriorLearningDataViewModel model)
        {
            var result = await _modelMapper.Map<RecognisePriorLearningResult>(model);

            if (result?.RplPriceReductionError == true)
            {
                return RedirectToAction(nameof(RecognisePriorLearningSummary), "DraftApprenticeship",
                    new { model.ProviderId, model.DraftApprenticeshipHashedId, model.CohortReference });
            }
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-summary")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearningSummary(PriorLearningSummaryRequest request)
        {
            var model = await _modelMapper.Map<PriorLearningSummaryViewModel>(request);

            if (model.RplPriceReductionError == true)
            {
                return View("RecognisePriorLearningSummary", model);
            }

            return RedirectToAction("Details", "Cohort", new { request.ProviderId, request.CohortReference });
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-summary")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult RecognisePriorLearningSummary(PriorLearningSummaryViewModel model)
        {
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectOptions(SelectOptionsRequest request)
        {
            var model = await _modelMapper.Map<ViewSelectOptionsViewModel>(request);

            if (!model.Options.Any())
            {
                return RedirectToAction("RecognisePriorLearning", new { model.ProviderId, model.CohortReference, model.DraftApprenticeshipHashedId });
            }

            return View("SelectStandardOption", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        public async Task<IActionResult> PostSelectOptions(ViewSelectOptionsViewModel model)
        {
            var request = await _modelMapper.Map<UpdateDraftApprenticeshipApimRequest>(model);

            await _outerApiService.UpdateDraftApprenticeship(model.CohortId, model.DraftApprenticeshipId, request);

            return RedirectToAction("RecognisePriorLearning", new { model.ProviderId, model.CohortReference, model.DraftApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}")]
        [Authorize(Policy = nameof(PolicyNames.HasViewerOrAbovePermission))]
        public async Task<IActionResult> ViewDraftApprenticeship(DraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<ViewDraftApprenticeshipViewModel>(request);

            return View("ViewDraftApprenticeship", model);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/Delete", Name = RouteNames.ApprenticeDelete)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> DeleteConfirmation(DeleteConfirmationRequest deleteConfirmationRequest)
        {
            try
            {
                var viewModel = await _modelMapper.Map<DeleteConfirmationViewModel>(deleteConfirmationRequest);
                return View(viewModel);
            }
            catch (Exception e) when (e is DraftApprenticeshipNotFoundException)
            {
                return RedirectToAction("Details", "Cohort", new { deleteConfirmationRequest.ProviderId, deleteConfirmationRequest.CohortReference });
            }
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/Delete", Name = RouteNames.ApprenticeDelete)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> DeleteConfirmation(DeleteConfirmationViewModel viewModel)
        {
            if (viewModel.DeleteConfirmed != null && viewModel.DeleteConfirmed.Value)
            {
                await _commitmentsApiClient.DeleteDraftApprenticeship(viewModel.CohortId, viewModel.DraftApprenticeshipId, new DeleteDraftApprenticeshipRequest(), CancellationToken.None);
                TempData.AddFlashMessage(DraftApprenticeDeleted, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            }

            return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
        }

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _commitmentsApiClient.GetCohort(model.CohortId.Value);

            var courses = await GetCourses(cohortDetail);

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = courses;
        }

        private static void PrePopulateDates(EditDraftApprenticeshipViewModel model)
        {
            EnsurePlannedStartDatePrePopulation(model);
        }

        private static void EnsurePlannedStartDatePrePopulation(EditDraftApprenticeshipViewModel model)
        {
            if (model.StartDate.HasValue)
                return;

            model.StartYear = model.ActualStartYear;
            model.StartMonth = model.ActualStartMonth;
        }

        private async Task<TrainingProgramme[]> GetCourses(GetCohortResponse cohortDetails)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = (!cohortDetails.IsFundedByTransfer &&
                                     cohortDetails.LevyStatus != ApprenticeshipEmployerType.NonLevy)
                                    || cohortDetails.IsLinkedToChangeOfPartyRequest
            });

            return result.TrainingCourses;
        }

        private void StoreAddDraftApprenticeshipState(AddDraftApprenticeshipViewModel model)
        {
            TempData.Put(nameof(AddDraftApprenticeshipViewModel), model);
        }

        private AddDraftApprenticeshipViewModel GetStoredAddDraftApprenticeshipState()
        {
            return TempData.Get<AddDraftApprenticeshipViewModel>(nameof(AddDraftApprenticeshipViewModel));
        }

        private void StoreEditDraftApprenticeshipState(EditDraftApprenticeshipViewModel model)
        {
            TempData.Put(nameof(EditDraftApprenticeshipViewModel), model);
        }

        private EditDraftApprenticeshipViewModel PeekStoredEditDraftApprenticeshipState()
        {
            return TempData.GetButDontRemove<EditDraftApprenticeshipViewModel>(nameof(EditDraftApprenticeshipViewModel));
        }

        private EditDraftApprenticeshipViewModel GetStoredEditDraftApprenticeshipState()
        {
            return TempData.Get<EditDraftApprenticeshipViewModel>(nameof(EditDraftApprenticeshipViewModel));
        }

        private IActionResult RedirectToOptionalPages(bool hasStandardOptions, long providerId, string draftApprenticeshipHashedId, string cohortReference)
        {
            var routeValues = new
            {
                providerId,
                draftApprenticeshipHashedId,
                cohortReference,
            };

            return hasStandardOptions
                ? RedirectToAction("SelectOptions", "DraftApprenticeship", routeValues)
                : RedirectToAction("Details", "Cohort", routeValues);
        }

        private async Task<ValidateUlnOverlapOnStartDateQueryResult> HasStartDateOverlap(DraftApprenticeshipViewModel model)
        {
            if (!model.StartDate.Date.HasValue || !model.EndDate.Date.HasValue || string.IsNullOrWhiteSpace(model.Uln))
            {
                return null;
            }

            var apimRequest = await _modelMapper.Map<ValidateDraftApprenticeshipApimRequest>(model);
            await _outerApiService.ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(apimRequest);

            return await _outerApiService.ValidateUlnOverlapOnStartDate(
                model.ProviderId,
                model.Uln,
                model.StartDate.Date.Value.ToString("dd-MM-yyyy"),
                model.EndDate.Date.Value.ToString("dd-MM-yyyy")
            );
        }
    }
}