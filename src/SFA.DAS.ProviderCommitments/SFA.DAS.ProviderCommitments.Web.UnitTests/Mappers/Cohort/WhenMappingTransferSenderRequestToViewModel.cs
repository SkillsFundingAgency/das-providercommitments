using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Routing;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Provider;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.ProviderRelationships;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenMappingTransferSenderRequestToViewModel
{
    private WhenMappingTransferSenderRequestToViewModelFixture _fixture;

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
    public void Then_OrderBy_OnDateTransferred_Correctly()
    {
        _fixture.Map();
        _fixture.Verify_Ordered_By_OnDateTransferred();
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
    private readonly Mock<IEncodingService> _encodingService;
    private readonly CohortsByProviderRequest _withTransferSenderRequest;
    private readonly GetCohortsResponse _getCohortsResponse;
    private readonly WithTransferSenderRequestViewModelMapper _mapper;
    private WithTransferSenderViewModel _withTransferSenderViewModel;
    private const long ProviderId = 1;
    private readonly DateTime _now = DateTime.Now;

    public WhenMappingTransferSenderRequestToViewModelFixture()
    {
        _withTransferSenderRequest = new CohortsByProviderRequest { ProviderId = ProviderId };
        _encodingService = new Mock<IEncodingService>();
        var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

        _getCohortsResponse = CreateGetCohortsResponse();

        commitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(_getCohortsResponse);
        _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

        var approvalsOuterApiClient = new Mock<IApprovalsOuterApiClient>();
        approvalsOuterApiClient.Setup(x => x.GetHasRelationshipWithPermission(It.IsAny<long>()))
            .ReturnsAsync(new GetHasRelationshipWithPermissionResponse { HasPermission = true });

        var pasAccountApiClient = new Mock<IPasAccountApiClient>();
        pasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new ProviderAgreement { Status = ProviderAgreementStatus.Agreed });

        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns<UrlActionContext>((ac) => $"http://{ac.Controller}/{ac.Action}/");
        var outerApiClient = new Mock<IOuterApiClient>();
        outerApiClient.Setup(x => x.Get<GetProviderDetailsResponse>(It.IsAny<GetProviderDetailsRequest>())).ReturnsAsync(new GetProviderDetailsResponse() { ProviderStatusTypeId = (int)Enums.ProviderStatusType.Active });
        _mapper = new WithTransferSenderRequestViewModelMapper(commitmentsApiClient.Object, approvalsOuterApiClient.Object, urlHelper.Object, pasAccountApiClient.Object, _encodingService.Object, outerApiClient.Object);
    }

    public WhenMappingTransferSenderRequestToViewModelFixture Map()
    {
        _withTransferSenderViewModel = _mapper.Map(_withTransferSenderRequest).Result;
        return this;
    }

    public WhenMappingTransferSenderRequestToViewModelFixture WithSortApplied(string sortField, bool reverse)
    {
        _withTransferSenderRequest.SortField = sortField;
        _withTransferSenderRequest.ReverseSort = reverse;
        return this;
    }

    public void Verify_Only_TheCohorts_WithTransferSender_Are_Mapped()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.Count().Should().Be(2);
            GetCohortInTransferSenderViewModel(1).Should().NotBeNull();
            GetCohortInTransferSenderViewModel(2).Should().NotBeNull();
        }
    }

    public void Verify_CohortReference_Is_Mapped()
    {
        _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(2));

        using (new AssertionScope())
        {
            GetCohortInTransferSenderViewModel(1).CohortReference.Should().Be("1_Encoded");
            GetCohortInTransferSenderViewModel(2).CohortReference.Should().Be("2_Encoded");
        }
    }

    public void Verify_NumberOfApprentices_Are_Mapped()
    {
        using (new AssertionScope())
        {
            GetCohortInTransferSenderViewModel(1).NumberOfApprentices.Should().Be(100);
            GetCohortInTransferSenderViewModel(2).NumberOfApprentices.Should().Be(200);
        }
    }

    public void Verify_Ordered_By_OnDateTransferred()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().CohortReference.Should().Be("2_Encoded");
            _withTransferSenderViewModel.Cohorts.Last().CohortReference.Should().Be("1_Encoded");
        }
    }

    public void Verify_Ordered_By_OnDateCreated()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().CohortReference.Should().Be("2_Encoded");
            _withTransferSenderViewModel.Cohorts.Last().CohortReference.Should().Be("1_Encoded");
        }
    }

    public void Verify_Ordered_By_LatestMessageByEmployer()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().CohortReference.Should().Be("2_Encoded");
            _withTransferSenderViewModel.Cohorts.Last().CohortReference.Should().Be("1_Encoded");
        }
    }

    public void Verify_Ordered_By_LatestMessageByProvider()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().CohortReference.Should().Be("2_Encoded");
            _withTransferSenderViewModel.Cohorts.Last().CohortReference.Should().Be("1_Encoded");
        }
    }

    public void SetOnlyOneTransferSender()
    {
        foreach (var resp in _getCohortsResponse.Cohorts)
        {
            resp.TransferSenderId = 1;
            resp.TransferSenderName = "TransferSender1";
        }
    }

    public void Verify_DateCreated_Is_Mapped()
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().DateSentToEmployer.Should().Be(_now.AddMinutes(-7));
            _withTransferSenderViewModel.Cohorts.Last().DateSentToEmployer.Should().Be(_now.AddMinutes(-10));
        }
    }

    public void Verify_Sort_IsApplied(string firstId, string lastId)
    {
        using (new AssertionScope())
        {
            _withTransferSenderViewModel.Cohorts.First().CohortReference.Should().Be(firstId);
            _withTransferSenderViewModel.Cohorts.Last().CohortReference.Should().Be(lastId);
        }
    }

    public WhenMappingTransferSenderRequestToViewModelFixture MakeTheMessagesNull()
    {
        foreach (var cohortSummary in _getCohortsResponse.Cohorts)
        {
            cohortSummary.LatestMessageFromEmployer = cohortSummary.LatestMessageFromProvider = null;
        }

        return this;
    }

    public WhenMappingTransferSenderRequestToViewModelFixture SetCreatedOn()
    {
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 1).CreatedOn = _now.AddMinutes(-5);
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 2).CreatedOn = _now.AddMinutes(-7);
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 3).CreatedOn = _now.AddMinutes(-9);
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 4).CreatedOn = _now.AddMinutes(-10);
        return this;
    }

    public WhenMappingTransferSenderRequestToViewModelFixture SetLatestMessageFromEmployer()
    {
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 1).LatestMessageFromEmployer = new Message("1st Message", _now.AddMinutes(-6));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 2).LatestMessageFromEmployer = new Message("2nd Message", _now.AddMinutes(-7));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 3).LatestMessageFromEmployer = new Message("3rd Message", _now.AddMinutes(-8));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 4).LatestMessageFromEmployer = new Message("4th Message", _now.AddMinutes(-9));

        return this;
    }

    public WhenMappingTransferSenderRequestToViewModelFixture SetLatestMessageFromProvider()
    {
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 1).LatestMessageFromProvider = new Message("1st Message", _now.AddMinutes(-6));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 2).LatestMessageFromProvider = new Message("2nd Message", _now.AddMinutes(-7));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 3).LatestMessageFromProvider = new Message("3rd Message", _now.AddMinutes(-8));
        _getCohortsResponse.Cohorts.First(x => x.CohortId == 4).LatestMessageFromProvider = new Message("4th Message", _now.AddMinutes(-9));

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
                LatestMessageFromEmployer = new Message("this is the last message from Employer", _now.AddMinutes(-10)),
                LatestMessageFromProvider = new Message("This is latestMessage from provider", _now.AddMinutes(-11))
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
                CreatedOn = _now.AddMinutes(-8),
                LatestMessageFromProvider = new Message("This is latestMessage from provider", _now.AddMinutes(-8)),
                LatestMessageFromEmployer = new Message("This is latestMessage from Employer", _now.AddMinutes(-7))
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
                CreatedOn = _now.AddMinutes(-1)
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
                CreatedOn = _now
            },
        };

        return new GetCohortsResponse(cohorts);
    }

    private static long GetCohortId(string cohortReference)
    {
        return long.Parse(cohortReference.Replace("_Encoded", ""));
    }

    private WithTransferSenderCohortSummaryViewModel GetCohortInTransferSenderViewModel(long id)
    {
        return _withTransferSenderViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
    }
}