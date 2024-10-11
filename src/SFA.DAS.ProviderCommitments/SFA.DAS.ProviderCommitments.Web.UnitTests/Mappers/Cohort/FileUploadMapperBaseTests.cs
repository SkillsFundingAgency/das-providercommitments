using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

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
            result.Count().Should().Be(1);
        }
    }

    [Test]
    public void VerifyFirstNameIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.FirstName.Should().Be(record.GivenNames);
        }
    }

    [Test]
    public void VerifyLastNameIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.LastName.Should().Be(record.FamilyName);
        }
    }

    [Test]
    public void VerifyDobIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.DateOfBirthAsString.Should().Be(record.DateOfBirth);
        }
    }

    [Test]
    public void VerifyStartDateIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.StartDateAsString.Should().Be(record.StartDate);
        }
    }

    [Test]
    public void VerifyEndDateIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.EndDateAsString.Should().Be(record.EndDate);
        }
    }

    [Test]
    public void VerifyCostIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.CostAsString.Should().Be(record.TotalPrice);
        }
    }

    [Test]
    public void VerifyProviderRefIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.ProviderRef.Should().Be(record.ProviderRef);
        }
    }

    [Test]
    public void VerifyCourseCodeIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.CourseCode.Should().Be(record.StdCode);
        }
    }

    [Test]
    public void VerifyLegalEntityIdIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.LegalEntityId.Should().Be(2);
        }
    }

    [Test]
    public void VerifyCohortIdIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.CohortId.Should().Be(1);
        }
    }

    [Test]
    public void VerifyTransferSenderIdIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.TransferSenderId.Should().Be(result.CohortId + 1);
        }
    }

    [Test]
    public void VerifyRecognisePriorLearningIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.RecognisePriorLearningAsString.Should().Be(record.RecognisePriorLearning);
        }
    }

    [Test]
    public void VerifyDurationReducedByIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.DurationReducedByAsString.Should().Be(record.DurationReducedBy);
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
            expectedValue.Should().Be(result.DurationReducedByAsString);
        }
    }

    [Test]
    public void VerifyPriceReducedByIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.PriceReducedByAsString.Should().Be(record.PriceReducedBy);
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

        result.FirstName.Should().Be(expectedResult);
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

        result.LastName.Should().Be(expectedResult);
    }

    [Test]
    public void VerifyIsDurationReducedByRplIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.IsDurationReducedByRPLAsString.Should().Be(record.IsDurationReducedByRPL);
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
            expectedValue.Should().Be(result.IsDurationReducedByRPLAsString);
        }
    }

    [Test]
    public void VerifyTrainingTotalHoursIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.TrainingTotalHoursAsString.Should().Be(record.TrainingTotalHours);
        }
    }

    [Test]
    public void VerifyTrainingHoursReductionIsMapped()
    {
        foreach (var record in CsvRecords)
        {
            var result = Result.First(x => x.Uln == record.ULN);
            result.TrainingHoursReductionAsString.Should().Be(record.TrainingHoursReduction);
        }
    }
}