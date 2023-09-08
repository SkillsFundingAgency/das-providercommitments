using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi
{
    [TestFixture]
    public class OuterApiServiceTests
    {
        const string Headers = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy";

        private OuterApiService _outerApiService;
        private Mock<IOuterApiClient> _outerApiClientMock;
        private string _fileContent;
        private long _providerId;
        private FileUploadLogResponse _reponse;
        private IFormFile _file;
        private Fixture _fixture;
        private List<CsvRecord> _csvList;
        private string _fileName = "test.csv";

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();
            _csvList = new List<CsvRecord>();
            PopulateCsvList();
            _providerId = _fixture.Create<long>();
            _reponse = _fixture.Create<FileUploadLogResponse>();

            _fileContent = Headers + Environment.NewLine +
                           "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine +
                           "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,false,,";

            CreateFile();
            _outerApiClientMock = new Mock<IOuterApiClient>();
            _outerApiClientMock.Setup(x => x.Post<FileUploadLogResponse>(It.IsAny<PostFileUploadLogRequest>()))
                .ReturnsAsync(_reponse);
            _outerApiService = new OuterApiService(_outerApiClientMock.Object);
        }

        [Test]
        public async Task RplCountIsCorrect()
        {
            await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            _outerApiClientMock.Verify(x=>x.Post<FileUploadLogResponse>(It.Is<PostFileUploadLogRequest>(p=> ((FileUploadLogRequest)p.Data).RplCount == 6)));
        }

        [Test]
        public async Task RowCountIsCorrect()
        {
            await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            _outerApiClientMock.Verify(x => x.Post<FileUploadLogResponse>(It.Is<PostFileUploadLogRequest>(p => ((FileUploadLogRequest)p.Data).RowCount == 11)));
        }

        [Test]
        public async Task FilenameIsCorrect()
        {
            await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            _outerApiClientMock.Verify(x => x.Post<FileUploadLogResponse>(It.Is<PostFileUploadLogRequest>(p => ((FileUploadLogRequest)p.Data).Filename == _fileName)));
        }

        [Test]
        public async Task ProviderIdIsCorrect()
        {
            await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            _outerApiClientMock.Verify(x => x.Post<FileUploadLogResponse>(It.Is<PostFileUploadLogRequest>(p => ((FileUploadLogRequest)p.Data).ProviderId == _providerId)));
        }

        [Test]
        public async Task FileContentIsCorrect()
        {
            await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            _outerApiClientMock.Verify(x => x.Post<FileUploadLogResponse>(It.Is<PostFileUploadLogRequest>(p => ((FileUploadLogRequest)p.Data).FileContent == _fileContent)));
        }

        [Test]
        public async Task ReturnedIdIsCorrect()
        {
            var id = await _outerApiService.CreateFileUploadLog(_providerId, _file, _csvList);
            id.Should().Be(_reponse.LogId);
        }

        private void PopulateCsvList()
        {
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "true").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "True").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "TRUE").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "yes").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "Yes").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "1").Create());

            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "false").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "False").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "No").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "no").Create());
            _csvList.Add(_fixture.Build<CsvRecord>().With(x => x.RecognisePriorLearning, "0").Create());
        }


        private void CreateFile()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(_fileContent);
            writer.Flush();
            stream.Position = 0;

            _file = new FormFile(stream, 0, stream.Length, "id_from_form", _fileName);
        }
    }
}
