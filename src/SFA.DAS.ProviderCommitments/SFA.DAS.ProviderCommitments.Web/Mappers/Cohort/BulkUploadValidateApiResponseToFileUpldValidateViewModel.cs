﻿using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class BulkUploadValidateApiResponseToFileUpldValidateViewModel : IMapper<BulkUploadValidateApiResponse, FileUploadValidateViewModel>
    {
        public Task<FileUploadValidateViewModel> Map(BulkUploadValidateApiResponse source)
        {
            var viewModel = new FileUploadValidateViewModel();
            foreach (var sourceError in source.BulkUploadValidationErrors)
            {
                var validationError = new Models.Cohort.BulkUploadValidationError();
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