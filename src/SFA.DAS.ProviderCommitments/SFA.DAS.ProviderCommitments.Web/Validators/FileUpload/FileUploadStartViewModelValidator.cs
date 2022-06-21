using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
    {
        private readonly ILogger<FileUploadStartViewModelValidator> _logger;
        private readonly BulkUploadFileValidationConfiguration _csvConfiguration;

        public FileUploadStartViewModelValidator(ILogger<FileUploadStartViewModelValidator> logger, BulkUploadFileValidationConfiguration csvConfiguration)
        {
            _logger = logger;
            _csvConfiguration = csvConfiguration;

            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Attachment)
                .NotNull().WithMessage("Select a file to upload")
                .Must(CheckFileSize).WithMessage($"The selected file must be smaller than {_csvConfiguration.MaxBulkUploadFileSize}KB")
                .Must(CheckFileType).WithMessage("The selected file must be a CSV")
                .MustAsync(CheckEmptyFileContent).WithMessage("The selected file is empty")
                .MustAsync(CheckFileColumnCount).WithMessage("The selected file could not be uploaded – use the template")
                .MustAsync(CheckColumnHeader).WithMessage("One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this")
                .MustAsync(CheckApprenticeContent).WithMessage("The selected file does not contain apprentice details")
                .MustAsync(CheckFileRowCount).WithMessage($"The selected file must be less than {_csvConfiguration.MaxAllowedFileRowCount} lines");
        }

        private bool CheckFileSize(IFormFile file)
        {
            var maxFileSize = _csvConfiguration.MaxBulkUploadFileSize * 1024; // Bytes
            return (file.Length <= maxFileSize);
        }

        private bool CheckFileType(IFormFile file)
        {
            return file.FileName.ToLower().EndsWith(".csv");
        }

        private async Task<bool> CheckEmptyFileContent(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return fileData.rowCount > 0;
        }

        private async Task<bool> CheckApprenticeContent(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return fileData.rowCount > 1;
        }

        private async Task<bool> CheckFileColumnCount(IFormFile file, CancellationToken cancellation)
        {
            var (firstlineData, _) = await ReadFileAsync(file);
            return BulkUploadFileRequirements.CheckHeaderCount(firstlineData);
        }

        private async Task<bool> CheckFileRowCount(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return fileData.rowCount <= _csvConfiguration.MaxAllowedFileRowCount;
        }

        private async Task<bool> CheckColumnHeader(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return (fileData.firstlineData[0] == "CohortRef" &&
                    fileData.firstlineData[1] == "AgreementID" &&
                    fileData.firstlineData[2] == "ULN" &&
                    fileData.firstlineData[3] == "FamilyName" &&
                    fileData.firstlineData[4] == "GivenNames" &&
                    fileData.firstlineData[5] == "DateOfBirth" &&
                    fileData.firstlineData[6] == "EmailAddress" &&
                    fileData.firstlineData[7] == "StdCode" &&
                    fileData.firstlineData[8] == "StartDate" &&
                    fileData.firstlineData[9] == "EndDate" &&
                    fileData.firstlineData[10] == "TotalPrice" &&
                    fileData.firstlineData[11] == "EPAOrgID" &&
                    fileData.firstlineData[12] == "ProviderRef");            
        }

        private async Task<(string[] firstlineData, int rowCount)> ReadFileAsync(IFormFile file)
        {
            try
            {
                var firstLineData = Array.Empty<string>();

                var fileContent = new StreamReader(file.OpenReadStream()).ReadToEnd();
                using (var reader = new StringReader(fileContent))
                {
                    int lineCounter = 0;

                    while (reader.Peek() >= 0)
                    {
                        string lineContent = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(lineContent))
                        {
                            if (firstLineData.Length == 0)
                            {
                                firstLineData = lineContent.Split(',');
                                lineCounter++;

                            }
                            else 
                            {
                                var lineContents = lineContent.Split(',');
                                if (!IsEmptyRow(lineContents))
                                {
                                    lineCounter++;
                                }
                            }
                        }
                    }

                    return (firstLineData, lineCounter);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to process bulk upload file");
                throw exc;
            }
        }

        private static bool IsEmptyRow(string[] lineContents)
        {
            return 
                (lineContents.Length == 1 && string.IsNullOrEmpty(lineContents[0]))
                || 
                (lineContents.Length == 13 &&
                                                 string.IsNullOrWhiteSpace(lineContents[0]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[1]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[2]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[3]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[4]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[5]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[6]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[7]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[8]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[9]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[10]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[11]) &&
                                                 string.IsNullOrWhiteSpace(lineContents[12]));
        }
    }
}