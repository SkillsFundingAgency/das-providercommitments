using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public sealed class BulkUploader
    {
      //  private readonly IMediator _mediator;
        //private readonly IBulkUploadValidator _bulkUploadValidator;
        //private readonly IProviderCommitmentsLogger _logger;
        private readonly BulkUploadFileParser _fileParser;
        private readonly IEncodingService _encodingService;

        //public BulkUploader(IMediator mediator, IBulkUploadValidator bulkUploadValidator)
        public BulkUploader(IEncodingService encodingService)
        {
            //_mediator = mediator;
            //_bulkUploadValidator = bulkUploadValidator;
            _fileParser = new BulkUploadFileParser(encodingService);
            _encodingService = encodingService;
            //_logger = logger;
        }

        //  public async Task<BulkUploadResult> ValidateFileRows(IEnumerable<ApprenticeshipUploadModel> rows, long providerId, long bulkUploadId)
        public BulkUploadResult ValidateFileRows(IEnumerable<ApprenticeshipUploadModel> rows, long providerId, long bulkUploadId)
        {
            //var trainingProgrammes = await GetTrainingProgrammes();
            //var validationErrors = _bulkUploadValidator.ValidateRecords(rows, trainingProgrammes).ToList();

            //if (validationErrors.Any())
            //{
            //    var logtext = new StringBuilder();
            //    logtext.AppendLine($"Failed validation of bulk upload id {bulkUploadId} with {validationErrors.Count} errors");

            //    var errorTypes = validationErrors.GroupBy(x => x.ErrorCode);
            //    foreach (var errorType in errorTypes)
            //    {
            //        var errorsOfType = validationErrors.FindAll(x => x.ErrorCode == errorType.Key);
            //        logtext.AppendLine($"{errorsOfType.Count} x {errorType.Key} - \"{StripHtml(errorsOfType.First().Message)}\"");
            //    }

            //    _logger.Warn(logtext.ToString(), providerId);

            //    return new BulkUploadResult { Errors = validationErrors };
            //}

            return new BulkUploadResult { Errors = new List<UploadError>(), Data = rows };
        }

        //  public async Task<BulkUploadResult> ValidateFileStructure(BulkUploadViewModel uploadApprenticeshipsViewModel, long providerId)
        public BulkUploadResult ValidateFileStructure(BulkUploadViewModel uploadApprenticeshipsViewModel, long providerId)
        {
            if (uploadApprenticeshipsViewModel.Attachment == null)
                return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("No file chosen") } };

            var fileContent = new StreamReader(uploadApprenticeshipsViewModel.Attachment.OpenReadStream()).ReadToEnd();
            var fileName = uploadApprenticeshipsViewModel?.Attachment?.FileName ?? "<- NO NAME ->";

            //_logger.Trace($"Saving bulk upload file. {fileName}");
            //var bulkUploadId = await _mediator.Send(
            //    new SaveBulkUploadFileCommand
            //    {
            //        ProviderId = uploadApprenticeshipsViewModel.ProviderId,
            //        CommitmentId = commitment.Id,
            //        FileContent = fileContent,
            //        FileName = fileName
            //    });
            //_logger.Info($"Saved bulk upload with Id: {bulkUploadId}");

            //var fileAttributeErrors = _bulkUploadValidator.ValidateFileSize(uploadApprenticeshipsViewModel.Attachment).ToList();

            //if (fileAttributeErrors.Any())
            //{
            //    foreach (var error in fileAttributeErrors)
            //        _logger.Warn($"File Structure Error  -->  {error.Message}", uploadApprenticeshipsViewModel.ProviderId, commitment.Id);

            //    _logger.Info($"Failed validation bulk upload file with {fileAttributeErrors.Count} errors", uploadApprenticeshipsViewModel.ProviderId, commitment.Id);

            //    return new BulkUploadResult { Errors = fileAttributeErrors };
            //}

            //var uploadResult = _fileParser.CreateViewModels(providerId, commitment, fileContent);
            var uploadResult = _fileParser.CreateViewModels(providerId, fileContent, _encodingService);

            if (uploadResult.HasErrors)
                return uploadResult;

            //var errors = _bulkUploadValidator.ValidateCohortReference(uploadResult.Data, uploadApprenticeshipsViewModel.HashedCommitmentId).ToList();
            //errors.AddRange(_bulkUploadValidator.ValidateUlnUniqueness(uploadResult.Data).ToList());

            return new BulkUploadResult
            {
                Errors = new List<UploadError>(),
                Data = uploadResult.Data,
                BulkUploadId = 1 // dummy id for the spike
            };
        }

        //private async Task<List<TrainingProgramme>> GetTrainingProgrammes()
        //{
        //    var programmes = await _mediator.Send(new GetTrainingProgrammesQueryRequest
        //    {
        //        IncludeFrameworks = true
        //    });
        //    return programmes.TrainingProgrammes;
        //}

        private string StripHtml(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }

    public class BulkUploadResult
    {
        public IEnumerable<UploadError> Errors { get; set; } = new List<UploadError>();

        public IEnumerable<ApprenticeshipUploadModel> Data { get; set; }

        public bool HasErrors => (Errors != null && Errors.Any());

        public long BulkUploadId { get; set; }
    }

    public sealed class BulkUploadValidator 
    {
        public BulkUploadValidator()
        {

        }

        public IEnumerable<UploadError> ValidateFileSize(IFormFile attachment)
        {
            var errors = new List<UploadError>();
            var maxFileSize = 500 * 1024; // Bytes

            if (attachment.Length > maxFileSize)
                errors.Add(new UploadError("more then the allowed size"));

            if (!attachment.FileName.ToLower().EndsWith(".csv"))
                errors.Add(new UploadError("It can only be CSV file"));

            return errors;
        }

        //public IEnumerable<UploadError> ValidateCohortReference(
        //    IEnumerable<ApprenticeshipUploadModel> records,
        //    string cohortReference)
        //{
        //    var errors = new List<UploadError>();

        //    var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();
        //    if (!apprenticeshipUploadModels.Any()) return new[] { new UploadError("No records found") };

        //    //if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != apprenticeshipUploadModels.First().CsvRecord.CohortRef))
        //    //    errors.Add(new UploadError(_validationText.CohortRef01.Text.RemoveHtmlTags(), _validationText.CohortRef01.ErrorCode));

        //    //if (apprenticeshipUploadModels.Any(m => m.CsvRecord.CohortRef != cohortReference))
        //    //    errors.Add(new UploadError(_validationText.CohortRef02.Text.RemoveHtmlTags(), _validationText.CohortRef02.ErrorCode));

        //    if (apprenticeshipUploadModels.Length != apprenticeshipUploadModels.Distinct(m => m.ApprenticeshipViewModel.ULN).Count())
        //        errors.Add(new UploadError(_validationText.Uln04.Text.RemoveHtmlTags(), _validationText.Uln04.ErrorCode));

        //    return errors;
        //}

        //public IEnumerable<UploadError> ValidateUlnUniqueness(IEnumerable<ApprenticeshipUploadModel> records)
        //{
        //    var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();

        //    var result = new List<UploadError>();

        //    var distinctUlns = apprenticeshipUploadModels.Select(x => x.ApprenticeshipViewModel.ULN).Distinct().Count();

        //    if (apprenticeshipUploadModels.Count() != distinctUlns)
        //    {
        //        result.Add(new UploadError(_validationText.Uln04.Text.RemoveHtmlTags(), _validationText.Uln04.ErrorCode));
        //    }

        //    return result;
        //}

        //public IEnumerable<UploadError> ValidateRecords(IEnumerable<ApprenticeshipUploadModel> records, List<TrainingProgramme> trainingProgrammes)
        //{
        //    var errors = new ConcurrentBag<UploadError>();
        //    var apprenticeshipUploadModels = records as ApprenticeshipUploadModel[] ?? records.ToArray();

        //    Parallel.ForEach(apprenticeshipUploadModels,
        //        (record, state, index) =>
        //        {
        //            var viewModel = record.ApprenticeshipViewModel;
        //            int i = (int)index + 1;

        //            // Validate view model for approval
        //            var validationResult = _viewModelValidator.Validate(record);
        //            validationResult.Errors.ForEach(m => errors.Add(new UploadError(m.ErrorMessage, m.ErrorCode, i, record)));

        //            var validationMessage = ValidateTrainingInConjunctionWithStartDate(viewModel, trainingProgrammes);
        //            if (validationMessage != null)
        //                errors.Add(new UploadError(validationMessage.Value.Text, validationMessage.Value.ErrorCode, i, record));
        //        });

        //    return errors;
        //}

        //private ValidationMessage? ValidateTrainingInConjunctionWithStartDate(ApprenticeshipViewModel viewModel, List<TrainingProgramme> trainingProgrammes)
        //{
        //    //todo: the validation messages belong in BulkUploadApprenticeshipValidationText (IApprenticeshipValidationErrorText), but...
        //    // the validationtext classes already contain a CourseCode01 but that has an errorCode of "DefaultErrorCode"
        //    // and the "Training_01" errorCode below already existed, and we can't change existing error codes, as external systems will probably rely on them.
        //    // also the different implementations of IApprenticeshipValidationErrorText contain their own subset of error messages so not entirely convinced we need the interface
        //    // and it's not injected as a dependency either, so might be best to just have seperate centralised validation message containers
        //    // but don't want to tackle it as a refactor now as the risk/reward ratio is not good

        //    if (!string.IsNullOrWhiteSpace(viewModel.CourseCode))
        //    {
        //        // not as safe as single, but quicker
        //        var trainingProgram = trainingProgrammes.Find(tp => tp.CourseCode == viewModel.CourseCode);
        //        if (trainingProgram == null)
        //            return new ValidationMessage("Not a valid <strong>Training code</strong>", "Training_01");

        //        if (viewModel.StartDate?.DateTime != null)
        //        {
        //            var courseStatus = trainingProgram.GetStatusOn(viewModel.StartDate.DateTime.Value);

        //            if (courseStatus != TrainingProgrammeStatus.Active)
        //            {
        //                // if EffectiveFrom is null, then programme is valid to the big bang, so won't be pending, so we don't have to check for null (similar for expired also)
        //                var suffix = courseStatus == TrainingProgrammeStatus.Pending
        //                    ? $"after {trainingProgram.EffectiveFrom.Value.AddMonths(-1):MM yyyy}"
        //                    : $"before {trainingProgram.EffectiveTo.Value.AddMonths(1):MM yyyy}";

        //                return new ValidationMessage(_validationText.LearnStartDateNotValidForTrainingCourse, suffix);
        //            }
        //        }
        //    }

        //    return null;
        //}
    }

    [Serializable]
    public class UploadError
    {
        public UploadError(string message)
        {
            Message = message;
            ErrorCode = string.Empty;
        }

        public UploadError(string message, string errorCode, int? index = null, ApprenticeshipUploadModel record = null)
        {
            Message = message;
            ErrorCode = errorCode;
            Row = index;
            IsGeneralError = index == null || record == null;
            Uln = record?.ApprenticeshipViewModel?.ULN;
            FirstName = record?.ApprenticeshipViewModel?.FirstName;
            LastName = record?.ApprenticeshipViewModel?.LastName;
            DateOfBirth = record?.ApprenticeshipViewModel?.DateOfBirth;
        }

        public bool IsGeneralError { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Message { get; set; }

        public string ErrorCode { get; set; }

        public int? Row { get; set; }

        public string Uln { get; set; }

        public override string ToString()
        {
            if (Row.HasValue)
                return $"Row:{Row} - {Message}";
            return $"{Message}";
        }
    }

    public class ApprenticeshipUploadModel
    {
        public ApprenticeshipViewModel ApprenticeshipViewModel { get; set; }

        public CsvRecord CsvRecord { get; set; }
    }

    public class ApprenticeshipViewModel
    {
        private const int CurrentYearAsTwoDigitOffSet = 0;

        public ApprenticeshipViewModel()
        {
            StartDate = new DateTime();
            EndDate = new DateTime();
        }

        public string HashedApprenticeshipId { get; set; }

        public string HashedCommitmentId { get; set; }

        public long ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string NINumber { get; set; }

        public string ULN { get; set; }

        public ProgrammeType CourseType { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Cost { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? EndDate { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public string ProviderRef { get; set; }
        public string EmployerRef { get; set; }

        public int? ProgType { get; set; }

        public bool HasStarted { get; set; }

        public bool IsLockedForUpdate { get; set; }
        public bool IsPaidForByTransfer { get; set; }
        public bool IsUpdateLockedForStartDateAndCourse { get; set; }
        public bool IsEndDateLockedForUpdate { get; set; }
        public string StartDateTransfersMinDateAltDetailMessage { get; set; }
        public Guid? ReservationId { get; set; }
        public bool IsContinuation { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }

        public string CohortRef { get; set; }
    }

    public class CsvRecord
    {
        public string CohortRef { get; set; }

        public string ULN { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string DateOfBirth { get; set; }

        public string ProgType { get; set; }

        public string FworkCode { get; set; }

        public string PwayCode { get; set; }

        public string StdCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string TotalPrice { get; set; }

        public string EPAOrgID { get; set; }  // ToDO: Validate Startwith EPA...

        public string ProviderRef { get; set; }

        public string AgreementId { get; set; }
    }

    public sealed class BulkUploadFileParser 
    {
        private IEncodingService _encodingService;

        //private readonly IProviderCommitmentsLogger _logger;

        public BulkUploadFileParser(IEncodingService encodingService)
        {
            _encodingService = encodingService;

            //_logger = logger;
        }

        public BulkUploadResult CreateViewModels(long providerId, string fileInput, IEncodingService encodingService)
        {
            const string errorMessage = "Upload failed. Please check your file and try again.";

            using (var tr = new StringReader(fileInput))
            {
                try
                {
                    var csvReader = new CsvReader(tr);
                    csvReader.Configuration.HasHeaderRecord = true;
                    //csvReader.Configuration.IsHeaderCaseSensitive = false;
                    csvReader.Configuration.PrepareHeaderForMatch = (header, index) => header.ToLower();
                    csvReader.Configuration.BadDataFound = cont => throw new Exception("Bad data found");
                    csvReader.Configuration.RegisterClassMap<CsvRecordMap>();

                    return new BulkUploadResult
                    {
                        Data = csvReader.GetRecords<CsvRecord>()
                            .ToList()
                            .Select(record => MapTo(record))
                    };
                }
               // catch (Exception exce)
               // {
               ////     _logger.Info("Failed to process bulk upload file (missing field).", providerId, commitment.Id);
               //     return new BulkUploadResult { Errors = new List<UploadError> { new UploadError("Some mandatory fields are incomplete. Please check your file and upload again.") } };
               // }
                catch (Exception)
                {
                    //_logger.Info("Failed to process bulk upload file.", providerId, commitment.Id);

                    return new BulkUploadResult { Errors = new List<UploadError> { new UploadError(errorMessage) } };
                }
            }
        }

        private ApprenticeshipUploadModel MapTo(CsvRecord record)
        {
            var dateOfBirth = GetValidDate(record.DateOfBirth, "yyyy-MM-dd");
            var learnerStartDate = GetValidDate(record.StartDate, "yyyy-MM");
            var learnerEndDate = GetValidDate(record.EndDate, "yyyy-MM");

            var courseCode = record.ProgType == "25"
                                   ? record.StdCode
                                   : $"{record.FworkCode}-{record.ProgType}-{record.PwayCode}";

            var apprenticeshipViewModel = new ApprenticeshipViewModel
            {
                AgreementStatus = AgreementStatus.NotAgreed,
                PaymentStatus = PaymentStatus.Active,
                ULN = record.ULN,
                FirstName = record.GivenNames,
                LastName = record.FamilyName,
                DateOfBirth = dateOfBirth,
                Cost = record.TotalPrice,
                ProviderRef = record.ProviderRef,
                StartDate = learnerStartDate,
                EndDate = learnerEndDate,
                ProgType = int.Parse(record.ProgType),
                CourseCode = courseCode,
                LegalEntityId = _encodingService.Decode(record.AgreementId, EncodingType.PublicAccountLegalEntityId),
                CohortRef = record.CohortRef
            };
            return new ApprenticeshipUploadModel
            {
                ApprenticeshipViewModel = apprenticeshipViewModel,
                CsvRecord = record
            };
        }

        private DateTime? GetValidDate(string date, string format)
        {
            DateTime outDateTime;
            if (DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDateTime))
                return outDateTime;
            return null;
        }
    }

    public sealed class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            AutoMap();
        }
    }
}
