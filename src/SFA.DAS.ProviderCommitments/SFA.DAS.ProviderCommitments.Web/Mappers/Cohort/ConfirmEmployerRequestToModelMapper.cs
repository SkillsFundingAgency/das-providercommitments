﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Cohort
{
    public class ConfirmEmployerRequestToModelMapper : IMapper<ConfirmEmployerViewModel, ConfirmEmployerRedirectModel>
    {
        private readonly IOuterApiClient _apiClient;

        public ConfirmEmployerRequestToModelMapper(IOuterApiClient apiClient) => _apiClient = apiClient;

        public async Task<ConfirmEmployerRedirectModel> Map(ConfirmEmployerViewModel source)
        {
            var apiRequest = new GetConfirmEmployerRequest(source.ProviderId);
            var apiResponse = await _apiClient.Get<GetConfirmEmployerResponse>(apiRequest);

            return new ConfirmEmployerRedirectModel
            {
                HasNoDeclaredStandards = apiResponse.HasNoDeclaredStandards
            };
        }
    }
}
