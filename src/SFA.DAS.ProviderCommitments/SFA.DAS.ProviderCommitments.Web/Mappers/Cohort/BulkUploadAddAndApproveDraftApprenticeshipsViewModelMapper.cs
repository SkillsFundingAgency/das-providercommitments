using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{  
    public class BulkUploadAddAndApproveDraftApprenticeshipsViewModelMapper : IMapper<BulkUploadAddAndApproveDraftApprenticeshipsResponse, BulkUploadAddAndApproveDraftApprenticeshipsViewModel>
    {
        public Task<BulkUploadAddAndApproveDraftApprenticeshipsViewModel> Map(BulkUploadAddAndApproveDraftApprenticeshipsResponse source)
        {          
            var viewModel = new BulkUploadAddAndApproveDraftApprenticeshipsViewModel
            {
                BulkUploadDraftApprenticeshipsViewModel = (from result in source.BulkUploadAddAndApproveDraftApprenticeshipResponse
                                                           let bulkUploadDraftApprenticeshipsViewModel = new BulkUploadDraftApprenticeshipViewModel
                                                           {
                                                               CohortReference = result.CohortReference,
                                                               NumberOfApprenticeships = result.NumberOfApprenticeships,
                                                               EmployerName = result.EmployerName
                                                           }
                                                           select bulkUploadDraftApprenticeshipsViewModel).ToList()
            };

            return Task.FromResult(viewModel);
        }
    }
}
