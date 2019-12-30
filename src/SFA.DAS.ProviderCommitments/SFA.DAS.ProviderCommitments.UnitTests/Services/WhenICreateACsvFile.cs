﻿using System;
using System.Collections.Generic;
using CsvHelper;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Services;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services
{
    public class WhenICreateACsvFile
    {
        private ICreateCsvService _createCsvService;
        private List<ApprenticeshipDetails> _apprenticeshipDetails;
        [SetUp]
        public void SetUp()
        {
            _createCsvService = new CreateCsvService();

            _apprenticeshipDetails = new List<ApprenticeshipDetails>
            {
                new ApprenticeshipDetails
                {
                    Alerts = null,
                    ApprenticeFirstName = "Name1",
                    CourseName = "Course1",
                    EmployerName = "Employer1",
                    PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                    PlannedStartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN1"
                },
                new ApprenticeshipDetails
                {
                    Alerts = null,
                    ApprenticeFirstName = "Name2",
                    CourseName = "Course2",
                    EmployerName = "Employer2",
                    PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                    PlannedStartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN2"
                },
                new ApprenticeshipDetails
                {
                    Alerts = null,
                    ApprenticeFirstName = "Name3",
                    CourseName = "Course3",
                    EmployerName = "Employer3",
                    PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                    PlannedStartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN3"
                },
                new ApprenticeshipDetails
                {
                    Alerts = null,
                    ApprenticeFirstName = "Name4",
                    CourseName = "Course4",
                    EmployerName = "Employer4",
                    PlannedEndDateTime = DateTime.UtcNow.AddMonths(2),
                    PlannedStartDate = DateTime.UtcNow,
                    PaymentStatus = PaymentStatus.Active,
                    Uln = "ULN4"
                }
            };
        }

        [Test]
        public void ThenTheCsvFileContentIsGenerated()
        {
            var actual = _createCsvService.GenerateCsvContent(_apprenticeshipDetails);

            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public void AndNothingIsPassedToTheContentGeneratorThenExceptionIsThrown()
        {
            List<ApprenticeshipDetails> nullList = null;

            Assert.Throws<WriterException>(() => _createCsvService.GenerateCsvContent(nullList));
        }
    }
}