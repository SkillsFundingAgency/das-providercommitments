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
                Assert.That(result.Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void VerifyFirstNameIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.FirstName, Is.EqualTo(record.GivenNames));
            }
        }

        [Test]
        public void VerifyLastNameIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.LastName, Is.EqualTo(record.FamilyName));
            }
        }

        [Test]
        public void VerifyDobIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.DateOfBirthAsString, Is.EqualTo(record.DateOfBirth));
            }
        }

        [Test]
        public void VerifyStartDateIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.StartDateAsString, Is.EqualTo(record.StartDate));
            }
        }

        [Test]
        public void VerifyEndDateIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.EndDateAsString, Is.EqualTo(record.EndDate));
            }
        }

        [Test]
        public void VerifyCostIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.CostAsString, Is.EqualTo(record.TotalPrice));
            }
        }

        [Test]
        public void VerifyProviderRefIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.ProviderRef, Is.EqualTo(record.ProviderRef));
            }
        }

        [Test]
        public void VerifyCourseCodeIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.CourseCode, Is.EqualTo(record.StdCode));
            }
        }

        [Test]
        public void VerifyLegalEntityIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.LegalEntityId, Is.EqualTo(2));
            }
        }

        [Test]
        public void VerifyCohortIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.CohortId, Is.EqualTo(1));
            }
        }

        [Test]
        public void VerifyTransferSenderIdIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.TransferSenderId, Is.EqualTo((result.CohortId + 1)));
            }
        }

        [Test]
        public void VerifyRecognisePriorLearningIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.RecognisePriorLearningAsString, Is.EqualTo(record.RecognisePriorLearning));
            }
        }

        [Test]
        public void VerifyDurationReducedByIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.DurationReducedByAsString, Is.EqualTo(record.DurationReducedBy));
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
                Assert.That(expectedValue, Is.EqualTo(result.DurationReducedByAsString));
            }
        }

        [Test]
        public void VerifyPriceReducedByIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.PriceReducedByAsString, Is.EqualTo(record.PriceReducedBy));
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

            Assert.That(result.FirstName, Is.EqualTo(expectedResult));
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

            Assert.That(result.LastName, Is.EqualTo(expectedResult));
        }

        [Test]
        public void VerifyIsDurationReducedByRplIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.IsDurationReducedByRPLAsString, Is.EqualTo(record.IsDurationReducedByRPL));
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
                Assert.That(expectedValue, Is.EqualTo(result.IsDurationReducedByRPLAsString));
            }
        }

        [Test]
        public void VerifyTrainingTotalHoursIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.TrainingTotalHoursAsString, Is.EqualTo(record.TrainingTotalHours));
            }
        }

        [Test]
        public void VerifyTrainingHoursReductionIsMapped()
        {
            foreach (var record in CsvRecords)
            {
                var result = Result.First(x => x.Uln == record.ULN);
                Assert.That(result.TrainingHoursReductionAsString, Is.EqualTo(record.TrainingHoursReduction));
            }
        }
    }
}