﻿using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.DraftApprenticeship;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.Mappers
{
    public class EditDraftApprenticeshipToUpdateRequestMapper : IMapper<EditDraftApprenticeshipViewModel, UpdateDraftApprenticeshipApimRequest>
    {
        public Task<UpdateDraftApprenticeshipApimRequest> Map(EditDraftApprenticeshipViewModel source)
        {
            int? GetCost()
            {
                if (source.IsOnFlexiPaymentPilot is not true) return source.Cost;
                if (source.TrainingPrice is null && source.EndPointAssessmentPrice is null) return source.Cost ?? null;
                
                return source.TrainingPrice.GetValueOrDefault() + source.EndPointAssessmentPrice.GetValueOrDefault();
            }

            return Task.FromResult(new UpdateDraftApprenticeshipApimRequest
            {
                ReservationId = source.ReservationId,
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                DateOfBirth = source.DateOfBirth.Date,
                Uln = source.Uln,
                CourseCode = source.CourseCode,
                Cost = GetCost(),
                TrainingPrice = source.TrainingPrice,
                EndPointAssessmentPrice = source.EndPointAssessmentPrice,
                StartDate = source.StartDate?.Date,
                ActualStartDate = source.ActualStartDate?.Date,
                EndDate = source.EndDate.Date,
                Reference = source.Reference,
                CourseOption = source.TrainingCourseOption == "-1" ? string.Empty : source.TrainingCourseOption,
                DeliveryModel = source.DeliveryModel.Value,
                EmploymentEndDate = source.EmploymentEndDate.Date,
                EmploymentPrice = source.EmploymentPrice,
                IsOnFlexiPaymentPilot = source.IsOnFlexiPaymentPilot
            });
        }
    }
}
