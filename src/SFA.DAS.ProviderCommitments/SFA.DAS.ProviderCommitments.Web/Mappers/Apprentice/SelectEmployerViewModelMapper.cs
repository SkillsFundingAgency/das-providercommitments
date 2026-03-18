using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Shared;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class SelectEmployerViewModelMapper(IApprovalsOuterApiClient approvalsOuterApiClient)
        : IMapper<SelectEmployerRequest, SelectEmployerViewModel>
    {
        public async Task<SelectEmployerViewModel> Map(SelectEmployerRequest source)
        {
            var pageNumber = source.PageNumber < 1 ? 1 : source.PageNumber;

            var apiRequest = new GetSelectNewEmployerRequest(
            source.ApprenticeshipId,
            source.ProviderId,
            source.SearchTerm,
            source.SortField,
            source.ReverseSort,
            pageNumber,
            Constants.SelectEmployer.NumberOfEmployersPerPage);

            var apiResponse = await approvalsOuterApiClient.GetSelectNewEmployer(apiRequest);

            var accountProviderLegalEntities = (apiResponse.AccountProviderLegalEntities ?? []).ConvertAll(x =>
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
                ProviderId = source.ProviderId,
                PageNumber = pageNumber,
                TotalEmployersFound = apiResponse.TotalCount,
                Employers = apiResponse.Employers ?? []
            };

            return new SelectEmployerViewModel
            {
                AccountProviderLegalEntities = accountProviderLegalEntities,
                LegalEntityName = apiResponse.EmployerName,
                SelectEmployerFilterModel = filterModel,
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