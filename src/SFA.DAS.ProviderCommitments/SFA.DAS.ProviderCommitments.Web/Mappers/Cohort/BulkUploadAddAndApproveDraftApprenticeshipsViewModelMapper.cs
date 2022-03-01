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
               
                BulkUploadDraftApprenticeshipsViewModel = source.BulkUploadAddAndApproveDraftApprenticeshipResponse
                                                        .Select(r => new BulkUploadDraftApprenticeshipViewModel
                                                        {                                                                        
                                                            CohortReference = r.CohortReference,
                                                            NumberOfApprenticeships = r.NumberOfApprenticeships,
                                                            EmployerName = r.EmployerName
                                                        }).ToList()

            };

            return Task.FromResult(viewModel);
        }
    }
}
