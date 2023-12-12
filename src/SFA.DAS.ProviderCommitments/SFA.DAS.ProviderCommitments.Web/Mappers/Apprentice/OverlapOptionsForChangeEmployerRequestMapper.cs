﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class OverlapOptionsForChangeEmployerRequestMapper : IMapper<ChangeOfEmployerOverlapAlertViewModel,
        OverlapOptionsForChangeEmployerRequest>
    {
        public async Task<OverlapOptionsForChangeEmployerRequest> Map(ChangeOfEmployerOverlapAlertViewModel source)
        {
            return await Task.FromResult(new OverlapOptionsForChangeEmployerRequest
                {
                    ProviderId = source.ProviderId,
                    ApprenticeshipHashedId = source.ApprenticeshipHashedId,
                    ApprenticeshipId = source.ApprenticeshipId,
                    CacheKey = source.CacheKey
                }
            );
        }
    }
}