using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.CommitmentPermissions.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Features;
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

        public const string DraftApprenticeDeleted = "Apprentice record deleted";

        public DraftApprenticeshipController(IMediator mediator, ICommitmentsApiClient commitmentsApiClient,
            IModelMapper modelMapper, IEncodingService encodingService,
            SFA.DAS.Authorization.Services.IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _commitmentsApiClient = commitmentsApiClient;
            _modelMapper = modelMapper;
            _encodingService = encodingService;
            _authorizationService = authorizationService;
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
            var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(request);

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
        public async Task<IActionResult> SetDeliveryModel(SelectDeliveryModelViewModel model)
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
            var model = await _modelMapper.Map<SelectDeliveryModelViewModel>(draft);

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
        public async Task<IActionResult> SetDeliveryModelForEdit(SelectDeliveryModelViewModel model)
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

            var request = await _modelMapper.Map<AddDraftApprenticeshipRequest>(model);
            request.UserId = User.Upn();

            var response = await _commitmentsApiClient.AddDraftApprenticeship(model.CohortId.Value, request);

            if (string.IsNullOrEmpty(model.CourseCode))
            {
                return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });    
            }
            
            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, response.DraftApprenticeshipId);
            
            if (draftApprenticeship.HasStandardOptions)
            {
                var draftApprenticeshipHashedId = _encodingService.Encode(draftApprenticeship.Id, EncodingType.ApprenticeshipId);
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new {model.ProviderId, draftApprenticeshipHashedId , model.CohortReference});
            }
            
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
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

            var updateRequest = await _modelMapper.Map<UpdateDraftApprenticeshipRequest>(model);
            await _commitmentsApiClient.UpdateDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value, updateRequest);

            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(model.CohortId.Value, model.DraftApprenticeshipId.Value);
            
            if (draftApprenticeship.HasStandardOptions)
            {
                return RedirectToAction("SelectOptions", "DraftApprenticeship", new {model.ProviderId, model.DraftApprenticeshipHashedId, model.CohortReference});
            }
            
            return RedirectToAction("Details", "Cohort", new { model.ProviderId, model.CohortReference });
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
                                     cohortDetails.LevyStatus != ApprenticeshipEmployerType.NonLevy)
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
    }
}