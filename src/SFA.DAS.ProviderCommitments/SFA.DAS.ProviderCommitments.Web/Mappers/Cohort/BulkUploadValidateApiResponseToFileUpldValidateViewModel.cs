using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class BulkUploadValidateApiResponseToFileUpldValidateViewModel : IMapper<FileUploadValidateErrorRequest, FileUploadValidateViewModel>
    {
        public Task<FileUploadValidateViewModel> Map(FileUploadValidateErrorRequest source)
        {
            var viewModel = new FileUploadValidateViewModel();
            viewModel.ProviderId = source.ProviderId;

            foreach (var sourceError in source.Errors)
            {
                var validationError = new BulkUploadValidationError();
                validationError.EmployerName = sourceError.EmployerName;
                validationError.ApprenticeName = sourceError.ApprenticeName;
                validationError.RowNumber = sourceError.RowNumber;
                validationError.Uln = sourceError.Uln;
                foreach (var sError in sourceError.Errors)
                {
                    var dError = new PropertyError()
                    {
                        ErrorText = sError.ErrorText,
                        Property = sError.Property
                    };

                    validationError.PropertyErrors.Add(dError);
                }

                viewModel.BulkUploadValidationErrors.Add(validationError);
            }


            return Task.FromResult(viewModel);
        }
    }
}
