using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.IO;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Models.Cohort
{
    [TestFixture]
    public class BulkUploadFileParserTests
    {
        BulkUploadFileParser _bulkUploadFileParser;
        Mock<IEncodingService> _encodingService;
        string _fileContent;
        long _proivderId;
        IFormFile _file;

       [SetUp]
        public void Setup()
        {
            _proivderId = 1;
            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.CohortReference)).Returns(100);
            _encodingService.Setup(x => x.Decode(It.IsAny<string>(), EncodingType.PublicAccountLegalEntityId)).Returns(200);
            _bulkUploadFileParser = new BulkUploadFileParser(_encodingService.Object, Mock.Of<ILogger<BulkUploadFileParser>>());
            _fileContent = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef" + Environment.NewLine +
                            "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768" + Environment.NewLine +
                            "P9DD4P, XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657";

            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_fileContent);
            writer.Flush();
            stream.Position = 0;

            _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        }

        [Test]
        public void CohortRefParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(100, result.BulkUploadDraftApprenticeships.First().CohortId);
            Assert.AreEqual(100, result.BulkUploadDraftApprenticeships.Last().CohortId);
        }

        [Test]
        public void AgreementIDParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(200, result.BulkUploadDraftApprenticeships.First().LegalEntityId);
            Assert.AreEqual(200, result.BulkUploadDraftApprenticeships.Last().LegalEntityId);
        }

        [Test]
        public void ULNParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("8652496047", result.BulkUploadDraftApprenticeships.First().Uln);
            Assert.AreEqual("6347198567", result.BulkUploadDraftApprenticeships.Last().Uln);
        }

        [Test]
        public void FamilyNameParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("Jones", result.BulkUploadDraftApprenticeships.First().LastName);
            Assert.AreEqual("Smith", result.BulkUploadDraftApprenticeships.Last().LastName);
        }

        [Test]
        public void GivenNamesParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("Louise", result.BulkUploadDraftApprenticeships.First().FirstName);
            Assert.AreEqual("Mark", result.BulkUploadDraftApprenticeships.Last().FirstName);
        }


        [Test]
        public void DateOfBirthParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(new DateTime(2000,1,1).Date, result.BulkUploadDraftApprenticeships.First().DateOfBirth.Value.Date);
            Assert.AreEqual(new DateTime(2002, 2, 2).Date, result.BulkUploadDraftApprenticeships.Last().DateOfBirth.Value.Date);
        }

        [Test]
        public void EmailAddressParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("abc1@abc.com", result.BulkUploadDraftApprenticeships.First().Email);
            Assert.AreEqual("abc2@abc.com", result.BulkUploadDraftApprenticeships.Last().Email);
        }

        [Test]
        public void StdCodeParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("57", result.BulkUploadDraftApprenticeships.First().CourseCode);
            Assert.AreEqual("58", result.BulkUploadDraftApprenticeships.Last().CourseCode);
        }

        [Test]
        public void StartDateParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(new DateTime(2017, 5, 1).Date, result.BulkUploadDraftApprenticeships.First().StartDate.Value.Date);
            Assert.AreEqual(new DateTime(2018, 6, 1).Date, result.BulkUploadDraftApprenticeships.Last().StartDate.Value.Date);
        }

        [Test]
        public void EndDateParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(new DateTime(2018, 5, 1).Date, result.BulkUploadDraftApprenticeships.First().EndDate.Value.Date);
            Assert.AreEqual(new DateTime(2019, 6, 1).Date, result.BulkUploadDraftApprenticeships.Last().EndDate.Value.Date);
        }

        [Test]
        public void TotalPriceParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(2000, result.BulkUploadDraftApprenticeships.First().Cost);
            Assert.AreEqual(3333, result.BulkUploadDraftApprenticeships.Last().Cost);
        }

        [Test]
        public void ProviderRefIsParsedCorrectly()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual("CX768", result.BulkUploadDraftApprenticeships.First().ProviderRef);
            Assert.AreEqual("ZB657", result.BulkUploadDraftApprenticeships.Last().ProviderRef);
        }

        [Test]
        public void CorrectNumberOfApprenticeshipMapped()
        {
            var result = _bulkUploadFileParser.CreateApiRequest(_proivderId, _file);
            Assert.AreEqual(2, result.BulkUploadDraftApprenticeships.Count());
        }
    }
}
