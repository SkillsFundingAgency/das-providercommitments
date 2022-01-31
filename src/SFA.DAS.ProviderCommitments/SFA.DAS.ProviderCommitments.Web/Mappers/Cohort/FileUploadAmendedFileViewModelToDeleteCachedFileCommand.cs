using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Application.Commands.BulkUpload;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class FileUploadAmendedFileViewModelToDeleteCachedFileCommand : IMapper<FileUploadAmendedFileViewModel, DeleteCachedFileCommand>
    {
        public Task<DeleteCachedFileCommand> Map(FileUploadAmendedFileViewModel source)
        {
            return Task.FromResult(new DeleteCachedFileCommand
            {
                CachedRequestId = source.CacheRequestId
            }); 
        }
    }
}
