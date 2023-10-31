using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Authorization;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    public class WhenMappingFileUploadReviewRequestToReviewViewModel
    {
        private WhenMappingFileUploadReviewRequestToReviewViewModelFixture _fixture;

        [SetUp]
        public void Arrange()
        {
            _fixture = new WhenMappingFileUploadReviewRequestToReviewViewModelFixture();
        }


        [TestCase("Employer1", "Employer1Name")]
        [TestCase("Employer2", "Employer2Name")]
        public async Task LegalEntityNameIsMappedCorrectly(string agreementId, string legalEntityName)
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyLegalEntityNameIsMappedCorrectly(agreementId, legalEntityName);
        }

        [TestCase("Employer1")]
        [TestCase("Employer2")]
        public async Task AgreementIdIsMappedCorrectly(string agreementId)
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyAgreementIdIsMappedCorrectly(agreementId);
        }

        [Test]
        public async Task CorrectNumberOfEmployersAreMapped()
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyCorrectNumberOfEmployersAreMapped();
        }

        [TestCase("Employer1", "Cohort1")]
        [TestCase("Employer1", "Cohort2")]
        [TestCase("Employer2", "Cohort3")]
        [TestCase("Employer2", "Cohort4")]
        public async Task CohortReferenceIsMappedCorrectly(string agreementId, string cohortRef)
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyCohortReferenceIsMappedCorrectly(agreementId, cohortRef);
        }

        [TestCase("Employer1", "Cohort1", 3)]
        [TestCase("Employer1", "Cohort2", 3)]
        [TestCase("Employer2", "Cohort3", 1)]
        [TestCase("Employer2", "Cohort4", 2)]
        public async Task NumberOfApprenticesAreMappedCorrectly(string agreementId, string cohortRef, int numberOfApprentices)
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyNumberOfApprenticesAreMappedCorrectly(agreementId, cohortRef, numberOfApprentices);
        }

        [TestCase("Employer1", "Cohort1", 3000)]
        [TestCase("Employer1", "Cohort2", 600)]
        [TestCase("Employer2", "Cohort3", 400)]
        [TestCase("Employer2", "Cohort4", 1000)]
        public async Task TotalCostIsMappedCorrectly(string agreementId, string cohortRef, int totalCost)
        {
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyTotalCostIsMappedCorrectly(agreementId, cohortRef, totalCost);
        }

        [TestCase("Employer1", "Cohort1", 6)]
        [TestCase("Employer1", "Cohort2", 6)]
        [TestCase("Employer2", "Cohort3", 4)]
        [TestCase("Employer2", "Cohort4", 5)]
        public async Task NumberOfApprenticesWithDraftApprenticesAreMappedCorrectly(string agreementId, string cohortRef, int numberOfApprentices)
        {
            //Arrange
            _fixture.SetupDraftApprenticeships();

            //Act            
            await _fixture.WithDefaultData().Action();
            
            //Assert
            _fixture.VerifyNumberOfApprenticesAreMappedCorrectly(agreementId, cohortRef, numberOfApprentices);
        }

        [TestCase("Employer1", "Cohort1", 3300)]
        [TestCase("Employer1", "Cohort2", 900)]
        [TestCase("Employer2", "Cohort3", 700)]
        [TestCase("Employer2", "Cohort4", 1300)]
        public async Task TotalCostWithDraftApprenticesAreMappedCorrectly(string agreementId, string cohortRef, int totalCost)
        {   
            //Arrange
            _fixture.SetupDraftApprenticeships();
            
            //Act
            await _fixture.WithDefaultData().Action();

            //Assert
            _fixture.VerifyTotalCostIsMappedCorrectly(agreementId, cohortRef, totalCost);
        }

        public class WhenMappingFileUploadReviewRequestToReviewViewModelFixture
        {
            private readonly Fixture _fixture;
            private readonly FileUploadReviewRequestToReviewViewModelMapper _sut;
            private readonly FileUploadReviewRequest _request;
            private readonly Mock<IOuterApiService> _commitmentApiClient;
            private readonly List<CsvRecord> _csvRecords;
            private FileUploadReviewViewModel _result;
            private GetDraftApprenticeshipsResult _draftApprenticeshipsResponse;
            
            public WhenMappingFileUploadReviewRequestToReviewViewModelFixture()
            {
                _fixture = new Fixture();
                _csvRecords = new List<CsvRecord>();

                _request = _fixture.Create<FileUploadReviewRequest>();
                var accountLegalEntityEmployer1 = _fixture.Build<GetAccountLegalEntityQueryResult>()
                                                            .With(x => x.LegalEntityName, "Employer1Name").Create();

                var accountLegalEntityEmployer2 = _fixture.Build<GetAccountLegalEntityQueryResult>()
                                                            .With(x => x.LegalEntityName, "Employer2Name").Create();

                var encodingService = new Mock<IEncodingService>();
                encodingService.Setup(x => x.Decode("Employer1", EncodingType.PublicAccountLegalEntityId)).Returns(1);
                encodingService.Setup(x => x.Decode("Employer2", EncodingType.PublicAccountLegalEntityId)).Returns(2);

                _commitmentApiClient = new Mock<IOuterApiService>();
                _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(1)).ReturnsAsync(accountLegalEntityEmployer1);
                _commitmentApiClient.Setup(x => x.GetAccountLegalEntity(2)).ReturnsAsync(accountLegalEntityEmployer2);               

                var cacheService = new Mock<ICacheService>();
                cacheService.Setup(x => x.GetFromCache<List<CsvRecord>>(_request.CacheRequestId.ToString())).ReturnsAsync(_csvRecords);
                
                var routeData = new RouteData();
                var routingFeature = new Mock<IRoutingFeature>();
                var httpContextAccessor = new Mock<IHttpContextAccessor>();
                routingFeature.Setup(f => f.RouteData).Returns(routeData);
                var featureCollection = new Mock<IFeatureCollection>();
                featureCollection.Setup(f => f.Get<IRoutingFeature>()).Returns(routingFeature.Object);

                var context = new Mock<HttpContext>();
                context.Setup(c => c.Features).Returns(featureCollection.Object);
                context.Setup(c => c.Request.Query).Returns(new QueryCollection());
                context.Setup(c => c.Request.Form).Returns(new FormCollection(new Dictionary<string, StringValues>()));
                httpContextAccessor.Setup(c => c.HttpContext).Returns(context.Object);

                var policyAuthorizationWrapper = new Mock<IPolicyAuthorizationWrapper>();
                policyAuthorizationWrapper.Setup(x => x.IsAuthorized(It.IsAny<ClaimsPrincipal>(), It.IsAny<string>())).ReturnsAsync(true);

                _sut = new FileUploadReviewRequestToReviewViewModelMapper(Mock.Of<ILogger<FileUploadReviewRequestToReviewViewModelMapper>>(), _commitmentApiClient.Object, 
                    cacheService.Object, encodingService.Object, policyAuthorizationWrapper.Object, httpContextAccessor.Object);
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
                _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer1", "Cohort1", 1000));
                // This will create three csv records, with total cost of 600 (3 *200)
                _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer1", "Cohort2", 200));
                // This will create three csv records, with total cost of 400 (1 *200)
                _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer2", "Cohort3", 400, 1));
                // This will create three csv records, with total cost of 1000 (2 *500)
                _csvRecords.AddRange(CreateCsvRecords(_fixture, "Employer2", "Cohort4", 500, 2));

                return this;
            }

            internal WhenMappingFileUploadReviewRequestToReviewViewModelFixture WithIncompleteData()
            {
                _commitmentApiClient.Setup(x => x.GetCohort(It.IsAny<long>()))
                    .ReturnsAsync(new GetCohortResult { IsCompleteForProvider = false });

                return this;
            }

            internal WhenMappingFileUploadReviewRequestToReviewViewModelFixture SetupDraftApprenticeships()
            {
                _draftApprenticeshipsResponse = _fixture.Create<GetDraftApprenticeshipsResult>();
                foreach (var item in _draftApprenticeshipsResponse.DraftApprenticeships)
                {
                    item.Cost = 100;
                }
                _commitmentApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>()))
                 .ReturnsAsync(_draftApprenticeshipsResponse);

                _commitmentApiClient.Setup(x => x.GetCohort(It.IsAny<long>()))
                    .ReturnsAsync(new GetCohortResult { IsCompleteForProvider = true });

                return this;
            }

            internal void VerifyCorrectNumberOfEmployersAreMapped()
            {
                Assert.AreEqual(2, _result.EmployerDetails.Count());
            }

            internal void VerifyAgreementIdIsMappedCorrectly(string agreementId)
            {
                var employer = _result.EmployerDetails.Where(x => x.AgreementId == agreementId).ToList();

                Assert.AreEqual(1, employer.Count);
            }

            public void VerifyLegalEntityNameIsMappedCorrectly(string agreementId, string legalEntityName)
            {
                var employer = _result.EmployerDetails.Where(x => x.AgreementId == agreementId).ToList();

                Assert.AreEqual(1, employer.Count());
                Assert.AreEqual(legalEntityName, employer.First().LegalEntityName);
            }

            internal void VerifyCohortReferenceIsMappedCorrectly(string agreementId, string cohortRef)
            {
                var employer = _result.EmployerDetails.First(x => x.AgreementId == agreementId);
                Assert.AreEqual(1, employer.CohortDetails.Count(x => x.CohortRef == cohortRef));
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
