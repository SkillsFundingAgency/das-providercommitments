using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization.ProviderPermissions.Options;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderUrlHelper;
using SFA.DAS.ProviderCommitments.Features;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.ProviderCommitments.Web.Authentication;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.LocalDevRegistry;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SFA.DAS.Encoding;
using Newtonsoft.Json;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("{providerId}/unapproved")]
    public class CohortController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IModelMapper _modelMapper;
        private readonly ILinkGenerator _urlHelper;
        private readonly ICommitmentsApiClient _commitmentApiClient;
        private readonly IEncodingService _encodingService;

        public CohortController(IMediator mediator,
            IModelMapper modelMapper,
            ILinkGenerator urlHelper,
            ICommitmentsApiClient commitmentsApiClient,
            IEncodingService encodingService)
        {
            _mediator = mediator;
            _modelMapper = modelMapper;
            _urlHelper = urlHelper;
            _commitmentApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        [HttpGet]
        public async Task<IActionResult> Cohorts(CohortsByProviderRequest request)
        {
            var model = await _modelMapper.Map<CohortsViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("review")]
        public async Task<IActionResult> Review(CohortsByProviderRequest request)
        {
            var reviewViewModel = await _modelMapper.Map<ReviewViewModel>(request);
            return View(reviewViewModel);
        }

        [HttpGet]
        [Route("draft")]
        public async Task<IActionResult> Draft(CohortsByProviderRequest request)
        {
            var draftViewModel = await _modelMapper.Map<DraftViewModel>(request);
            return View(draftViewModel);
        }

        [HttpGet]
        [Route("with-employer")]
        public async Task<IActionResult> WithEmployer(CohortsByProviderRequest request)
        {
            var withEmployerViewModel = await _modelMapper.Map<WithEmployerViewModel>(request);
            return View(withEmployerViewModel);
        }

        [HttpGet]
        [Route("with-transfer-sender")]
        public async Task<IActionResult> WithTransferSender(CohortsByProviderRequest request)
        {
            var withTransferSenderViewModel = await _modelMapper.Map<WithTransferSenderViewModel>(request);
            return View(withTransferSenderViewModel);
        }

        [HttpGet]
        [Route("add-apprentice")]
        [Route("add/apprentice")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
        {
            var model = await _modelMapper.Map<AddDraftApprenticeshipViewModel>(request);
            return View(model);
        }

        [HttpPost]
        [Route("add-apprentice")]
        [Route("add/apprentice")]
        [DasAuthorize(ProviderOperation.CreateCohort)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            var request = await _modelMapper.Map<CreateCohortRequest>(model);

            var response = await _mediator.Send(request);
            var cohortDetailsUrl = $"{model.ProviderId}/apprentices/{response.CohortReference}/Details";
            var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
            return Redirect(url);
        }

        [HttpGet]
        [Route("add/select-employer")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> SelectEmployer(SelectEmployerRequest request)
        {
            var model = await _modelMapper.Map<SelectEmployerViewModel>(request);
            return View(model);
        }

        [HttpGet]
        [Route("add/confirm-employer")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerRequest request)
        {
            var model = await _modelMapper.Map<ConfirmEmployerViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("add/confirm-employer")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> ConfirmEmployer(ConfirmEmployerViewModel viewModel)
        {
            if (viewModel.Confirm.Value)
            {
                var request = await _modelMapper.Map<CommitmentsV2.Api.Types.Requests.CreateEmptyCohortRequest>(viewModel);
                var response = await _commitmentApiClient.CreateCohort(request);

                var cohortDetailsUrl = $"{viewModel.ProviderId}/apprentices/{response.CohortReference}/Details";
                var url = _urlHelper.ProviderApprenticeshipServiceLink(cohortDetailsUrl);
                return Redirect(url);
            }

            return RedirectToAction("SelectEmployer", new { viewModel.ProviderId });
        }



        [HttpGet]
        [Route("add/bulk-upload")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Bulkupload(BulkUploadRequest request)
        {
            var model = await _modelMapper.Map<BulkUploadViewModel>(request);

            return View(model);
        }

        [HttpPost]
        [Route("add/bulk-upload")]
        [DasAuthorize(ProviderFeature.ProviderCreateCohortV2)]
        [Authorize(Policy = nameof(PolicyNames.HasContributorOrAbovePermission))]
        public async Task<IActionResult> Bulkupload(BulkUploadViewModel viewModel)
        {
            BulkUploader bulkUploader = new BulkUploader(_encodingService);
            var uploadFile = bulkUploader.ValidateFileStructure(viewModel, viewModel.ProviderId);

            var client = (_commitmentApiClient as LocalDevRegistry.CommitmentsApiClient2);

           var request = new BulkUploadValidatorRequest();
            request.BulkUploadApprenticeships = new List<BulkUploadApprenticeship>();

            foreach (var app in uploadFile.Data)
            {

                var apprenticeship = new BulkUploadApprenticeship();
                apprenticeship.LastName = app.ApprenticeshipViewModel.LastName;
                apprenticeship.FirstName = app.ApprenticeshipViewModel.FirstName;
                apprenticeship.DateOfBirth = app.ApprenticeshipViewModel.DateOfBirth;
                apprenticeship.StartDate = app.ApprenticeshipViewModel.StartDate;
                apprenticeship.EndDate = app.ApprenticeshipViewModel.EndDate;
                apprenticeship.LastName = app.ApprenticeshipViewModel.LastName;
                apprenticeship.Cost = app.ApprenticeshipViewModel.Cost;
                apprenticeship.CourseCode = app.ApprenticeshipViewModel.CourseCode;
                apprenticeship.ULN = app.ApprenticeshipViewModel.ULN;
                apprenticeship.LegalEntityId = app.ApprenticeshipViewModel.LegalEntityId;
                apprenticeship.ProviderId = viewModel.ProviderId;
                apprenticeship.CohortRef = app.ApprenticeshipViewModel.CohortRef;

                request.BulkUploadApprenticeships.Add(apprenticeship);
            }


           var result = await client.BulkUpload(request, CancellationToken.None);

            TempData["UploadResult"] = JsonConvert.SerializeObject(result);

            return await Task.FromResult(RedirectToAction("BulkuploadSummary", new { viewModel.ProviderId }));
        }

        [HttpGet]
        [Route("add/bulk-upload-summary")]
       
        public IActionResult BulkuploadSummary(BulkUploadSummaryRequest summary)
        {
            object o;
            TempData.TryGetValue("UploadResult", out o);
            var bulkuploadResponse = JsonConvert.DeserializeObject<BulkUploadResponse>((string)o);
           
                var result = new BulkUploadSummaryViewModel
            {
                ProviderId = summary.ProviderId,
                BulkUploadResponse =bulkuploadResponse
            };

            TempData["UploadResult"] = TempData["UploadResult"];

            return View(result);
        }

        [HttpPost]
        [Route("add/bulk-upload-summary")]

        public async Task<IActionResult> BulkuploadSummary(BulkUploadSummaryViewModel summary)
        {
            object o;
            TempData.TryGetValue("UploadResult", out o);
            var bulkuploadResponse = JsonConvert.DeserializeObject<BulkUploadResponse>((string)o);
            TempData["UploadResult"] = TempData["UploadResult"];

            //var result = new BulkUploadSummaryViewModel
            //{
            //    ProviderId = summary.ProviderId,
            //    BulkUploadResponse = bulkuploadResponse
            //};

            var cohortIds = bulkuploadResponse.Results.Select(x => x.CohortId);

            var request = new BulkCohortActionRequest
            {
                CohortAction = summary.CohortAction,
                CohortIds = cohortIds.ToList()
            };

            var client = (_commitmentApiClient as LocalDevRegistry.CommitmentsApiClient2);

            await client.BulkAction(request, CancellationToken.None);

            return RedirectToAction("Cohorts", new { summary.ProviderId });
        }
    }
}