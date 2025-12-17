using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class SelectEmployerViewModelMapper(IApprovalsOuterApiClient approvalsOuterApiClient)
        : IMapper<SelectEmployerRequest, SelectEmployerViewModel>
    {
        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest source)
        {
            var apiRequest = new GetSelectEmployerRequest(
                source.ProviderId,
                source.SearchTerm,
                source.SortField,
                source.ReverseSort,
                source.UseLearnerData);

            var apiResponse = await approvalsOuterApiClient.GetSelectEmployer(apiRequest);

            var accountProviderLegalEntities = apiResponse.AccountProviderLegalEntities.ConvertAll(x =>
                new AccountProviderLegalEntityViewModel
                {
                    EmployerAccountLegalEntityName = x.AccountLegalEntityName,
                    EmployerAccountLegalEntityPublicHashedId = x.AccountLegalEntityPublicHashedId,
                    EmployerAccountName = x.AccountName,
                    EmployerAccountPublicHashedId = x.AccountPublicHashedId,
                    AccountHashedId = x.AccountHashedId,
                    LevyStatus = ParseApprenticeshipEmployerType(x.ApprenticeshipEmployerType)
                });

            var filterModel = new SelectEmployerFilterModel
            {
                SearchTerm = source.SearchTerm,
                ReverseSort = source.ReverseSort,
                CurrentlySortedByField = source.SortField,
                UseLearnerData = source.UseLearnerData,
                Employers = apiResponse.Employers
            };

            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = accountProviderLegalEntities,
                ProviderId = source.ProviderId,
                SelectEmployerFilterModel = filterModel,
                UseLearnerData = source.UseLearnerData
            };
        }

        private static ApprenticeshipEmployerType ParseApprenticeshipEmployerType(string apprenticeshipEmployerType)
        {
            if (string.IsNullOrWhiteSpace(apprenticeshipEmployerType))
            {
                return ApprenticeshipEmployerType.NonLevy;
            }

            if (Enum.TryParse<ApprenticeshipEmployerType>(apprenticeshipEmployerType, out var levyStatus))
            {
                return levyStatus;
            }

            return ApprenticeshipEmployerType.NonLevy;
        }
    }
}
