﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.ApprenticeshipDetailsToViewModelMapperTests
{
    public class WhenCallingMap
    {
        [Test, MoqAutoData]
        public async Task Then_Maps_ApprenticeshipId(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
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
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Uln(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.Uln.Should().Be(source.Uln);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_EmployerName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.EmployerName.Should().Be(source.EmployerName);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_CourseName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_PlannedStartDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.PlannedStartDate.Should().Be(source.StartDate);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_PlannedEndDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.PlannedEndDate.Should().Be(source.EndDate);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Status(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var result = await mapper.Map(source);

            result.Status.Should().Be(source.PaymentStatus.ToString());
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Alerts(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            var expectedAlertString = string.Empty;

            foreach (var alert in source.Alerts)
            {
                expectedAlertString += alert.FormatAlert() + "|";
            }
            expectedAlertString = expectedAlertString.TrimEnd('|');

            var result = await mapper.Map(source);

            result.Alerts.Value.Should().Be(expectedAlertString);
        }

        [Test, MoqAutoData]
        public async Task And_No_Alerts_Then_Maps_Alerts_To_Empty_String(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source,
            ApprenticeshipDetailsToViewModelMapper mapper)
        {
            source.Alerts = new List<Alerts>();
            var result = await mapper.Map(source);

            result.Alerts.Value.Should().Be("");
        }
    }
}