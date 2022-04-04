using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Queries.BulkUploadValidate;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingFileUploadValidateDataRequestToApiRequestTests
    {
        private FileUploadValidateDataRequestToApiRequest _mapper;
        private BulkUploadValidateApiRequest _result;
        private List<CsvRecord> _csvRecords;
        private FileUploadValidateDataRequest _request;

        [SetUp]
        public async Task Setup()
        {
             _csvRecords = new List<CsvRecord>
            {
                new CsvRecord
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
                new CsvRecord
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

            _mapper = new FileUploadValidateDataRequestToApiRequest();
            _result = await _mapper.Map(_request);
        }

        [Test]
        public void ProviderIdIsMapped()
        {
            Assert.AreEqual(1, _result.ProviderId);
        }

        [Test]
        public void RowNumber_Is_Mapped()
        {
            Assert.AreEqual(1, _result.CsvRecords.First().RowNumber);
            Assert.AreEqual(2, _result.CsvRecords.Last().RowNumber);
        }

        [Test]
        public void ProviderRef_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().ProviderRef, _result.CsvRecords.First().ProviderRef);
            Assert.AreEqual(_csvRecords.Last().ProviderRef, _result.CsvRecords.Last().ProviderRef);
        }

        [Test]
        public void EmailAddress_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().EmailAddress, _result.CsvRecords.First().Email);
            Assert.AreEqual(_csvRecords.Last().EmailAddress, _result.CsvRecords.Last().Email);
        }

        [Test]
        public void AgreementId_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().AgreementId, _result.CsvRecords.First().AgreementId);
            Assert.AreEqual(_csvRecords.Last().AgreementId, _result.CsvRecords.Last().AgreementId);
        }

        [Test]
        public void CohortRef_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().CohortRef, _result.CsvRecords.First().CohortRef);
            Assert.AreEqual(_csvRecords.Last().CohortRef, _result.CsvRecords.Last().CohortRef);
        }

        [Test]
        public void ULN_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().ULN, _result.CsvRecords.First().Uln);
            Assert.AreEqual(_csvRecords.Last().ULN, _result.CsvRecords.Last().Uln);
        }

        [Test]
        public void FamilyName_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().FamilyName, _result.CsvRecords.First().LastName);
            Assert.AreEqual(_csvRecords.Last().FamilyName, _result.CsvRecords.Last().LastName);
        }

        [Test]
        public void GivenNames_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().GivenNames, _result.CsvRecords.First().FirstName);
            Assert.AreEqual(_csvRecords.Last().GivenNames, _result.CsvRecords.Last().FirstName);
        }

        [Test]
        public void DateOfBirth_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().DateOfBirth, _result.CsvRecords.First().DateOfBirthAsString);
            Assert.AreEqual(_csvRecords.Last().DateOfBirth, _result.CsvRecords.Last().DateOfBirthAsString);
        }

        [Test]
        public void StdCode_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().StdCode, _result.CsvRecords.First().CourseCode);
            Assert.AreEqual(_csvRecords.Last().StdCode, _result.CsvRecords.Last().CourseCode);
        }

        [Test]
        public void StartDate_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().StartDate, _result.CsvRecords.First().StartDateAsString);
            Assert.AreEqual(_csvRecords.Last().StartDate, _result.CsvRecords.Last().StartDateAsString);
        }

        [Test]
        public void EndDate_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().EndDate, _result.CsvRecords.First().EndDateAsString);
            Assert.AreEqual(_csvRecords.Last().EndDate, _result.CsvRecords.Last().EndDateAsString);
        }

        [Test]
        public void TotalPrice_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().TotalPrice, _result.CsvRecords.First().CostAsString);
            Assert.AreEqual(_csvRecords.Last().TotalPrice, _result.CsvRecords.Last().CostAsString);
        }

        [Test]
        public void EpaOrgId_Is_Mapped()
        {
            Assert.AreEqual(_csvRecords.First().EPAOrgID, _result.CsvRecords.First().EPAOrgId);
            Assert.AreEqual(_csvRecords.Last().EPAOrgID, _result.CsvRecords.Last().EPAOrgId);
        }


        [Test]
        public void ProviderId_Is_Mapped()
        {
            Assert.AreEqual(1, _result.CsvRecords.First().ProviderId);
            Assert.AreEqual(1, _result.CsvRecords.Last().ProviderId);
        }
    }
}
