using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Controllers
{
    [Route("v2/{providerId}/apprentices")]
    public class ManageApprenticesController : Controller
    {
        private readonly ICommitmentsService _commitmentsService;

        public ManageApprenticesController(ICommitmentsService commitmentsService)
        {
            _commitmentsService = commitmentsService;
        }

        [Route("all")]
        public async Task<IActionResult> Index(uint providerId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new ManageApprenticesViewModel
            {
                ProviderId = providerId,
                Apprenticeships = await _commitmentsService.GetApprovedApprenticeships(providerId)
            };

            return View(model);
        }

        [HttpGet]
        [Route("download",Name = "Download")]
        public async Task<IActionResult> Download(uint providerId)
        {
            var result = await _commitmentsService.GetApprovedApprenticeships(providerId);
            return CreateCsvStream(result, "Manageyourapprentices");
        }

        private ActionResult CreateCsvStream<T>(IEnumerable<T> results, string fileNamePreFix)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.WriteRecords(results);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "text/csv", $"{fileNamePreFix}_{DateTime.Now:yyyyMMddhhmmss}.csv");
                    }
                }
            }
        }
    }
}