using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi
{
    [TestFixture]
    public class OuterApiTests
    {
        const string Headers = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy";

        private OuterApiService _outerApiService;
        private Mock<IOuterApiClient> _outerApiClientMock;
        private string _fileContent;
        private long _proivderId;
        private IFormFile _file;

        [SetUp]
        public void Setup()
        {
            _proivderId = 1;
            _fileContent = Headers + Environment.NewLine +
                           "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine +
                           "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,false,,";

            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_fileContent);
            writer.Flush();
            stream.Position = 0;

            _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
            _outerApiClientMock = new Mock<IOuterApiClient>();
            _outerApiService = new OuterApiService(_outerApiClientMock.Object);
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
        public void RecognisePriorLearningIsParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("true", result.First().RecognisePriorLearning);
            Assert.AreEqual("false", result.Last().RecognisePriorLearning);
        }

        [Test]
        public void DurationReducedByLearningIsParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("12", result.First().DurationReducedBy);
            Assert.AreEqual("", result.Last().DurationReducedBy);
        }

        [Test]
        public void PriceReducedByLearningIsParsedCorrectly()
        {
            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual("99", result.First().PriceReducedBy);
            Assert.AreEqual("", result.Last().PriceReducedBy);
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
            _fileContent = Headers + Environment.NewLine +
                ",,,,,,,,,,,,,,," + Environment.NewLine +
                 "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,,," + Environment.NewLine +
                 "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,,," + Environment.NewLine +
                 ",,,,,,,,,,,,,,,";

            CreateFile();

            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void VerifyNoRowsReturnedFromEmptyFile()
        {
            //Arrange          
            _fileContent = Headers + Environment.NewLine +
                ",,,,,,,,,,,,,,," + Environment.NewLine +
                ",,,,,,,,,,,,,,,";

            CreateFile();

            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void OptionalFieldsMayBeOmitted()
        {
            //Arrange          
            _fileContent = Headers.Replace(",RecognisePriorLearning,DurationReducedBy,PriceReducedBy", "") + Environment.NewLine +
                "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine;

            CreateFile();

            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            result.Should().ContainEquivalentOf(new
            {
                CohortRef = "P9DD4P",
                RecognisePriorLearning = (bool?)null,
                DurationReducedBy = (string)null,
                PriceReducedBy = (string)null,
            });
        }

        [Test]
        public void OptionalFieldsWithoutHeaderName()
        {
            //Arrange          
            _fileContent = Headers.Replace(",RecognisePriorLearning,DurationReducedBy,PriceReducedBy", ",,,") + Environment.NewLine +
                "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine;

            CreateFile();

            var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
            result.Should().ContainEquivalentOf(new
            {
                CohortRef = "P9DD4P",
                RecognisePriorLearning = (bool?)null,
                DurationReducedBy = (string)null,
                PriceReducedBy = (string)null,
            });
        }

        private void CreateFile()
        {
            var fileName = "test.pdf";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_fileContent);
            writer.Flush();
            stream.Position = 0;

            _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
        }
    }
}
