using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class FileUploadMapperBaseTests
    {
        public List<CsvRecord> CsvRecords { get; set; }
        public List<BulkUploadAddDraftApprenticeshipRequest> Result { get; set; }
        public FileUploadMapperBase Sut { get; set; }
        public Fixture Fixture { get; set; }
        public Mock<IEncodingService> EncodingService { get; set; }
        private Mock<IOuterApiService> _outerApiService;
        private GetCohortResult _cohortResult;

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
            EncodingService = new Mock<IEncodingService>();
            EncodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId)).Returns(() => 2);
            EncodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(() => 1);

            CsvRecords = Fixture.Build<CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .CreateMany(2).ToList();

            _cohortResult = Fixture.Create<GetCohortResult>();
            _outerApiService = new Mock<IOuterApiService>();
            _outerApiService.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync((long cohortId) => { _cohortResult.TransferSenderId = cohortId + 1; return _cohortResult; });

            Sut = new FileUploadMapperBase(EncodingService.Object, _outerApiService.Object);
            Result = Sut.ConvertToBulkUploadApiRequest(CsvRecords, 1, false);
        }

        [Test]
        public void VerifyUlnIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.Where(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.Count());
            }
        }

        [Test]
        public void VerifyFirstNameIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.GivenNames, result.FirstName);
            }
        }

        [Test]
        public void VerifyLastNameIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.FamilyName, result.LastName);
            }
        }

        [Test]
        public void VerifyDobIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.DateOfBirth, result.DateOfBirthAsString);
            }
        }

        [Test]
        public void VerifyStartDateIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.StartDate, result.StartDateAsString);
            }
        }

        [Test]
        public void VerifyEndDateIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.EndDate, result.EndDateAsString);
            }
        }

        [Test]
        public void VerifyCostIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TotalPrice, result.CostAsString);
            }
        }

        [Test]
        public void VerifyProviderRefIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.ProviderRef, result.ProviderRef);
            }
        }

        [Test]
        public void VerifyCourseCodeIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.StdCode, result.CourseCode);
            }
        }

        [Test]
        public void VerifyLegalEntityIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(2, result.LegalEntityId);
            }
        }

        [Test]
        public void VerifyCohortIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(1, result.CohortId);
            }
        }

        [Test]
        public void VerifyTransferSenderIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual((result.CohortId + 1), result.TransferSenderId);
            }
        }

        [Test]
        public void VerifyRecognisePriorLearningIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.RecognisePriorLearning, result.RecognisePriorLearningAsString);
            }
        }

        [Test]
        public void VerifyDurationReducedByIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.DurationReducedBy, result.DurationReducedByAsString);
            }
        }

        [TestCase(null, null, null)]
        [TestCase(null, "200", "200")]
        [TestCase(null, "0", null)]
        [TestCase("TRUE", "200", "200")]
        [TestCase("TRUE", "0", "0")]
        [TestCase("FALSE", "0", "0")]
        public void VerifyDurationReducedByIsMappedCorrectlyWhenExtendedRplIsOn(string isDurationReducedByRpl, string durationReducedBy, string expectedValue)
        {
            CsvRecords = Fixture.Build<CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .With(x => x.IsDurationReducedByRPL, isDurationReducedByRpl)
                .With(x => x.DurationReducedBy, durationReducedBy)
                .CreateMany(2).ToList();

            Result = Sut.ConvertToBulkUploadApiRequest(CsvRecords, 1, true);

            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(result.DurationReducedByAsString, expectedValue);
            }
        }

        [Test]
        public void VerifyPriceReducedByIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.PriceReducedBy, result.PriceReducedByAsString);
            }
        }

        [TestCase(" Test ", "Test")]
        [TestCase(" Test Me ", "Test Me")]
        [TestCase(" Test\tMe ", "Test Me")]
        [TestCase(" \tTest\tMe\t", "Test Me")]
        public void VerifyTabsAndSpacesAreTrimmedFromFirstName(string inputValue, string expectedResult)
        {
            var source = CsvRecords.First();
            source.GivenNames = inputValue;

            Result = Sut.ConvertToBulkUploadApiRequest(CsvRecords, 1, false);

            var result = Result.First(x => x.Uln == source.ULN);

            Assert.AreEqual(expectedResult, result.FirstName);
        }

        [TestCase(" Test ", "Test")]
        [TestCase(" Test Me ", "Test Me")]
        [TestCase(" Test\tMe ", "Test Me")]
        [TestCase(" \tTest\tMe\t", "Test Me")]
        public void VerifyTabsAndSpacesAreTrimmedFromLastName(string inputValue, string expectedResult)
        {
            var source = CsvRecords.First();
            source.FamilyName = inputValue;

            Result = Sut.ConvertToBulkUploadApiRequest(CsvRecords, 1, false);

            var result = Result.First(x => x.Uln == source.ULN);

            Assert.AreEqual(expectedResult, result.LastName);
        }

        [Test]
        public void VerifyIsDurationReducedByRplIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.IsDurationReducedByRPL, result.IsDurationReducedByRPLAsString);
            }
        }

        [TestCase(null, null, null)]
        [TestCase(null, "200", "TRUE")]
        [TestCase("TRUE", "200", "TRUE")]
        [TestCase("FALSE", "200", "FALSE")]
        [TestCase("FALSE", null, "FALSE")]
        [TestCase("XXX", null, "XXX")]
        [TestCase(null, "0", "FALSE")]
        public void VerifyIsDurationReducedByRplIsDefaultedCorrectlyWhenExtendedRplIsOn(string isDurationReducedByRpl, string durationReducedBy, string expectedValue)
        {
            CsvRecords = Fixture.Build<CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .With(x=>x.IsDurationReducedByRPL, isDurationReducedByRpl)
                .With(x=>x.DurationReducedBy, durationReducedBy)
                .CreateMany(2).ToList();

            Result = Sut.ConvertToBulkUploadApiRequest(CsvRecords, 1, true);

            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(result.IsDurationReducedByRPLAsString, expectedValue);
            }
        }

        [Test]
        public void VerifyTrainingTotalHoursIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TrainingTotalHours, result.TrainingTotalHoursAsString);
            }
        }

        [Test]
        public void VerifyTrainingHoursReductionIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.AreEqual(record.TrainingHoursReduction, result.TrainingHoursReductionAsString);
            }
        }
    }
}