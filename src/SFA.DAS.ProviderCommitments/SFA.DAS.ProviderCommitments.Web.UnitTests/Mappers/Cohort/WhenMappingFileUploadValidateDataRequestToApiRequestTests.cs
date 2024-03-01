using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
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
            Assert.That(_result.ProviderId, Is.EqualTo(1));
        }

        [Test]
        public void RowNumber_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().RowNumber, Is.EqualTo(1));
                Assert.That(_result.CsvRecords.Last().RowNumber, Is.EqualTo(2));
            });
        }

        [Test]
        public void ProviderRef_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().ProviderRef, Is.EqualTo(_csvRecords.First().ProviderRef));
                Assert.That(_result.CsvRecords.Last().ProviderRef, Is.EqualTo(_csvRecords.Last().ProviderRef));
            });
        }

        [Test]
        public void EmailAddress_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().Email, Is.EqualTo(_csvRecords.First().EmailAddress));
                Assert.That(_result.CsvRecords.Last().Email, Is.EqualTo(_csvRecords.Last().EmailAddress));
            });
        }

        [Test]
        public void AgreementId_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().AgreementId, Is.EqualTo(_csvRecords.First().AgreementId));
                Assert.That(_result.CsvRecords.Last().AgreementId, Is.EqualTo(_csvRecords.Last().AgreementId));
            });
        }

        [Test]
        public void CohortRef_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().CohortRef, Is.EqualTo(_csvRecords.First().CohortRef));
                Assert.That(_result.CsvRecords.Last().CohortRef, Is.EqualTo(_csvRecords.Last().CohortRef));
            });
        }

        [Test]
        public void ULN_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().Uln, Is.EqualTo(_csvRecords.First().ULN));
                Assert.That(_result.CsvRecords.Last().Uln, Is.EqualTo(_csvRecords.Last().ULN));
            });
        }

        [Test]
        public void FamilyName_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().LastName, Is.EqualTo(_csvRecords.First().FamilyName));
                Assert.That(_result.CsvRecords.Last().LastName, Is.EqualTo(_csvRecords.Last().FamilyName));
            });
        }

        [Test]
        public void GivenNames_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().FirstName, Is.EqualTo(_csvRecords.First().GivenNames));
                Assert.That(_result.CsvRecords.Last().FirstName, Is.EqualTo(_csvRecords.Last().GivenNames));
            });
        }

        [Test]
        public void DateOfBirth_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().DateOfBirthAsString, Is.EqualTo(_csvRecords.First().DateOfBirth));
                Assert.That(_result.CsvRecords.Last().DateOfBirthAsString, Is.EqualTo(_csvRecords.Last().DateOfBirth));
            });
        }

        [Test]
        public void StdCode_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().CourseCode, Is.EqualTo(_csvRecords.First().StdCode));
                Assert.That(_result.CsvRecords.Last().CourseCode, Is.EqualTo(_csvRecords.Last().StdCode));
            });
        }

        [Test]
        public void StartDate_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().StartDateAsString, Is.EqualTo(_csvRecords.First().StartDate));
                Assert.That(_result.CsvRecords.Last().StartDateAsString, Is.EqualTo(_csvRecords.Last().StartDate));
            });
        }

        [Test]
        public void EndDate_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().EndDateAsString, Is.EqualTo(_csvRecords.First().EndDate));
                Assert.That(_result.CsvRecords.Last().EndDateAsString, Is.EqualTo(_csvRecords.Last().EndDate));
            });
        }

        [Test]
        public void TotalPrice_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().CostAsString, Is.EqualTo(_csvRecords.First().TotalPrice));
                Assert.That(_result.CsvRecords.Last().CostAsString, Is.EqualTo(_csvRecords.Last().TotalPrice));
            });
        }

        [Test]
        public void EpaOrgId_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().EPAOrgId, Is.EqualTo(_csvRecords.First().EPAOrgID));
                Assert.That(_result.CsvRecords.Last().EPAOrgId, Is.EqualTo(_csvRecords.Last().EPAOrgID));
            });
        }

        [Test]
        public void ProviderId_Is_Mapped()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_result.CsvRecords.First().ProviderId, Is.EqualTo(1));
                Assert.That(_result.CsvRecords.Last().ProviderId, Is.EqualTo(1));
            });
        }
    }
}
