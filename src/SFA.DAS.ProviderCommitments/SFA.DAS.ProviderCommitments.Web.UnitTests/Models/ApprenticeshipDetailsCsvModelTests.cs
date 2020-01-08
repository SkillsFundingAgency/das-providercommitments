using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderCommitments.Web.Models;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models
{
    public class ApprenticeshipDetailsCsvModelTests
    {
        [Test, AutoData]
        public void Then_Maps_ApprenticeName(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.ApprenticeName.Should().Be($"{source.ApprenticeFirstName} {source.ApprenticeLastName}");
        }

        [Test, AutoData]
        public void Then_Maps_Uln(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Uln.Should().Be(source.Uln);
        }

        [Test, AutoData]
        public void Then_Maps_EmployerName(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Employer.Should().Be(source.EmployerName);
        }

        [Test, AutoData]
        public void Then_Maps_CourseName(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.CourseName.Should().Be(source.CourseName);
        }

        [Test, AutoData]
        public void Then_Maps_PlannedStartDate(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.PlannedStartDate.Should().Be(source.PlannedStartDate.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_PlannedEndDate(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.PlannedEndDate.Should().Be(source.PlannedEndDateTime.ToString("MMM yyyy"));
        }

        [Test, AutoData]
        public void Then_Maps_Status(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Status.Should().Be(source.PaymentStatus.ToString());
        }

        [Test, AutoData]
        public void Then_Maps_Alerts(
            ApprenticeshipDetails source)
        {
            ApprenticeshipDetailsCsvModel result = source;

            result.Alerts.Should().Be(source.Alerts.Aggregate((a,b) => $"{a}|{b}"));
        }
    }
}