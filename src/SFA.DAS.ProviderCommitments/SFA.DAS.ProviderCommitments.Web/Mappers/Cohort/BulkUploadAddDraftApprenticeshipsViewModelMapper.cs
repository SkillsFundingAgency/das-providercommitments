using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class BulkUploadAddDraftApprenticeshipsViewModelMapper : IMapper<GetBulkUploadAddDraftApprenticeshipsResponse, BulkUploadAddDraftApprenticeshipsViewModel>
    {
        public Task<BulkUploadAddDraftApprenticeshipsViewModel> Map(GetBulkUploadAddDraftApprenticeshipsResponse source)
        {   
            var viewModel = new BulkUploadAddDraftApprenticeshipsViewModel
            {                
                BulkUploadDraftApprenticeshipsViewModel = (from result in source.BulkUploadAddDraftApprenticeshipsResponse
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

