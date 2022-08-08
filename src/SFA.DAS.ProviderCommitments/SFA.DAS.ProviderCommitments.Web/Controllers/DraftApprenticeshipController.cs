using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderFeatures.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;
using SFA.DAS.ProviderCommitments.Web.Attributes;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Exceptions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.RouteValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IAuthorizationService = SFA.DAS.Authorization.Services.IAuthorizationService;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved/{cohortReference}/apprentices")]
    [DasAuthorize(CommitmentOperation.AccessCohort)]
    public class DraftApprenticeshipController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IModelMapper _modelMapper;
        private readonly IEncodingService _encodingService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IOuterApiService _outerApiService;
        public const string DraftApprenticeDeleted = "Apprentice record deleted";

        public DraftApprenticeshipController(IMediator mediator, ICommitmentsApiClient commitmentsApiClient,
            IModelMapper modelMapper, IEncodingService encodingService,
            IAuthorizationService authorizationService,
            IOuterApiService outerApiService)
        {
            _mediator = mediator;
            _commitmentsApiClient = commitmentsApiClient;
            _modelMapper = modelMapper;
            _encodingService = encodingService;
            _authorizationService = authorizationService;
            _outerApiService = outerApiService;
        }

        [HttpGet]
        [Route("add")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public IActionResult AddNewDraftApprenticeship(ReservationsAddDraftApprenticeshipRequest request)
        {
            if (_authorizationService.IsAuthorized(ProviderFeature.DeliveryModel))
            {
                return RedirectToAction(nameof(SelectCourse), request);
            }

            return RedirectToAction(nameof(AddDraftApprenticeship), request);
        }

        [HttpGet]
        [Route("add/select-course")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourse(ReservationsAddDraftApprenticeshipRequest request)
        {
            var draft = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            await AddLegalEntityAndCoursesToModel(draft);
            var model = new SelectCourseViewModel
            {
                CourseCode = draft.CourseCode,
                Courses = draft.Courses
            };

            return View("SelectCourse", model);
        }

        [HttpPost]
        [Route("add/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> SetCourse(SelectCourseViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var request = await _modelMapper.Map<ReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModel), request);
        }

        [HttpGet]
        [Route("add/select-delivery-model")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModel(ReservationsAddDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<Models.SelectDeliveryModelViewModel>(request);

            if (model.DeliveryModels.Length > 1)
            {
                return View("SelectDeliveryModel", model);
            }

            request.DeliveryModel = model.DeliveryModels.FirstOrDefault();
            return RedirectToAction("AddDraftApprenticeship", request);
        }

        [HttpPost]
        [Route("add/select-delivery-model")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SetDeliveryModel(Models.SelectDeliveryModelViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
            }

            var request = await _modelMapper.Map<ReservationsAddDraftApprenticeshipRequest>(model);
            return RedirectToAction("AddDraftApprenticeship", request);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectCourseForEdit(DraftApprenticeshipRequest request)
        {
            var draft = PeekStoredEditDraftApprenticeshipState();
            await AddLegalEntityAndCoursesToModel(draft);
            var model = new SelectCourseViewModel
            {
                CourseCode = draft.CourseCode,
                Courses = draft.Courses
            };

            return View("SelectCourse", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit/select-course")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<ActionResult> SetCourseForEdit(SelectCourseViewModel model)
        {
            if (string.IsNullOrEmpty(model.CourseCode))
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail(nameof(model.CourseCode), "You must select a training course")});
            }

            var draft = PeekStoredEditDraftApprenticeshipState();
            draft.CourseCode = model.CourseCode;
            StoreEditDraftApprenticeshipState(draft);

            var request = await _modelMapper.Map<BaseDraftApprenticeshipRequest>(model);
            return RedirectToAction(nameof(SelectDeliveryModelForEdit), request);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectDeliveryModelForEdit(DraftApprenticeshipRequest request)
        {
            var draft = PeekStoredEditDraftApprenticeshipState();
            var model = await _modelMapper.Map<Models.SelectDeliveryModelViewModel>(draft);

            if (model.DeliveryModels.Length > 1)
            {
                return View("SelectDeliveryModel", model);
            }
            draft.DeliveryModel = model.DeliveryModels.FirstOrDefault();
            StoreEditDraftApprenticeshipState(draft);

            return RedirectToAction("EditDraftApprenticeship", request);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit/select-delivery-model")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SetDeliveryModelForEdit(Models.SelectDeliveryModelViewModel model)
        {
            if (model.DeliveryModel == null)
            {
                throw new CommitmentsApiModelException(new List<ErrorDetail>
                    {new ErrorDetail("DeliveryModel", "You must select the apprenticeship delivery model")});
            }

            var draft = PeekStoredEditDraftApprenticeshipState();
            draft.DeliveryModel = model.DeliveryModel;
            StoreEditDraftApprenticeshipState(draft);


            var request = await _modelMapper.Map<BaseDraftApprenticeshipRequest>(model);
            return RedirectToAction("EditDraftApprenticeship", request);
        }

        [HttpGet]
        [Route("add-another")]
        [RequireQueryParameter("ReservationId")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
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
        [Route("add-another")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(string changeCourse, string changeDeliveryModel, AddDraftApprenticeshipViewModel model)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                StoreAddDraftApprenticeshipState(model);
                var req = await _modelMapper.Map<BaseReservationsAddDraftApprenticeshipRequest>(model);
                return RedirectToAction(changeCourse == "Edit" ? nameof(SelectCourse) : nameof(SelectDeliveryModel), req);
            }

            if (await HasStartDateOverlap(model))
            {
                StoreAddDraftApprenticeshipState(model);
                return RedirectToAction(nameof(DraftApprenticeshipOverlapAlert));
            }

            var request = await _modelMapper.Map<AddDraftApprenticeshipRequest>(model);
            request.UserId = User.Upn();

            var response = await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);

            if (RequireRpl(model.StartDate))
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(response.DraftApprenticeshipId, EncodingType.ApprenticeshipId);
                return RedirectToAction("RecognisePriorLearning", "DraftApprenticeship", new { model.CohortReference, draftApprenticeshipHashedId });
            }
            else
            {
                if (string.IsNullOrEmpty(model.CourseCode))
                {
                    return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
                }

                var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, response.DraftApprenticeshipId);

                if (draftApprenticeship.HasStandardOptions)
                {
                    var draftApprenticeshipHashedId = _encodingService.Encode(draftApprenticeship.Id, EncodingType.ApprenticeshipId);
                    return RedirectToAction("SelectOptions", "DraftApprenticeship", new { model.ProviderId, draftApprenticeshipHashedId, model.CohortReference });
                }

                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
            }
        }

        [HttpGet]
        [Route("add/apprenticeship/overlap-alert", Name = RouteNames.DraftApprenticeshipOverlapAlert)]
        public IActionResult DraftApprenticeshipOverlapAlert(DraftApprenticeshipOverlapAlertRequest request, [FromServices] IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService)
        {
            var featureToggleEnabled = featureTogglesService.GetFeatureToggle(ProviderFeature.OverlappingTrainingDate).IsEnabled;

            DraftApprenticeshipViewModel model;
            if (request.DraftApprenticeshipHashedId != null)
            {
                model = PeekStoredEditDraftApprenticeshipState();
            }
            else
            {
                model = PeekStoredAddDraftApprenticeshipState();
            }

            var vm = new DraftApprenticeshipOverlapAlertViewModel
            {
                OverlappingTrainingDateRequestToggleEnabled = featureToggleEnabled,
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                StartDate = model.StartDate.Date.GetValueOrDefault(),
                EndDate = model.EndDate.Date.GetValueOrDefault(),
                Uln = model.Uln,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            return View(vm);
        }

        [HttpPost]
        [Route("add/apprenticeship/overlap-alert")]
        public IActionResult DraftApprenticeshipOverlapAlert(DraftApprenticeshipOverlapAlertViewModel viewModel)
        {
            return RedirectToAction(nameof(DraftApprenticeshipOverlapOptions), new { DraftApprenticeshipHashedId = viewModel.DraftApprenticeshipHashedId });
        }

        [HttpGet]
        [Route("edit/overlap-options")]
        public IActionResult DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionRequest request, [FromServices] IFeatureTogglesService<ProviderFeatureToggle> featureTogglesService)
        {
            var featureToggleEnabled = featureTogglesService.GetFeatureToggle(ProviderFeature.OverlappingTrainingDate).IsEnabled;
            var vm = new DraftApprenticeshipOverlapOptionViewModel
            {
                DraftApprenticeshipHashedId = request.DraftApprenticeshipHashedId,
                OverlappingTrainingDateRequestToggleEnabled = featureToggleEnabled
            };
            return View(vm);
        }

        [HttpPost]
        [Route("edit/overlap-options")]
        public async Task<IActionResult> DraftApprenticeshipOverlapOptions(DraftApprenticeshipOverlapOptionViewModel viewModel)
        {
            var model = GetStoredAddDraftApprenticeshipState();

            if (viewModel.OverlapOptions == OverlapOptions.AddApprenticeshipLater)
            {
                // redirect 302 does not clear tempdata.
                RemoveStoredDraftApprenticeshipState();
                return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
            }

            if (string.IsNullOrWhiteSpace(viewModel.DraftApprenticeshipHashedId))
            {
                await AddDraftApprenticeship(viewModel, model);
            }
            else
            {
                await UpdateDraftApprenticeship();
            }

            if (viewModel.OverlapOptions == OverlapOptions.SendStopRequest)
            {
                var createOverlappingTrainingDateApimRequest = await _modelMapper.Map<CreateOverlappingTrainingDateApimRequest>(viewModel);
                await _outerApiService.CreateOverlappingTrainingDateRequest(createOverlappingTrainingDateApimRequest);
            }

            return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
        }

        private async Task AddDraftApprenticeship(DraftApprenticeshipOverlapOptionViewModel viewModel, AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<AddDraftApprenticeshipRequest>(model);
            request.IgnoreStartDateOverlap = true;
            request.UserId = User.Upn();

            var response = await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);
            viewModel.DraftApprenticeshipId = response.DraftApprenticeshipId;
        }

        private async Task UpdateDraftApprenticeship()
        {
            var editModel = GetStoredEditDraftApprenticeshipState();
            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(editModel);
            updateRequest.IgnoreStartDateOverlap = true;
            await _commitmentsApiClient.UpdateDraftApprenticeship(editModel.CohortId.Value, editModel.DraftApprenticeshipId.Value, updateRequest);
        }

        private bool RequireRpl(MonthYearModel startDate)
        {
            if (!_authorizationService.IsAuthorized(ProviderFeature.RecognitionOfPriorLearning))
                return false;

            return startDate?.Date >= new DateTime(2022, 08, 01);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> EditDraftApprenticeship(string changeCourse, string changeDeliveryModel, EditDraftApprenticeshipViewModel model)
        {
            if (changeCourse == "Edit" || changeDeliveryModel == "Edit")
            {
                StoreEditDraftApprenticeshipState(model);
                var req = await _modelMapper.Map<BaseDraftApprenticeshipRequest>(model);
                return RedirectToAction(changeCourse == "Edit" ? nameof(SelectCourseForEdit) : nameof(SelectDeliveryModelForEdit), req);
            }

            if (await HasStartDateOverlap(model))
            {
                StoreEditDraftApprenticeshipState(model);
                return RedirectToAction(nameof(DraftApprenticeshipOverlapAlert), new { DraftApprenticeshipHashedId = model.DraftApprenticeshipHashedId });
            }


            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(model);
            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);

            if (RequireRpl(model.StartDate))
            {
                return RedirectToAction("RecognisePriorLearning", "DraftApprenticeship", new
                {
                    model.CohortReference,
                    model.DraftApprenticeshipHashedId,
                });
            }
            else
            {
                var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value);
                return RedirectToOptionalPages(
                    draftApprenticeship.HasStandardOptions,
                    model.ProviderId,
                    model.DraftApprenticeshipHashedId,
                    model.CohortReference);
            }
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/edit")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ViewEditDraftApprenticeship(DraftApprenticeshipRequest request)
        {
            try
            {
                IDraftApprenticeshipViewModel model = GetStoredEditDraftApprenticeshipState() ?? await _modelMapper.Map<IDraftApprenticeshipViewModel>(request);

                if (model is EditDraftApprenticeshipViewModel editModel)
                {

                    await AddLegalEntityAndCoursesToModel(editModel);
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
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearning(Models.RecognisePriorLearningRequest request)
        {
            var model = await _modelMapper.Map<RecognisePriorLearningViewModel>(request);
            return View("RecognisePriorLearning", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearning(RecognisePriorLearningViewModel request)
        {
            var result = await _modelMapper.Map<RecognisePriorLearningResult>(request);

            if (request.IsTherePriorLearning == true)
            {
                return RedirectToAction("RecognisePriorLearningDetails", "DraftApprenticeship", new
                {
                    request.ProviderId,
                    request.DraftApprenticeshipHashedId,
                    request.CohortReference,
                });
            }
            else
            {
                return RedirectToOptionalPages(
                    result.HasStandardOptions,
                    request.ProviderId,
                    request.DraftApprenticeshipHashedId,
                    request.CohortReference);
            }
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-details")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearningDetails(Models.RecognisePriorLearningRequest request)
        {
            var model = await _modelMapper.Map<PriorLearningDetailsViewModel>(request);
            return View("RecognisePriorLearningDetails", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/recognise-prior-learning-details")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> RecognisePriorLearningDetails(PriorLearningDetailsViewModel request)
        {
            var result = await _modelMapper.Map<RecognisePriorLearningResult>(request);

            return RedirectToOptionalPages(
                result.HasStandardOptions,
                request.ProviderId,
                request.DraftApprenticeshipHashedId,
                request.CohortReference);
        }

        [HttpGet]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectOptions(SelectOptionsRequest request)
        {
            var model = await _modelMapper.Map<ViewSelectOptionsViewModel>(request);

            if (!model.Options.Any())
            {
                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
            }

            return View("SelectStandardOption", model);
        }

        [HttpPost]
        [Route("{DraftApprenticeshipHashedId}/select-options")]
        public async Task<IActionResult> PostSelectOptions(ViewSelectOptionsViewModel model)
        {
            var request = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(model);

            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId, model.DraftApprenticeshipId, request);

            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
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
            if (viewModel.DeleteConfirmed != null && !viewModel.DeleteConfirmed.Value)
            {
                return RedirectToAction("ViewEditDraftApprenticeship", "DraftApprenticeship", new DraftApprenticeshipRequest
                {
                    ProviderId = viewModel.ProviderId,
                    CohortReference = viewModel.CohortReference,
                    DraftApprenticeshipHashedId = viewModel.DraftApprenticeshipHashedId
                });
            }

            await _commitmentsApiClient.DeleteDraftApprenticeship(viewModel.CohortId, viewModel.DraftApprenticeshipId, new DeleteDraftApprenticeshipRequest(), CancellationToken.None);
            TempData.AddFlashMessage(DraftApprenticeDeleted, ITempDataDictionaryExtensions.FlashMessageLevel.Success);
            return RedirectToAction("Details", "Cohort", new { viewModel.ProviderId, viewModel.CohortReference });
        }

        private async Task AddLegalEntityAndCoursesToModel(DraftApprenticeshipViewModel model)
        {
            var cohortDetail = await _commitmentsApiClient.GetCohort(model.CohortId.Value);

            var courses = await GetCourses(cohortDetail);

            model.Employer = cohortDetail.LegalEntityName;
            model.Courses = courses;
        }

        private async Task<TrainingProgramme[]> GetCourses(GetCohortResponse cohortDetails)
        {
            var result = await _mediator.Send(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = (!cohortDetails.IsFundedByTransfer &&
                                     cohortDetails.LevyStatus != CommitmentsV2.Types.ApprenticeshipEmployerType.NonLevy)
                                    || cohortDetails.IsLinkedToChangeOfPartyRequest
            });

            return result.TrainingCourses;
        }

        private void StoreAddDraftApprenticeshipState(AddDraftApprenticeshipViewModel model)
        {
            TempData.Put(nameof(AddDraftApprenticeshipViewModel), model);
        }

        private AddDraftApprenticeshipViewModel PeekStoredAddDraftApprenticeshipState()
        {
            return TempData.GetButDontRemove<AddDraftApprenticeshipViewModel>(nameof(AddDraftApprenticeshipViewModel));
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

            if (hasStandardOptions)
            {
                return RedirectToAction("SelectOptions", "DraftApprenticeship", routeValues);
            }
            else
            {
                return RedirectToAction("Details", "Cohort", routeValues);
            }
        }

        private async Task<bool> HasStartDateOverlap(DraftApprenticeshipViewModel model)
        {
            if (model.StartDate.Date.HasValue && model.EndDate.Date.HasValue && !string.IsNullOrWhiteSpace(model.Uln))
            {
                var apimRequest = await _modelMapper.Map<ValidateDraftApprenticeshipApimRequest>(model);
                await _outerApiService.ValidateDraftApprenticeshipForOverlappingTrainingDateRequest(apimRequest);

                var result = await _commitmentsApiClient.ValidateUlnOverlap(new ValidateUlnOverlapRequest
                {
                    EndDate = model.EndDate.Date.Value,
                    StartDate = model.StartDate.Date.Value,
                    ULN = model.Uln,
                });

                return result.HasOverlappingStartDate && !result.HasOverlappingEndDate;
            }

            return false;
        }

        private void RemoveStoredDraftApprenticeshipState()
        {
            TempData.Remove(nameof(AddDraftApprenticeshipViewModel));
        }
    }
}