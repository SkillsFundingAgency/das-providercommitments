using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenMappingFileUploadValidateDataRequestToApiRequestTests
{
    private FileUploadValidateDataRequestToApiRequest _mapper;
    private BulkUploadValidateApimRequest _result;
    private List<CsvRecord> _csvRecords;
    private FileUploadValidateDataRequest _request;

    [SetUp]
    public async Task Setup()
    {
        _csvRecords = new List<CsvRecord>
        {
            new()
            {
                AgreementId = "XEGE5X",
                CohortRef = "P97BKL",
                ULN = "6591690157",
                FamilyName = "Smith",
                GivenNames = "Mark",
                DateOfBirth = "2000-01-02",
                StdCode = "59",
                StartDate = "2019-05-01",
                EndDate = "2020-05",
                TotalPrice = "2000",
                EPAOrgID = "EPA0001",
                ProviderRef = "ZB88",
                EmailAddress = "abc34628125987@abc.com"
            },
            new()
            {
                AgreementId = "XEGE5X2",
                CohortRef = "P97BKL2",
                ULN = "65916901572",
                FamilyName = "Smith2",
                GivenNames = "Mark2",
                DateOfBirth = "2000-01-22",
                StdCode = "592",
                StartDate = "2019-05-02",
                EndDate = "2020-02",
                TotalPrice = "2002",
                EPAOrgID = "EPA0002",
                ProviderRef = "ZB82",
                EmailAddress = "abc34628125987@abc.com2"
            }
        };
        _request = new FileUploadValidateDataRequest { CsvRecords = _csvRecords, ProviderId = 1 };

        var encodingService = new Mock<IEncodingService>();
        encodingService.Setup(x => x.Decode(It.IsAny<string>(), It.IsAny<EncodingType>())).Returns(2);
        var outerApiService = new Mock<IOuterApiService>();
        outerApiService.Setup(x => x.GetCohort(It.IsAny<long>())).ReturnsAsync(() => new GetCohortResult());

        _mapper = new FileUploadValidateDataRequestToApiRequest(encodingService.Object, outerApiService.Object);
        _result = await _mapper.Map(_request);
    }

    [Test]
    public void ProviderIdIsMapped()
    {
        _result.ProviderId.Should().Be(1);
    }

    [Test]
    public void RowNumber_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().RowNumber.Should().Be(1);
            _result.CsvRecords.Last().RowNumber.Should().Be(2);
        });
    }

    [Test]
    public void ProviderRef_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().ProviderRef.Should().Be(_csvRecords.First().ProviderRef);
            _result.CsvRecords.Last().ProviderRef.Should().Be(_csvRecords.Last().ProviderRef);
        });
    }

    [Test]
    public void EmailAddress_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().Email.Should().Be(_csvRecords.First().EmailAddress);
            _result.CsvRecords.Last().Email.Should().Be(_csvRecords.Last().EmailAddress);
        });
    }

    [Test]
    public void AgreementId_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().AgreementId.Should().Be(_csvRecords.First().AgreementId);
            _result.CsvRecords.Last().AgreementId.Should().Be(_csvRecords.Last().AgreementId);
        });
    }

    [Test]
    public void CohortRef_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().CohortRef.Should().Be(_csvRecords.First().CohortRef);
            _result.CsvRecords.Last().CohortRef.Should().Be(_csvRecords.Last().CohortRef);
        });
    }

    [Test]
    public void ULN_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().Uln.Should().Be(_csvRecords.First().ULN);
            _result.CsvRecords.Last().Uln.Should().Be(_csvRecords.Last().ULN);
        });
    }

    [Test]
    public void FamilyName_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().LastName.Should().Be(_csvRecords.First().FamilyName);
            _result.CsvRecords.Last().LastName.Should().Be(_csvRecords.Last().FamilyName);
        });
    }

    [Test]
    public void GivenNames_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().FirstName.Should().Be(_csvRecords.First().GivenNames);
            _result.CsvRecords.Last().FirstName.Should().Be(_csvRecords.Last().GivenNames);
        });
    }

    [Test]
    public void DateOfBirth_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().DateOfBirthAsString.Should().Be(_csvRecords.First().DateOfBirth);
            _result.CsvRecords.Last().DateOfBirthAsString.Should().Be(_csvRecords.Last().DateOfBirth);
        });
    }

    [Test]
    public void StdCode_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().CourseCode.Should().Be(_csvRecords.First().StdCode);
            _result.CsvRecords.Last().CourseCode.Should().Be(_csvRecords.Last().StdCode);
        });
    }

    [Test]
    public void StartDate_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().StartDateAsString.Should().Be(_csvRecords.First().StartDate);
            _result.CsvRecords.Last().StartDateAsString.Should().Be(_csvRecords.Last().StartDate);
        });
    }

    [Test]
    public void EndDate_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().EndDateAsString.Should().Be(_csvRecords.First().EndDate);
            _result.CsvRecords.Last().EndDateAsString.Should().Be(_csvRecords.Last().EndDate);
        });
    }

    [Test]
    public void TotalPrice_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().CostAsString.Should().Be(_csvRecords.First().TotalPrice);
            _result.CsvRecords.Last().CostAsString.Should().Be(_csvRecords.Last().TotalPrice);
        });
    }

    [Test]
    public void EpaOrgId_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().EPAOrgId.Should().Be(_csvRecords.First().EPAOrgID);
            _result.CsvRecords.Last().EPAOrgId.Should().Be(_csvRecords.Last().EPAOrgID);
        });
    }

    [Test]
    public void ProviderId_Is_Mapped()
    {
        Assert.Multiple(() =>
        {
            _result.CsvRecords.First().ProviderId.Should().Be(1);
            _result.CsvRecords.Last().ProviderId.Should().Be(1);
        });
    }
}