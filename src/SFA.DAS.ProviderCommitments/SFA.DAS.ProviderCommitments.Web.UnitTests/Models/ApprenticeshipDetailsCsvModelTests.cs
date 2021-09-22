using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models
{
    public class ApprenticeshipDetailsCsvModelTests
    {
        [Test, MoqAutoData]
        public void Then_Maps_ApprenticeName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source,encodingService.Object);

            result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
        }

        [Test, AutoData]
        public void Then_Maps_Uln(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Uln.Should().Be(source.Uln);
        }

        [Test, AutoData]
        public void Then_Maps_EmployerName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Employer.Should().Be(source.EmployerName);
        }

        [Test, AutoData]
        public void Then_Maps_CourseName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, AutoData]
        public void Then_Maps_PlannedStartDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PlannedStartDate.Should().Be(source.StartDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PlannedEndDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PlannedEndDate.Should().Be(source.EndDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PausedDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PausedDate.Should().Be(source.PauseDate.ToString("MMM yyyy"));
        }
        [Test, AutoData]
        public void Then_Maps_Empty_If_No_PausedDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            source.PauseDate = DateTime.MinValue;
            var result = model.Map(source, encodingService.Object);

            result.PausedDate.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_Maps_DateOfBirth(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.DateOfBirth.Should().Be(source.DateOfBirth.ToString("d MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_ApprenticeConfirmation(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.ApprenticeConfirmation.Should().Be(source.ConfirmationStatus.GetDescription());
        }

        [Test, AutoData]
        public void Then_Maps_Null_ApprenticeConfirmation(GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            source.ConfirmationStatus = null;
            var result = model.Map(source, encodingService.Object);

            result.ApprenticeConfirmation.Should().Be("N/A");
        }

        [Test, AutoData]
        public void Then_Maps_Status(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Status.Should().Be(source.ApprenticeshipStatus.GetDescription());
        }

        [Test, AutoData]
        public void Then_Maps_Reference(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.CohortReference.Should().Be(source.CohortReference);
        }

        [Test, AutoData]
        public void Then_Maps_Your_Reference(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.ProviderRef.Should().Be(source.ProviderRef);
        }

        [Test, AutoData]
        public void Then_Maps_TotalAgreedPrice(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.TotalAgreedPrice.Should().Be($"{source.TotalAgreedPrice.Value as object:n0}");
        }

        [Test, AutoData]
        public void Then_Maps_AgreementId(
            string agreementId,
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            encodingService.Setup(x => x.Encode(source.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId)).Returns(agreementId);
            var result = model.Map(source, encodingService.Object);

            result.AgreementId.Should().Be(agreementId);
        }

        [Test, MoqAutoData]
        public void Then_Maps_Alerts(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var expectedAlertString = string.Empty;

            foreach (var alert in source.Alerts)
            {
                expectedAlertString += alert.GetDescription() + "|";
            }
            expectedAlertString = expectedAlertString.TrimEnd('|');

            var result = model.Map(source, encodingService.Object);

            result.Alerts.Should().Be(expectedAlertString);
        }
    }
}