using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderRelationships.Api.Client;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    public class WhenMappingTransferSenderRequestToViewModel
    {
        WhenMappingTransferSenderRequestToViewModelFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new WhenMappingTransferSenderRequestToViewModelFixture();
        }

        [Test]
        public void OnlyTheCohortsWithTransferSenderAreMapped()
        {
            _fixture.Map();
            _fixture.Verify_Only_TheCohorts_WithTransferSender_Are_Mapped();
        }

        [Test]
        public void Then_TheCohortReferenceIsMapped()
        {
            _fixture.Map();
            _fixture.Verify_CohortReference_Is_Mapped();
        }

        [Test]
        public void Then_NumberOfApprenticesAreMapped()
        {
            _fixture.Map();
            _fixture.Verify_NumberOfApprentices_Are_Mapped();
        }

        [Test]
        public void Then_OrderBy_OnDateTransfered_Correctly()
        {
            _fixture.Map();
            _fixture.Verify_Ordered_By_OnDateTransfered();
        }

        [Test]
        public void Then_OrderBy_OnDateCreated_Correctly()
        {
            _fixture.MakeTheMessagesNull().SetCreatedOn();
            _fixture.Map();
            _fixture.Verify_Ordered_By_OnDateCreated();
        }

        [Test]
        public void Then_OrderBy_LatestMessageByEmployer_Correctly()
        {
            _fixture.MakeTheMessagesNull().SetLatestMessageFromEmployer();
            _fixture.Map();
            _fixture.Verify_Ordered_By_LatestMessageByEmployer();
        }

        [Test]
        public void Then_OrderBy_LatestMessageByProvider_Correctly()
        {
            _fixture.MakeTheMessagesNull().SetLatestMessageFromProvider();
            _fixture.Map();
            _fixture.Verify_Ordered_By_LatestMessageByProvider();
        }

        [Test]
        public void Then_DateCreated_IsMapped_Correctly() 
        { 
            _fixture.Map();
            _fixture.Verify_DateCreated_Is_Mapped();
        }
        
        [TestCase("", false, "2_Encoded", "1_Encoded")]
        [TestCase("Employer", false, "2_Encoded", "1_Encoded")]
        [TestCase("Employer", true, "1_Encoded", "2_Encoded")]
        [TestCase("CohortReference", false, "1_Encoded", "2_Encoded")]
        [TestCase("CohortReference", true, "2_Encoded", "1_Encoded")]
        [TestCase("DateSentToEmployer", false, "2_Encoded", "1_Encoded")]
        [TestCase("DateSentToEmployer", true, "2_Encoded", "1_Encoded")]
        public void Then_Sort_IsApplied_Correctly(string sortField, bool reverse, string expectedFirstId, string expectedLastId)
        {
            _fixture.WithSortApplied(sortField, reverse);
            _fixture.Map();
            _fixture.Verify_Sort_IsApplied(expectedFirstId, expectedLastId);
        }
    }

    public class WhenMappingTransferSenderRequestToViewModelFixture
    {
        public Mock<IEncodingService> EncodingService { get; set; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; set; }
        public Mock<IProviderRelationshipsApiClient> ProviderRelationshipsApiClient { get; }
        public Mock<IPasAccountApiClient> PasAccountApiClient { get; set; }
        public Mock<IUrlHelper> UrlHelper { get; }
        public CohortsByProviderRequest WithTransferSenderRequest { get; set; }
        public GetCohortsResponse GetCohortsResponse { get; set; }
        public WithTransferSenderRequestViewModelMapper Mapper { get; set; }
        public WithTransferSenderViewModel WithTransferSenderViewModel { get; set; }
        public long ProviderId => 1;
        public DateTime Now = DateTime.Now;

        public WhenMappingTransferSenderRequestToViewModelFixture()
        {
            WithTransferSenderRequest = new CohortsByProviderRequest { ProviderId = ProviderId };
            EncodingService = new Mock<IEncodingService>();
            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();

            GetCohortsResponse = CreateGetCohortsResponse();

            CommitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(GetCohortsResponse);
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

            ProviderRelationshipsApiClient = new Mock<IProviderRelationshipsApiClient>();

            PasAccountApiClient = new Mock<IPasAccountApiClient>();
            PasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

            UrlHelper = new Mock<IUrlHelper>();
            UrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");

            Mapper = new WithTransferSenderRequestViewModelMapper(CommitmentsApiClient.Object, ProviderRelationshipsApiClient.Object, UrlHelper.Object, PasAccountApiClient.Object, EncodingService.Object);
        }

        public WhenMappingTransferSenderRequestToViewModelFixture Map()
        {
            WithTransferSenderViewModel = Mapper.Map(WithTransferSenderRequest).Result;
            return this;
        }

        public WhenMappingTransferSenderRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
        {
            WithTransferSenderRequest.SortField = sortField;
            WithTransferSenderRequest.ReverseSort = reverse;
            return this;
        }

        public void Verify_Only_TheCohorts_WithTransferSender_Are_Mapped()
        {
            Assert.AreEqual(2, WithTransferSenderViewModel.Cohorts.Count());

            Assert.IsNotNull(GetCohortInTransferSenderViewModel(1));
            Assert.IsNotNull(GetCohortInTransferSenderViewModel(2));
        }

        public void Verify_CohortReference_Is_Mapped()
        {
            EncodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

            Assert.AreEqual("1_Encoded", GetCohortInTransferSenderViewModel(1).CohortReference);
            Assert.AreEqual("2_Encoded", GetCohortInTransferSenderViewModel(2).CohortReference);
        }

        public void Verify_NumberOfApprentices_Are_Mapped()
        {
            Assert.AreEqual(100, GetCohortInTransferSenderViewModel(1).NumberOfApprentices);
            Assert.AreEqual(200, GetCohortInTransferSenderViewModel(2).NumberOfApprentices);
        }

        public void Verify_Ordered_By_OnDateTransfered()
        {
            Assert.AreEqual("2_Encoded", WithTransferSenderViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual("1_Encoded", WithTransferSenderViewModel.Cohorts.Last().CohortReference);
        }

        public void Verify_Ordered_By_OnDateCreated()
        {
            Assert.AreEqual("2_Encoded", WithTransferSenderViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual("1_Encoded", WithTransferSenderViewModel.Cohorts.Last().CohortReference);
        }

        public void Verify_Ordered_By_LatestMessageByEmployer()
        {
            Assert.AreEqual("2_Encoded", WithTransferSenderViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual("1_Encoded", WithTransferSenderViewModel.Cohorts.Last().CohortReference);
        }

        public void Verify_Ordered_By_LatestMessageByProvider()
        {
            Assert.AreEqual("2_Encoded", WithTransferSenderViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual("1_Encoded", WithTransferSenderViewModel.Cohorts.Last().CohortReference);
        }

        public void SetOnlyOneTransferSender()
        {
            foreach (var resp in GetCohortsResponse.Cohorts)
            {
                resp.TransferSenderId = 1;
                resp.TransferSenderName = "TransferSender1";
            }
        }

        public void Verify_DateCreated_Is_Mapped()
        {
            Assert.AreEqual(Now.AddMinutes(-7), WithTransferSenderViewModel.Cohorts.First().DateSentToEmployer);
            Assert.AreEqual(Now.AddMinutes(-10), WithTransferSenderViewModel.Cohorts.Last().DateSentToEmployer);
        }

        public void Verify_Sort_IsApplied(string firstId, string lastId)
        {
            Assert.AreEqual(firstId, WithTransferSenderViewModel.Cohorts.First().CohortReference);
            Assert.AreEqual(lastId, WithTransferSenderViewModel.Cohorts.Last().CohortReference);
        }

        public WhenMappingTransferSenderRequestToViewModelFixture MakeTheMessagesNull()
        {
            foreach (var c in GetCohortsResponse.Cohorts)
            {
                c.LatestMessageFromEmployer = c.LatestMessageFromProvider = null;
            }

            return this;
        }

        public WhenMappingTransferSenderRequestToViewModelFixture SetCreatedOn()
        {
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 1).CreatedOn = Now.AddMinutes(-5);
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 2).CreatedOn = Now.AddMinutes(-7);
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 3).CreatedOn = Now.AddMinutes(-9);
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 4).CreatedOn = Now.AddMinutes(-10);
            return this;
        }

        public WhenMappingTransferSenderRequestToViewModelFixture SetLatestMessageFromEmployer()
        {
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 1).LatestMessageFromEmployer = new Message("1st Message", Now.AddMinutes(-6));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 2).LatestMessageFromEmployer = new Message("2nd Message", Now.AddMinutes(-7));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 3).LatestMessageFromEmployer = new Message("3rd Message", Now.AddMinutes(-8));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 4).LatestMessageFromEmployer = new Message("4th Message", Now.AddMinutes(-9));

            return this;
        }

        public WhenMappingTransferSenderRequestToViewModelFixture SetLatestMessageFromProvider()
        {
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 1).LatestMessageFromProvider = new Message("1st Message", Now.AddMinutes(-6));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 2).LatestMessageFromProvider = new Message("2nd Message", Now.AddMinutes(-7));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 3).LatestMessageFromProvider = new Message("3rd Message", Now.AddMinutes(-8));
            GetCohortsResponse.Cohorts.First(x => x.CohortId == 4).LatestMessageFromProvider = new Message("4th Message", Now.AddMinutes(-9));

            return this;
        }

        private GetCohortsResponse CreateGetCohortsResponse()
        {
            IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
            {
                new()
                {
                    CohortId = 1,
                    AccountId = 1,
                    ProviderId = 1,
                    TransferSenderId = 1,
                    TransferSenderName = "TransferSender1",
                    LegalEntityName = "2",
                    ProviderName = "Provider1",
                    NumberOfDraftApprentices = 100,
                    IsDraft = false,
                    WithParty = Party.TransferSender,
                    LatestMessageFromEmployer = new Message("this is the last message from Employer", Now.AddMinutes(-10)),
                    LatestMessageFromProvider = new Message("This is latestMessage from provider", Now.AddMinutes(-11))
                },
                new()
                {
                    CohortId = 2,
                    AccountId = 1,
                    ProviderId = 2,
                    TransferSenderId = 2,
                    TransferSenderName = "TransferSender2",
                    LegalEntityName = "1",
                    ProviderName = "Provider2",
                    NumberOfDraftApprentices = 200,
                    IsDraft = false,
                    WithParty = Party.TransferSender,
                    CreatedOn = Now.AddMinutes(-8),
                    LatestMessageFromProvider = new Message("This is latestMessage from provider", Now.AddMinutes(-8)),
                    LatestMessageFromEmployer = new Message("This is latestMessage from Employer", Now.AddMinutes(-7))
                },
                new()
                {
                    CohortId = 3,
                    AccountId = 1,
                    ProviderId = 2,
                    TransferSenderId = 2,
                    TransferSenderName = "TransferSender2",
                    LegalEntityName = "4",
                    ProviderName = "Provider3",
                    NumberOfDraftApprentices = 300,
                    IsDraft = false,
                    WithParty = Party.Employer,
                    CreatedOn = Now.AddMinutes(-1)
                },
                 new()
                 {
                    CohortId = 4,
                    AccountId = 1,
                    ProviderId = 4,
                    ProviderName = "Provider4",
                    LegalEntityName = "3",
                    NumberOfDraftApprentices = 400,
                    IsDraft = true,
                    WithParty = Party.Employer,
                    CreatedOn = Now
                },
            };

            return new GetCohortsResponse(cohorts);
        }

        private long GetCohortId(string cohortReference)
        {
            return long.Parse(cohortReference.Replace("_Encoded", ""));
        }

        private WithTransferSenderCohortSummaryViewModel GetCohortInTransferSenderViewModel(long id)
        {
            return WithTransferSenderViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
        }
    }
}
