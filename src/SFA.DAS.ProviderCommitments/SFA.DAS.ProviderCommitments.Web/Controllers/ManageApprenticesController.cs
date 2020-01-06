using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices",Name = "index")]
    public class ManageApprenticesController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;
        private readonly ICreateCsvService _createCsvService;

        public ManageApprenticesController(ICommitmentsService commitmentsService, ICreateCsvService createCsvService)
        {
            _commitmentsService = commitmentsService;
            _createCsvService = createCsvService;
        }

        public async Task<IActionResult> Index(uint providerId, string sortField = "", bool isReversed = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new ManageApprenticesViewModel{ProviderId = providerId, SortField = sortField, IsReversed = isReversed};
            if (sortField == "")
            {
                model.Apprenticeships = await _commitmentsService.GetApprenticeships(providerId);
            }
            else
            {
                model.Apprenticeships = await _commitmentsService.GetApprenticeships(providerId, sortField, isReversed);
            }
            
            return View(model);
        }

        [HttpGet]
        [Route("download",Name = "Download")]
        public async Task<IActionResult> Download(uint providerId)
        {
            var result = await _commitmentsService.GetApprenticeships(providerId);

            var csvContent = result.Select(c => (ApprenticeshipDetailsCsvViewModel)c).ToList();
            
            var csvFileContent = _createCsvService.GenerateCsvContent(csvContent);
            return File(csvFileContent, "text/csv", $"{"Manageyourapprentices"}_{DateTime.Now:yyyyMMddhhmmss}.csv");
        }
    }
}