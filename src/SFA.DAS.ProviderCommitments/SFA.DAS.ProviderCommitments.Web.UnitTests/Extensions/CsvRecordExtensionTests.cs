using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Extensions;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Extensions
{
    [TestFixture]   
    public  class CsvRecordExtensionTests
    {
        [Test]
        public void VerifyUlnIsMappedTest()
        {
            //Arrange
            var fixture = new Fixture();
            var csvFileWithData = fixture.Build<CsvRecord>()
                .With(x => x.DateOfBirth, "2000-02-02")
                .With(x => x.StartDate, "2021-03-04")
                .With(x => x.EndDate, "2022-04")
                .With(x => x.TotalPrice, "1000")
                .CreateMany(2).ToList();

            var csvFileWithEmptyDatas = fixture.Build<CsvRecord>()
               .With(x => x.CohortRef, "")
               .With(x => x.AgreementId, "")
               .With(x => x.ULN, "")
               .With(x => x.FamilyName, "")
               .With(x => x.GivenNames, "")
               .With(x => x.DateOfBirth, "")
               .With(x => x.EmailAddress, "")
               .With(x => x.StdCode, "")
               .With(x => x.StartDate, "")
               .With(x => x.EndDate, "")
               .With(x => x.TotalPrice, "")
               .With(x => x.EPAOrgID, "")
               .With(x => x.ProviderRef, "")
               .CreateMany(2).ToList();

            csvFileWithData.AddRange(csvFileWithEmptyDatas);

            //Act
            var result = csvFileWithEmptyDatas.GetEmptyRecords();

            //Assert            
            Assert.AreEqual(2, result.Count());
        }
    }
}
