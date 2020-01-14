using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Html;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeshipDetailsToViewModelMapperTests
{
    public class WhenCallingMap
    {
        [Test, MoqAutoData]
        public async Task Then_Maps_ApprenticeshipId(
            ApprenticeshipDetailsResponse source,
            string encodedApprenticeshipId,
            [Frozen] Mock<IEncodingService> mockEncodingService,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            mockEncodingService
                .Setup(service => service.Encode(source.Id, EncodingType.ApprenticeshipId))
                .Returns(encodedApprenticeshipId);

            var result = await mapper.Map(source);

            result.EncodedApprenticeshipId.Should().Be(encodedApprenticeshipId);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_ApprenticeName(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Uln(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.Uln.Should().Be(source.Uln);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_EmployerName(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.EmployerName.Should().Be(source.EmployerName);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_CourseName(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_PlannedStartDate(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.PlannedStartDate.Should().Be(source.StartDate.ToString("MMM yyyy"));
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_PlannedEndDate(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.PlannedEndDate.Should().Be(source.EndDate.ToString("MMM yyyy"));
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Status(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.Status.Should().Be(source.PaymentStatus.ToString());
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Alerts(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.Alerts.Value.Should().Be(source.Alerts.Aggregate((a,b) => $"{a}<br/>{b}"));
        }

        [Test, MoqAutoData]
        public async Task And_No_Alerts_Then_Maps_Alerts_To_Empty_String(
            ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            source.Alerts = new List<string>();
            var result = await mapper.Map(source);

            result.Alerts.Value.Should().Be("");
        }
    }
}