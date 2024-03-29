﻿using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice
{
    public class ConfirmEditApprenticeshipViewModelToEditApiRequestMapper : IMapper<ConfirmEditApprenticeshipViewModel, EditApprenticeshipApiRequest>
    {
        public Task<EditApprenticeshipApiRequest> Map(ConfirmEditApprenticeshipViewModel source)
        {
            return Task.FromResult(new EditApprenticeshipApiRequest
            {
                ApprenticeshipId = source.ApprenticeshipId,
                ProviderId = source.ProviderId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth,
                Cost = source.Cost,
                ProviderReference = source.ProviderReference,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                DeliveryModel = source.DeliveryModel,
                EmploymentEndDate = source.EmploymentEndDate,
                EmploymentPrice = source.EmploymentPrice,
                CourseCode = source.CourseCode,
                Version = source.Version,
                Option = source.Option == "TBC" ? string.Empty : source.Option
            });
        }
    }
}
