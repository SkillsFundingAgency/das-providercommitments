using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class BulkUploadValidateApiResponseToFileUpldValidateViewModel : IMapper<FileUploadValidateErrorRequest, FileUploadValidateViewModel>
    {
        private readonly ICacheService _cacheService;
        public BulkUploadValidateApiResponseToFileUpldValidateViewModel(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<FileUploadValidateViewModel> Map(FileUploadValidateErrorRequest source )
        {
            var errors = new List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError>();

            if (!string.IsNullOrWhiteSpace(source.CachedErrorGuid))
            {
                errors = await _cacheService.GetFromCache<List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError>>(source.CachedErrorGuid) ??
                    new List<Infrastructure.OuterApi.ErrorHandling.BulkUploadValidationError>();
                await _cacheService.ClearCache(source.CachedErrorGuid, nameof(BulkUploadValidateApiResponseToFileUpldValidateViewModel));
            }

            var viewModel = new FileUploadValidateViewModel
            {
                ProviderId = source.ProviderId
            };

            foreach (var sourceError in errors)
            {
                var validationError = new BulkUploadValidationError();
                validationError.EmployerName = sourceError.EmployerName;
                validationError.ApprenticeName = sourceError.ApprenticeName;
                validationError.RowNumber = sourceError.RowNumber;
                validationError.Uln = sourceError.Uln;
                foreach (var sError in sourceError.Errors)
                {
                    if (sError.Property == "DeclaredStandards") viewModel.HasNoDeclaredStandards = true;
                    var dError = new PropertyError()
                    {
                        ErrorText = sError.ErrorText,
                        Property = sError.Property
                    };

                    validationError.PropertyErrors.Add(dError);
                }

                viewModel.BulkUploadValidationErrors.Add(validationError);
            }

            return viewModel;
        }
    }
}
