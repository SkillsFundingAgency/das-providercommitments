using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectIlrRecordViewModelMapper(IOuterApiService client) : IMapper<SelectIlrRecordRequest, SelectIlrRecordViewModel>
    {
        public async Task<SelectIlrRecordViewModel> Map(SelectIlrRecordRequest source)
        {
            var response = await client.GetIlrDetailsForProvider(source.ProviderId, source.AccountLegalEntityId, source.SearchTerm, source.SortField, source.ReverseSort, 1);

            var filterModel = new IlrRecordsFilterModel()
            {
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                TotalNumberOfApprenticeshipsFound = response.Total,
                PageNumber = response.Page,
                SortField = source.SortField,
                ReverseSort = source.ReverseSort,
                SearchTerm = source.SearchTerm,
            };

            var model= new SelectIlrRecordViewModel
            {
                ProviderId = source.ProviderId,
                EmployerAccountLegalEntityPublicHashedId = source.EmployerAccountLegalEntityPublicHashedId,
                EmployerAccountName = response.EmployerName,
                IlrApprenticeships = response.Learners.Select(x=>(IlrApprenticeshipSummary)x).ToList(),
                
                FilterModel = filterModel
            };
            model.SortedByHeader();
            return model;
        }
    }
}
