using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.IO;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure
{
    [TestFixture]
    public class BulkUploadFileParserTests
    {
        BulkUploadFileParser _bulkUploadFileParser;
        string _fileContent;
        long _proivderId;
        IFormFile _file;

        [SetUp]
        public void Setup()
        {
            _proivderId = 1;
            _bulkUploadFileParser = new BulkUploadFileParser(Mock.Of<ILogger<BulkUploadFileParser>>());
            _fileContent = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef" + Environment.NewLine +
                            "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768" + Environment.NewLine +
                            "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657";

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
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("P9DD4P", result.First().CohortRef);
            Assert.AreEqual("P9DD4P", result.Last().CohortRef);
        }

        [Test]
        public void AgreementIDParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("XEGE5X", result.First().AgreementId);
            Assert.AreEqual("XEGE5X", result.Last().AgreementId);
        }

        [Test]
        public void ULNParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("8652496047", result.First().ULN);
            Assert.AreEqual("6347198567", result.Last().ULN);
        }

        [Test]
        public void FamilyNameParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("Jones", result.First().FamilyName);
            Assert.AreEqual("Smith", result.Last().FamilyName);
        }

        [Test]
        public void GivenNamesParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("Louise",  result.First().GivenNames);
            Assert.AreEqual("Mark", result.Last().GivenNames);
        }


        [Test]
        public void DateOfBirthParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("2000-01-01", result.First().DateOfBirth);
            Assert.AreEqual("2002-02-02", result.Last().DateOfBirth);
        }

        [Test]
        public void EmailAddressParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("abc1@abc.com", result.First().EmailAddress);
            Assert.AreEqual("abc2@abc.com", result.Last().EmailAddress);
        }

        [Test]
        public void StdCodeParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("57", result.First().StdCode);
            Assert.AreEqual("58", result.Last().StdCode);
        }

        [Test]
        public void StartDateParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("2017-05-03", result.First().StartDate);
            Assert.AreEqual("2018-06-01", result.Last().StartDate);
        }

        [Test]
        public void EndDateParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("2018-05", result.First().EndDate);
            Assert.AreEqual("2019-06", result.Last().EndDate);
        }

        [Test]
        public void TotalPriceParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("2000", result.First().TotalPrice);
            Assert.AreEqual("3333", result.Last().TotalPrice);
        }

        [Test]
        public void ProviderRefIsParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("CX768", result.First().ProviderRef);
            Assert.AreEqual("ZB657", result.Last().ProviderRef);
        }

        [Test]
        public void CorrectNumberOfApprenticeshipMapped()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void VerifyEmptyRowsRemovedFromUploadedFile()
        {
            //Arrange          
            _fileContent = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef" + Environment.NewLine +
                ",,,,,,,,,,,," + Environment.NewLine +
                 "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768" + Environment.NewLine +
                 "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657" + Environment.NewLine +
                 ",,,,,,,,,,,,";


            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual(2, result.Count());
        }
    }
}
