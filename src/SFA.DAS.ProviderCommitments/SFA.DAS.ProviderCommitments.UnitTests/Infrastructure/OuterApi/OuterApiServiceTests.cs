using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;
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
        private List<BulkUploadValidationError> _errors;
        
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
            _errors = _fixture.CreateMany<BulkUploadValidationError>().ToList();
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

        [Test]
        public async Task VerifyAddValidationMessagesToFileUploadLogIsCalledAsExpected()
        {
            await _outerApiService.AddValidationMessagesToFileUploadLog(_providerId, 1234, _errors);
            
            var expectedErrorContent = "Validation failure \r\n" + JsonConvert.SerializeObject(_errors);
            _outerApiClientMock.Verify(x=>x.Put<object>(It.Is<PutFileUploadUpdateLogRequest>(p=>p.LogId == 1234 && ((FileUploadUpdateLogWithErrorContentRequest)p.Data).ErrorContent == expectedErrorContent)));
        }

        [Test]
        public async Task VerifyAddUnhandledExceptionToFileUploadLogIsCalledAsExpected()
        {
            await _outerApiService.AddUnhandledExceptionToFileUploadLog(_providerId, 1234, "Bang");

            var expectedErrorContent = "Unhandled exception \r\n" + "Bang";
            _outerApiClientMock.Verify(x => x.Put<object>(It.Is<PutFileUploadUpdateLogRequest>(p => p.LogId == 1234 && ((FileUploadUpdateLogWithErrorContentRequest)p.Data).ErrorContent == expectedErrorContent)));
        }

        [Test]
        public async Task VerifyBulkUploadAddAndApproveIsCalledAsExpected()
        {
            var request = _fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsRequest>();
            var response = _fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsResult>();
            _outerApiClientMock
                .Setup(x => x.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddAndApproveDraftApprenticeshipsRequest>(p => p.Data == request)))
                .ReturnsAsync(response);

            var result = await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(request);

            result.Should().Be(response);
        }

        [Test]
        public async Task VerifyBulkUploadAddAndApproveRecordsCommitmentsApiBulkUploadModelException()
        {
            var request = _fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsRequest>();
            var exception = new CommitmentsApiBulkUploadModelException(new List<BulkUploadValidationError>());
            _outerApiClientMock
                .Setup(x => x.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddAndApproveDraftApprenticeshipsRequest>(p => p.Data == request)))
                .Throws(exception);

            try
            {
                await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(request);
            }
            catch (CommitmentsApiBulkUploadModelException)
            {
                _outerApiClientMock.Verify(x=>x.Put<object>(It.IsAny<PutFileUploadUpdateLogRequest>()));
            }
        }

        [Test]
        public async Task VerifyBulkUploadAddAndApproveRecordsUnhandledException()
        {
            var request = _fixture.Create<BulkUploadAddAndApproveDraftApprenticeshipsRequest>();
            var exception = new InvalidCastException();
            _outerApiClientMock
                .Setup(x => x.Post<BulkUploadAddAndApproveDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddAndApproveDraftApprenticeshipsRequest>(p => p.Data == request)))
                .Throws(exception);

            try
            {
                await _outerApiService.BulkUploadAddAndApproveDraftApprenticeships(request);
            }
            catch (Exception)
            {
                _outerApiClientMock.Verify(x => x.Put<object>(It.IsAny<PutFileUploadUpdateLogRequest>()));
            }
        }

        [Test]
        public async Task VerifyBulkUploadAddIsCalledAsExpected()
        {
            var request = _fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            var response = _fixture.Create<GetBulkUploadAddDraftApprenticeshipsResult>();
            _outerApiClientMock
                .Setup(x => x.Post<GetBulkUploadAddDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddDraftApprenticeshipsRequest>(p => p.Data == request)))
                .ReturnsAsync(response);

            var result = await _outerApiService.BulkUploadDraftApprenticeships(request);

            result.Should().Be(response);
        }

        [Test]
        public async Task VerifyBulkUploadAddRecordsCommitmentsApiBulkUploadModelException()
        {
            var request = _fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            var exception = new CommitmentsApiBulkUploadModelException(new List<BulkUploadValidationError>());
            _outerApiClientMock
                .Setup(x => x.Post<GetBulkUploadAddDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddDraftApprenticeshipsRequest>(p => p.Data == request)))
                .Throws(exception);

            try
            {
                await _outerApiService.BulkUploadDraftApprenticeships(request);
            }
            catch (CommitmentsApiBulkUploadModelException)
            {
                _outerApiClientMock.Verify(x => x.Put<object>(It.IsAny<PutFileUploadUpdateLogRequest>()));
            }
        }

        [Test]
        public async Task VerifyBulkUploadAddRecordsUnhandledException()
        {
            var request = _fixture.Create<BulkUploadAddDraftApprenticeshipsRequest>();
            var exception = new DivideByZeroException();
            _outerApiClientMock
                .Setup(x => x.Post<GetBulkUploadAddDraftApprenticeshipsResult>(
                    It.Is<PostBulkUploadAddDraftApprenticeshipsRequest>(p => p.Data == request)))
                .Throws(exception);

            try
            {
                await _outerApiService.BulkUploadDraftApprenticeships(request);
            }
            catch (Exception)
            {
                _outerApiClientMock.Verify(x => x.Put<object>(It.IsAny<PutFileUploadUpdateLogRequest>()));
            }
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
