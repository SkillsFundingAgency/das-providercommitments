using System;
using System.Collections.Generic;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Razor.Language;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;

namespace SFA.DAS.ProviderCommitments.Web.Models.Apprentice
{
    public class ApprenticeshipDetailsCsvModel
    {
        [Name("Apprentice name")]
        public string ApprenticeName { get ; private set ; }
        [Name("ULN")]
        public string Uln { get ; private set ; }
        [Name("Employer")]
        public string Employer { get ; private set ; }
        [Name("Apprenticeship training course")]
        public string CourseName { get ; private set ; }
        [Name("Planned start date")]
        public string PlannedStartDate { get ; private set ; }
        [Name("Planned end date")]
        public string PlannedEndDate { get ; private set ; }
        [Name("Paused date")]
        public string PausedDate { get; private set; }
        [Name("Reference")]
        public string CohortReference { get; private set; }
        [Name("Date of birth")]
        public string DateOfBirth { get; private set; }
        [Name("Total agreed apprenticeship price")]
        public string TotalAgreedPrice { get; private set; }
        [Name("Your reference")]
        public string ProviderRef { get; private set; }
        [Name("Agreement ID")]
        public string AgreementId { get; private set; }
        [Name("Status")]
        public string Status { get ; private set ; }

        [Name("Alerts")]
        public string Alerts { get ; private set ; }


        public ApprenticeshipDetailsCsvModel Map(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse model, IEncodingService encodingService)
        {
            return new ApprenticeshipDetailsCsvModel
            {
                ApprenticeName = $"{model.FirstName} {model.LastName}",
                Uln = model.Uln,
                Employer = model.EmployerName,
                CourseName = model.CourseName,
                PlannedStartDate = model.StartDate.ToGdsFormatWithoutDay(),
                PlannedEndDate = model.EndDate.ToGdsFormatWithoutDay(),
                Status = model.ApprenticeshipStatus.GetDescription(),
                Alerts = GenerateAlerts(model.Alerts),
                ProviderRef = model.ProviderRef,
                CohortReference = model.CohortReference,
                AgreementId = encodingService.Encode(model.AccountLegalEntityId,EncodingType.PublicAccountLegalEntityId),
                DateOfBirth = model.DateOfBirth.ToGdsFormat(),
                PausedDate = model.PauseDate != DateTime.MinValue ? model.PauseDate.ToGdsFormatWithoutDay() : "",
                TotalAgreedPrice = $"{model.TotalAgreedPrice.Value as object:n0}"
            };
        }


        private static string GenerateAlerts(IEnumerable<Alerts> alerts)
        {
            var alertString = string.Empty;

            foreach (var alert in alerts)
            {
                if (!string.IsNullOrWhiteSpace(alertString))
                {
                    alertString += "|";
                }
                alertString += alert.GetDescription();
            }

            return alertString;
        }
    }
}