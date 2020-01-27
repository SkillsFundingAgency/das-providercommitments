using System;
using System.Collections.Generic;
using CsvHelper;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Services;


namespace SFA.DAS.ProviderCommitments.UnitTests.Services
{
    public class WhenICreateACsvFile
    {
        private ICreateCsvService _createCsvService;
        private List<ApprenticeshipDetailsResponse> _apprenticeshipDetails;
        [SetUp]
        public void SetUp()
        {
            _createCsvService = new CreateCsvService();

            _apprenticeshipDetails = new List<ApprenticeshipDetailsResponse>
            {
                new ApprenticeshipDetailsResponse
                {
                    Alerts = null,
                    FirstName = "Name1",
                    CourseName = "Course1",
                    EmployerName = "Employer1",
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN1"
                },
                new ApprenticeshipDetailsResponse
                {
                    Alerts = null,
                    FirstName = "Name2",
                    CourseName = "Course2",
                    EmployerName = "Employer2",
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN2"
                },
                new ApprenticeshipDetailsResponse
                {
                    Alerts = null,
                    FirstName = "Name3",
                    CourseName = "Course3",
                    EmployerName = "Employer3",
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN3"
                },
                new ApprenticeshipDetailsResponse
                {
                    Alerts = null,
                    FirstName = "Name4",
                    CourseName = "Course4",
                    EmployerName = "Employer4",
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN4"
                }
            };
        }

        [Ignore("Currently broken, Scott to fix on another branch")]
        public void Then_The_First_Line_Of_The_File_Is_The_Headers()
        {
            var actual = _createCsvService.GenerateCsvContent(_apprenticeshipDetails);

            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
            Assert.IsAssignableFrom<byte[]>(actual);
            var fileString = System.Text.Encoding.Default.GetString(actual);
            var headerLine = fileString.Split('\n')[0];
            Assert.AreEqual(9,headerLine.Split(',').Length);
            Assert.Contains(nameof(ApprenticeshipDetails.StartDate),headerLine.Split(','));
        }

        [Ignore("Currently broken, Scott to fix on another branch")]
        public void ThenTheCsvFileContentIsGenerated()
        {
            var actual = _createCsvService.GenerateCsvContent(_apprenticeshipDetails);

            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
            Assert.IsAssignableFrom<byte[]>(actual);
            var fileString = System.Text.Encoding.Default.GetString(actual);
            var lines = fileString.Split('\n');
            Assert.AreEqual(_apprenticeshipDetails.Count + 2,lines.Length);
            Assert.AreEqual(9,lines[0].Split(',').Length);
            Assert.AreEqual(_apprenticeshipDetails[0].StartDate.ToString(),lines[1].Split(',')[6]);
        }

        [Test]
        public void AndNothingIsPassedToTheContentGeneratorThenExceptionIsThrown()
        {
            List<ApprenticeshipDetails> nullList = null;

            Assert.Throws<WriterException>(() => _createCsvService.GenerateCsvContent(nullList));
        }
    }
}