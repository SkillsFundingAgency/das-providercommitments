using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadStartViewModelToBulkUploadRequestMapper : IMapper<FileUploadStartViewModel, BulkUploadAddDraftApprenticeshipsRequest>
    {
        private readonly IBulkUploadFileParser _fileParser;
        public FileUploadStartViewModelToBulkUploadRequestMapper(IBulkUploadFileParser fileParser)
        {
            _fileParser = fileParser;
        }

        public Task<BulkUploadAddDraftApprenticeshipsRequest> Map(FileUploadStartViewModel source)
        {
            var request = _fileParser.CreateApiRequest(source.ProviderId, source.Attachment);
            return Task.FromResult(request);
        }
    }
}
