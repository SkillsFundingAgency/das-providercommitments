using System;
using AutoFixture.NUnit3;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models
{
    public class ApprenticeshipDetailsCsvModelTests
    {
        [Test, MoqAutoData]
        public void Then_Maps_ApprenticeName(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source,encodingService.Object);

            result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
        }
        
        [Test, MoqAutoData]
        public void Then_Maps_Email(
            PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source,encodingService.Object);

            result.Email.Should().Be(source.Email);
        }

        [Test, AutoData]
        public void Then_Maps_Uln(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Uln.Should().Be(source.Uln);
        }

        [Test, AutoData]
        public void Then_Maps_EmployerName(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Employer.Should().Be(source.EmployerName);
        }

        [Test, AutoData]
        public void Then_Maps_CourseName(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, AutoData]
        public void Then_Maps_PlannedStartDate(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PlannedStartDate.Should().Be(source.StartDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PlannedEndDate(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PlannedEndDate.Should().Be(source.EndDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PausedDate(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.PausedDate.Should().Be(source.PauseDate.ToString("MMM yyyy"));
        }
        [Test, AutoData]
        public void Then_Maps_Empty_If_No_PausedDate(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            source.PauseDate = DateTime.MinValue;
            var result = model.Map(source, encodingService.Object);

            result.PausedDate.Should().BeEmpty();
        }

        [Test, AutoData]
        public void Then_Maps_DateOfBirth(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.DateOfBirth.Should().Be(source.DateOfBirth.ToString("d MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_ApprenticeConfirmation(PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.ApprenticeConfirmation.Should().Be(source.ConfirmationStatus.GetDescription());
        }

        [Test, AutoData]
        public void Then_Maps_Null_ApprenticeConfirmation(PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            source.ConfirmationStatus = null;
            var result = model.Map(source, encodingService.Object);

            result.ApprenticeConfirmation.Should().Be("N/A");
        }

        [Test, AutoData]
        public void Then_Maps_Status(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.Status.Should().Be(source.ApprenticeshipStatus.GetDescription());
        }

        [Test, AutoData]
        public void Then_Maps_Reference(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.CohortReference.Should().Be(source.CohortReference);
        }

        [Test, AutoData]
        public void Then_Maps_Your_Reference(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.ProviderRef.Should().Be(source.ProviderRef);
        }

        [Test, AutoData]
        public void Then_Maps_TotalAgreedPrice(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            var result = model.Map(source, encodingService.Object);

            result.TotalAgreedPrice.Should().Be($"{source.TotalAgreedPrice.Value as object:n0}");
        }

        [Test, AutoData]
        public void Then_Maps_AgreementId(
            string agreementId,
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
            [Frozen] Mock<IEncodingService> encodingService,
            ApprenticeshipDetailsCsvModel model)
        {
            encodingService.Setup(x => x.Encode(source.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId)).Returns(agreementId);
            var result = model.Map(source, encodingService.Object);

            result.AgreementId.Should().Be(agreementId);
        }

        [Test, MoqAutoData]
        public void Then_Maps_Alerts(
           PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse source,
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

        [TestCase(DeliveryModel.FlexiJobAgency, "Flexi-job agency")]
        [TestCase(DeliveryModel.PortableFlexiJob, "Portable flexi-job")]
        [TestCase(DeliveryModel.Regular, "Regular")]
        public void Then_Maps_DeliveryModel(DeliveryModel deliveryModel, string desc)
        {
            var f = new Fixture();
            var source = f.Build<PostApprenticeshipsCSVResponse.ApprenticeshipDetailsCSVResponse>()
                .With(x => x.DeliveryModel, deliveryModel).Create();
        
            var encodingService = new Mock<IEncodingService>();
            var model = new ApprenticeshipDetailsCsvModel();
        
            var result = model.Map(source, encodingService.Object);

            result.DeliveryModel.Should().Be(desc);
        }
    }
}