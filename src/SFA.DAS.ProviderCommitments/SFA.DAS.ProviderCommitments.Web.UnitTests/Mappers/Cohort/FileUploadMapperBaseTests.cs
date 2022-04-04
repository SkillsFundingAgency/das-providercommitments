﻿using AutoFixture;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class FileUploadMapperBaseTests
    {
        public List<Web.Models.Cohort.CsvRecord> _csvRecords { get; set; }
        public List<BulkUploadAddDraftApprenticeshipRequest> _result { get; set; }
        public FileUploadMapperBase Sut { get; set; }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            var encodingService = new Mock<IEncodingService>();
            encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId)).Returns(() => 2);
            encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(() => 1);

            _csvRecords = fixture.Build<Web.Models.Cohort.CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .CreateMany(2).ToList();

            Sut = new FileUploadMapperBase(encodingService.Object);
            _result = Sut.ConvertToBulkUploadApiRequest(_csvRecords, 1);
        }

        [Test]
        public void VerifyUlnIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.Where(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.Count());
            }
        }

        [Test]
        public void VerifyFirstNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.GivenNames, result.FirstName);
            }
        }

        [Test]
        public void VerifyLastNameIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.FamilyName, result.LastName);
            }
        }

        [Test]
        public void VerifyDOBIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.DateOfBirth, result.DateOfBirthAsString);
            }
        }

        [Test]
        public void VerifyStartDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.StartDate, result.StartDateAsString);
            }
        }

        [Test]
        public void VerifyEndDateIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.EndDate, result.EndDateAsString);
            }
        }

        [Test]
        public void VerifyCostIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TotalPrice, result.CostAsString);
            }
        }

        [Test]
        public void VerifyProviderRefIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.ProviderRef, result.ProviderRef);
            }
        }

        [Test]
        public void VerifyCourseCodeIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.StdCode, result.CourseCode);
            }
        }

        [Test]
        public void VerifyLegalEntityIdIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(2, result.LegalEntityId);
            }
        }

        [Test]
        public void VerifyCohortIdIsMapped()
        {
            foreach (var record in _csvRecords)
            {
                var result = _result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.CohortId);
            }
        }
    }
}
