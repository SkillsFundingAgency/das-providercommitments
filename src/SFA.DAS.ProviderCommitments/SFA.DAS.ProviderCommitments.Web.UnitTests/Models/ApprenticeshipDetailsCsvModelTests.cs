using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Mappers;
using SFA.DAS.ProviderCommitments.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models
{
    public class ApprenticeshipDetailsCsvModelTests
    {
        [Test, AutoData]
        public void Then_Maps_ApprenticeName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.ApprenticeName.Should().Be($"{source.FirstName} {source.LastName}");
        }

        [Test, AutoData]
        public void Then_Maps_Uln(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Uln.Should().Be(source.Uln);
        }

        [Test, AutoData]
        public void Then_Maps_EmployerName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Employer.Should().Be(source.EmployerName);
        }

        [Test, AutoData]
        public void Then_Maps_CourseName(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, AutoData]
        public void Then_Maps_PlannedStartDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.PlannedStartDate.Should().Be(source.StartDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PlannedEndDate(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.PlannedEndDate.Should().Be(source.EndDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_Status(
            GetApprenticeshipsResponse.ApprenticeshipDetailsResponse source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Status.Should().Be(source.ApprenticeshipStatus.GetDescription());
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

            ApprenticeshipDetailsCsvModel result = source;

            result.Alerts.Should().Be(expectedAlertString);
        }
    }
}