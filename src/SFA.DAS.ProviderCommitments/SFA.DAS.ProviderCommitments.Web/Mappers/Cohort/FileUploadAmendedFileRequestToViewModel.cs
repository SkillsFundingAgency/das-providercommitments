using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadAmendedFileRequestToViewModel : IMapper<FileUploadAmendedFileRequest, FileUploadAmendedFileViewModel>
    {
        public Task<FileUploadAmendedFileViewModel> Map(FileUploadAmendedFileRequest source)
        {
            return Task.FromResult(new FileUploadAmendedFileViewModel
            {
                CacheRequestId = source.CacheRequestId,
                ProviderId = source.ProviderId
            }); 
        }
    }
}
