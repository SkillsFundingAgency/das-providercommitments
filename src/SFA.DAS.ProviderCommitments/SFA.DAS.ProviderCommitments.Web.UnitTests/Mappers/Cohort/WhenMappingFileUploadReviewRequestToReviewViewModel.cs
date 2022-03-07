using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    public class WhenMappingFileUploadReviewRequestToReviewViewModel
    {
        [TestCase("Employer1", "Employer1Name")]
        [TestCase("Employer2", "Employer2Name")]
        public async Task EmployerNameIsMappedCorrectly(string agreementId, string employerName)
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyEmployerNameIsMappedCorrectly(agreementId, employerName);
        }

        [TestCase("Employer1")]
        [TestCase("Employer2")]
        public async Task AgrrementIdIsMappedCorrectly(string agreementId)
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyAgreementIdIsMappedCorrectly(agreementId);
        }

        [Test]
        public async Task CorrectNumberOfEmployersAreMapped()
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyCorrectNumberOfEmployersAreMapped();
        }

        [TestCase("Employer1", "Cohort1")]
        [TestCase("Employer1", "Cohort2")]
        [TestCase("Employer2", "Cohort3")]
        [TestCase("Employer2", "Cohort4")]
        public async Task CohortReferenceIsMappedCorrectly(string agreementId, string cohortRef)
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyCohortReferenceIsMappedCorrectly(agreementId, cohortRef);
        }

        [TestCase("Employer1", "Cohort1", 3)]
        [TestCase("Employer1", "Cohort2", 3)]
        [TestCase("Employer2", "Cohort3", 1)]
        [TestCase("Employer2", "Cohort4", 2)]
        public async Task NumberOfApprenticesAreMappedCorrectly(string agreementId, string cohortRef, int numberOfApprentices)
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyNumberOfApprenticesAreMappedCorrectly(agreementId, cohortRef, numberOfApprentices);
        }

        [TestCase("Employer1", "Cohort1", 3000)]
        [TestCase("Employer1", "Cohort2", 600)]
        [TestCase("Employer2", "Cohort3", 400)]
        [TestCase("Employer2", "Cohort4", 1000)]
        public async Task TotalCostIsMappedCorrectly(string agreementId, string cohortRef, int totalCost)
        {
            var fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
            await fixture.WithDefaultData().Action();

            fixture.VerifyTotalCostIsMappedCorrectly(agreementId, cohortRef, totalCost);
        }

        public class WhenMappingFileUploadReviewRequestToReviewViewModelFixture
        {
            private Fixture fixture;
            private FileUploadReviewRequestToReviewViewModelMapper _sut;
            private FileUploadReviewRequest _request;
            private Mock<IEncodingService> _encodingService;
            private Mock<ICommitmentsApiClient> _commitmentApiClient;
            private Mock<ICacheService> _cacheService;
            private List<CsvRecord> _csvRecords;
            private FileUploadReviewViewModel _result;
            private readonly Mock<IPolicyAuthorizationWrapper> _policyAuthorizationWrapper;
            private Mock<IHttpContextAccessor> _httpContextAccessor;
            private RouteData _routeData;
            private Mock<IRoutingFeature> _routingFeature;

            public WhenMappingFileUploadReviewRequestToReviewViewModelFixture()
            {
                fixture = new Fixture();
                _csvRecords = new List<CsvRecord>();

                _request = fixture.Create<FileUploadReviewRequest>();
                var accountLegalEntityEmployer1 = fixture.Build<AccountLegalEntityResponse>()
                                                            .With(x => x.AccountName, "Employer1Name").Create();

                var accountLegalEntityEmployer2 = fixture.Build<AccountLegalEntityResponse>()
                                                            .With(x => x.AccountName, "Employer2Name").Create();

                _encodingService = new Mock<IEncodingService>();
                _encodingService.Setup(x => x.Decode("Employer1", EncodingType.PublicAccountLegalEntityId)).Returns(1);
                _encodingService.Setup(x => x.Decode("Employer2", EncodingType.PublicAccountLegalEntityId)).Returns(2);

                _commitmentApiClient = new Mock<ICommitmentsApiClient>();
                _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(1, It.IsAny<CancellationToken>())).ReturnsAsync(accountLegalEntityEmployer1);
                _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(2, It.IsAny<CancellationToken>())).ReturnsAsync(accountLegalEntityEmployer2);

                _cacheService = new Mock<ICacheService>();
                _cacheService.Setup(x => x.GetFromCache<List<CsvRecord>>(_request.CacheRequestId.ToString())).ReturnsAsync(_csvRecords);
                
                _routeData = new RouteData();
                _routingFeature = new Mock<IRoutingFeature>();
                _httpContextAccessor = new Mock<IHttpContextAccessor>();
                _routingFeature.Setup(f => f.RouteData).Returns(_routeData);
                var featureCollection = new Mock<IFeatureCollection>();
                featureCollection.Setup(f => f.Get<IRoutingFeature>()).Returns(_routingFeature.Object);

                var context = new Mock<HttpContext>();
                context.Setup(c => c.Features).Returns(featureCollection.Object);
                context.Setup(c => c.Request.Query).Returns(new QueryCollection());
                context.Setup(c => c.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
                _httpContextAccessor.Setup(c => c.HttpContext).Returns(context.Object);

                _policyAuthorizationWrapper = new Mock<IPolicyAuthorizationWrapper>();
                _policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<string>())).ReturnsAsync(true);

                _sut = new FileUploadReviewRequestToReviewViewModelMapper(Mock.Of<ILogger<FileUploadReviewRequestToReviewViewModelMapper>>(), _commitmentApiClient.Object, 
                    _cacheService.Object, _encodingService.Object, _policyAuthorizationWrapper.Object, _httpContextAccessor.Object);
            }

            public async Task Action() =>  _result = await _sut.Map(_request);

            /// <summary>
            /// This will create a list of three CsvRecords, with the passed agreementId, cohortRef and price.
            /// If price is empty, default value of 1000 will be used.
            /// </summary>
            /// <param name="fixture"></param>
            /// <param name="employerAgreementId"></param>
            /// <param name="cohortRef"></param>
            /// <param name="price"></param>
            /// <returns></returns>
            private static List<CsvRecord> CreateCsvRecords(Fixture fixture, string employerAgreementId, string cohortRef, int price = 1000, int numberOfApprentices = 3)
            {
               var list = fixture.Build<CsvRecord>()
                    .With(x => x.AgreementId, employerAgreementId)
                    .With(x => x.CohortRef, cohortRef)
                    .With(x => x.TotalPrice ,price.ToString())
                    .CreateMany(numberOfApprentices)
                    .ToList();
                return list;
            }

            internal WhenMappingFileUploadReviewRequestToReviewViewModelFixture WithDefaultData()
            {
                // This will create three csv records, with total cost of 3000 (3*1000)
                _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer1", "Cohort1", 1000));
                // This will create three csv records, with total cost of 600 (3 *200)
                _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer1", "Cohort2", 200));
                // This will create three csv records, with total cost of 400 (1 *200)
                _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer2", "Cohort3", 400, 1));
                // This will create three csv records, with total cost of 1000 (2 *500)
                _csvRecords.AddRange(CreateCsvRecords(fixture, "Employer2", "Cohort4", 500, 2));

                return this;
            }

            internal void VerifyCorrectNumberOfEmployersAreMapped()
            {
                Assert.AreEqual(2, _result.EmployerDetails.Count());
            }

            internal void VerifyAgreementIdIsMappedCorrectly(string agreementId)
            {
                var employer = _result.EmployerDetails.Where(x => x.AgreementId == agreementId).ToList();

                Assert.AreEqual(1, employer.Count());
            }

            public void VerifyEmployerNameIsMappedCorrectly(string agreementId, string employerName)
            {
                var employer = _result.EmployerDetails.Where(x => x.AgreementId == agreementId).ToList();

                Assert.AreEqual(1, employer.Count());
                Assert.AreEqual(employerName, employer.First().EmployerName);
            }

            internal void VerifyCohortReferenceIsMappedCorrectly(string agreementId, string cohortRef)
            {
                var employer = _result.EmployerDetails.Where(x => x.AgreementId == agreementId).First();
                Assert.AreEqual(1, employer.CohortDetails.Where(x => x.CohortRef == cohortRef).Count());
            }

            internal void VerifyNumberOfApprenticesAreMappedCorrectly(string agreementId, string cohortRef, int numberOfApprentices)
            {
                var employer = _result.EmployerDetails.First(x => x.AgreementId == agreementId);
                var cohort = employer.CohortDetails.First(x => x.CohortRef == cohortRef);
                Assert.AreEqual(numberOfApprentices, cohort.NumberOfApprentices);
            }

            internal void VerifyTotalCostIsMappedCorrectly(string agreementId, string cohortRef, int totalCost)
            {
                var employer = _result.EmployerDetails.First(x => x.AgreementId == agreementId);
                var cohort = employer.CohortDetails.First(x => x.CohortRef == cohortRef);
                Assert.AreEqual(totalCost, cohort.TotalCost);
            }
        }
    }
}
