using FluentValidation;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

public class FileUploadValidationHelper
{
    private readonly BulkUploadFileValidationConfiguration _csvConfiguration;
    private const int EXTENDEDRPLCOLUMNCOUNT = 22;

    public FileUploadValidationHelper(BulkUploadFileValidationConfiguration config)
    {
        _csvConfiguration = config;
    }

    // The async methods have been refactored to be synchronous due to
    // https://docs.fluentvalidation.net/en/latest/upgrading-to-11.html#sync-over-async-now-throws-an-exception
    public void AddFileValidationRules(IRuleBuilderInitial<FileUploadStartViewModel, IFormFile> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Select a file to upload")
            .Must(CheckFileSize).WithMessage($"The selected file must be smaller than {_csvConfiguration.MaxBulkUploadFileSize}KB")
            .Must(CheckFileType).WithMessage("The selected file must be a CSV")
            .Must(CheckEmptyFileContent).WithMessage("The selected file is empty")
            .Must(CheckFileColumnCount).WithMessage("The selected file could not be uploaded – use the template")
            .Must(CheckColumnHeaders).WithMessage($"One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this")
            .Must(CheckApprenticeContent).WithMessage("The selected file does not contain apprentice details")
            .Must(CheckFileRowCount).WithMessage($"The selected file must be less than {_csvConfiguration.MaxAllowedFileRowCount} lines")
            .Must(CheckColumnHeaders).WithMessage($"One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this");
    }

    public void AddFileValidationRules(IRuleBuilderInitial<FileUploadValidateViewModel, IFormFile> ruleBuilder)
    {
        ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Select a file to upload")
            .Must(CheckFileSize).WithMessage($"The selected file must be smaller than {_csvConfiguration.MaxBulkUploadFileSize}KB")
            .Must(CheckFileType).WithMessage("The selected file must be a CSV")
            .Must(CheckEmptyFileContent).WithMessage("The selected file is empty")
            .Must(CheckFileColumnCount).WithMessage("The selected file could not be uploaded – use the template")
            .Must(CheckColumnHeaders).WithMessage("One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this")
            .Must(CheckApprenticeContent).WithMessage("The selected file does not contain apprentice details")
            .Must(CheckFileRowCount).WithMessage($"The selected file must be less than {_csvConfiguration.MaxAllowedFileRowCount} lines");
    }

    private bool CheckFileSize(IFormFile file)
    {
        var maxFileSize = _csvConfiguration.MaxBulkUploadFileSize * 1024; // Bytes
        return file.Length <= maxFileSize;
    }

    private static bool CheckFileType(IFormFile file)
    {
        return file.FileName.ToLower().EndsWith(".csv");
    }

    private static bool CheckEmptyFileContent(IFormFile file)
    {
        var fileData = ReadFile(file);
        return fileData.rowCount > 0;
    }

    private static bool CheckApprenticeContent(IFormFile file)
    {
        var fileData = ReadFile(file);
        return fileData.rowCount > 1;
    }

    private static bool CheckFileColumnCount(IFormFile file)
    {
        var fileContent = new StreamReader(file.OpenReadStream()).ReadToEnd();
        using var reader = new StringReader(fileContent);

        var firstLine = reader.ReadLine();
        var firstlineData = firstLine.Split(',');

        return
            BulkUploadFileRequirements.CheckHeaderCount(firstlineData)
            && AllLinesHaveSameColumnCount(reader, firstlineData.Length);

        static bool AllLinesHaveSameColumnCount(TextReader reader, int count)
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Split(',').Length != count)
                {
                    return false;
                }
            }

            return true;
        }
    }

    private bool CheckFileRowCount(IFormFile file)
    {
        var fileData = ReadFile(file);
        return fileData.rowCount <= _csvConfiguration.MaxAllowedFileRowCount;
    }

    private static bool CheckColumnHeaders(IFormFile file)
    {
        var fileContent = new StreamReader(file.OpenReadStream()).ReadToEnd();
        using var reader = new StringReader(fileContent);

        var firstLine = reader.ReadLine();
        var firstlineData = firstLine.Split(',');

        if (!BulkUploadFileRequirements.HasMinimumRequiredColumns(firstlineData))
        {
            return false;
        }

        return firstlineData.Length != EXTENDEDRPLCOLUMNCOUNT;
    }

    private static (string[] firstlineData, int rowCount) ReadFile(IFormFile file)
    {
        var firstLineData = Array.Empty<string>();

        var fileContent = new StreamReader(file.OpenReadStream()).ReadToEnd();
        using var reader = new StringReader(fileContent);
        var lineCounter = 0;

        while (reader.Peek() >= 0)
        {
            var lineContent =  reader.ReadLine();
            if (string.IsNullOrWhiteSpace(lineContent))
            {
                continue;
            }

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

        return (firstLineData, lineCounter);
    }

    private static bool IsEmptyRow(IReadOnlyList<string> lineContents)
    {
        return
            (lineContents.Count == 1 && string.IsNullOrEmpty(lineContents[0]))
            ||
            (lineContents.Count == 13 &&
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