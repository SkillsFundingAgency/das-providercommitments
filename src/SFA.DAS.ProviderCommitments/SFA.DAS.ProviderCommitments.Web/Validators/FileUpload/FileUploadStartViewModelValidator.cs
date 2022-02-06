using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Validators
{
    public class FileUploadStartViewModelValidator : AbstractValidator<FileUploadStartViewModel>
    {
        private const int MaxBulkUploadFileSize = 50000;
        private const int FileColumnCount = 13;
        private const int MaxAllowedFileRowCount = 100;

        private readonly ILogger<FileUploadStartViewModelValidator> _logger;

        public FileUploadStartViewModelValidator(ILogger<FileUploadStartViewModelValidator> logger)
        {
            _logger = logger;
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Attachment)
                .NotNull()
                .Must(CheckFileSize).WithMessage("The selected file must be smaller than 50KB")
                .Must(CheckFileType).WithMessage("The selected file must be a CSV")
                .MustAsync(CheckEmptyFileContent).WithMessage("The selected file is empty")
                .MustAsync(CheckFileColumnCount).WithMessage("The selected file could not be uploaded – use the template")
                .MustAsync(CheckFileRowCount).WithMessage("The selected file must be less than 100 lines");
        }

        private bool CheckFileSize(IFormFile file)
        {
            var maxFileSize = MaxBulkUploadFileSize * 1024; // Bytes
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

        private async Task<bool> CheckFileColumnCount(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return fileData.firstlineData.Length == FileColumnCount;
        }

        private async Task<bool> CheckFileRowCount(IFormFile file, CancellationToken cancellation)
        {
            var fileData = await ReadFileAsync(file);
            return fileData.rowCount <= MaxAllowedFileRowCount;
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
                        if (!string.IsNullOrWhiteSpace(lineContent) && firstLineData.Length == 0)
                        {
                            firstLineData = lineContent.Split(',');
                        }

                        lineCounter++;
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
    }
}