﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Encoding;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort;

[TestFixture]
public class WhenMappingChooseCohortViewModel
{
    [Test]
    public async Task CohortsInDraftWithProviderAreMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_Cohorts_InDraftWithProvider_Are_Mapped();
    }

    [Test]
    public async Task CohortsInReviewWithProviderAreMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_OnlyCohortsNotRelatedToChangeOfPartyAreMapped();
    }

    [Test]
    public async Task OnlyCohortsNotRelatedToChangeOfPartyAreMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_Cohorts_InDraftWithProvider_Are_Mapped();
    }

    [Test]
    public async Task Then_TheCohortReferenceIsMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_CohortReference_Is_Mapped();
    }

    [Test]
    public async Task Then_EmployerNameIsMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_EmployerName_Is_Mapped();
    }

    [Test]
    public async Task Then_NumberOfApprenticesAreMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_NumberOfApprentices_Are_Mapped();
    }

    [Test]
    public async Task Then_ProviderId_IsMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_ProviderId_IsMapped();
    }

    [Test]
    public async Task Then_UseLearnerData_IsMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_UseLearnerData_IsMapped();
    }

    [Test]
    public async Task Then_AccountLegalEntityPublicHashedId_IsMapped()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_AccountLegalEntityPublicHashedId_IsMapped();
    }

    [Test]
    public async Task Then_By_Default_Cohort_Should_Be_OrderBy_OnDateCreated_Descending_Correctly()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture();
        await fixture.Map();

        fixture.Verify_Ordered_By_DateCreatedDescending();
    }

    [Test]
    public async Task Then_Cohort_Ordered_By_EmployerNameDescending_Correctly()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture
        {
            ChooseCohortByProviderRequest =
            {
                SortField = nameof(ChooseCohortSummaryViewModel.EmployerName),
                ReverseSort = true
            }
        };

        await fixture.Map();

        fixture.Verify_Ordered_By_EmployerNameDescending();
    }

    [Test]
    public async Task Then_Cohort_Ordered_By_CohortReferenceDescending_Correctly()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture
        {
            ChooseCohortByProviderRequest =
            {
                SortField = nameof(ChooseCohortSummaryViewModel.CohortReference),
                ReverseSort = true
            }
        };

        await fixture.Map();

        fixture.Verify_Ordered_By_CohortReferenceDescending();
    }

    [Test]
    public async Task Then_Cohort_Ordered_By_StatusDescending_Correctly()
    {
        var fixture = new WhenMappingChooseCohortViewModelFixture
        {
            ChooseCohortByProviderRequest =
            {
                SortField = nameof(ChooseCohortSummaryViewModel.Status),
                ReverseSort = true
            }
        };

        await fixture.Map();

        fixture.Verify_Ordered_By_StatusDescending();
    }
}

public class WhenMappingChooseCohortViewModelFixture
{
    private readonly Mock<IEncodingService> _encodingService;
    private readonly ChooseCohortViewModelMapper _mapper;
    private ChooseCohortViewModel _chooseCohortViewModel;
    private const long ProviderId = 1;

    public ChooseCohortByProviderRequest ChooseCohortByProviderRequest { get; }

    public WhenMappingChooseCohortViewModelFixture()
    {
        _encodingService = new Mock<IEncodingService>();
        var commitmentsApiClient = new Mock<ICommitmentsApiClient>();

        ChooseCohortByProviderRequest = new ChooseCohortByProviderRequest() { ProviderId = ProviderId };
        var getCohortsResponse = CreateGetCohortsResponse();

        commitmentsApiClient.Setup(c => c.GetCohorts(It.Is<GetCohortsRequest>(r => r.ProviderId == ProviderId), CancellationToken.None)).ReturnsAsync(getCohortsResponse);
        _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference)).Returns((long y, EncodingType z) => y + "_Encoded");

        _mapper = new ChooseCohortViewModelMapper(commitmentsApiClient.Object, _encodingService.Object);
    }

    public async Task<WhenMappingChooseCohortViewModelFixture> Map()
    {
        _chooseCohortViewModel = await _mapper.Map(ChooseCohortByProviderRequest);
        return this;
    }

    public void Verify_Cohorts_InDraftWithProvider_Are_Mapped()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.Count().Should().Be(4);
            GetCohortInReviewViewModel(5).Should().NotBeNull();
            GetCohortInReviewViewModel(6).Should().NotBeNull();
        }
    }

    public void Verify_OnlyCohortsNotRelatedToChangeOfPartyAreMapped()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.Count().Should().Be(4);
            GetCohortInReviewViewModel(7).Should().BeNull();
        }
    }

    public void Verify_CohortReference_Is_Mapped()
    {
        _encodingService.Verify(x => x.Encode(It.IsAny<long>(), EncodingType.CohortReference), Times.Exactly(4));

        using (new AssertionScope())
        {
            GetCohortInReviewViewModel(1).CohortReference.Should().Be("1_Encoded");
            GetCohortInReviewViewModel(2).CohortReference.Should().Be("2_Encoded");
            GetCohortInReviewViewModel(5).CohortReference.Should().Be("5_Encoded");
            GetCohortInReviewViewModel(6).CohortReference.Should().Be("6_Encoded");
        }
    }

    public void Verify_EmployerName_Is_Mapped()
    {
        using (new AssertionScope())
        {
            GetCohortInReviewViewModel(1).EmployerName.Should().Be("Employer1");
            GetCohortInReviewViewModel(2).EmployerName.Should().Be("Employer2");
            GetCohortInReviewViewModel(5).EmployerName.Should().Be("Employer5");
            GetCohortInReviewViewModel(6).EmployerName.Should().Be("Employer6");
        }
    }

    public void Verify_NumberOfApprentices_Are_Mapped()
    {
        using (new AssertionScope())
        {
            GetCohortInReviewViewModel(5).NumberOfApprentices.Should().Be(500);
            GetCohortInReviewViewModel(6).NumberOfApprentices.Should().Be(600);
        }
    }

    public void Verify_AccountLegalEntityPublicHashedId_IsMapped()
    {
        using (new AssertionScope())
        {
            GetCohortInReviewViewModel(1).AccountLegalEntityPublicHashedId.Should().Be("100A");
            GetCohortInReviewViewModel(2).AccountLegalEntityPublicHashedId.Should().Be("200B");
        }
    }

    public void Verify_ProviderId_IsMapped()
    {
        _chooseCohortViewModel.ProviderId.Should().Be(ProviderId);
    }

    public void Verify_UseLearnerData_IsMapped()
    {
        _chooseCohortViewModel.UseLearnerData.Should().Be(_chooseCohortViewModel.UseLearnerData);
    }

    public void Verify_Ordered_By_DateCreatedDescending()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.First().EmployerName.Should().Be("Employer6");
            _chooseCohortViewModel.Cohorts.Last().EmployerName.Should().Be("Employer1");
        }
    }

    public void Verify_Ordered_By_EmployerNameDescending()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.First().EmployerName.Should().Be("Employer6");
            _chooseCohortViewModel.Cohorts.Last().EmployerName.Should().Be("Employer1");
        }
    }

    public void Verify_Ordered_By_CohortReferenceDescending()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.First().CohortReference.Should().Be("6_Encoded");
            _chooseCohortViewModel.Cohorts.Last().CohortReference.Should().Be("1_Encoded");
        }
    }

    public void Verify_Ordered_By_StatusDescending()
    {
        using (new AssertionScope())
        {
            _chooseCohortViewModel.Cohorts.First().Status.Should().Be("Ready to review");
            _chooseCohortViewModel.Cohorts.Last().Status.Should().Be("Draft");
        }
    }

    private static GetCohortsResponse CreateGetCohortsResponse()
    {
        IEnumerable<CohortSummary> cohorts = new List<CohortSummary>()
        {
            new()
            {
                CohortId = 1,
                AccountId = 1,
                ProviderId = 1,
                LegalEntityName = "Employer1",
                AccountLegalEntityPublicHashedId = "100A",
                NumberOfDraftApprentices = 100,
                IsDraft = false,
                WithParty = Party.Provider,
                CreatedOn = DateTime.Now.AddHours(-6),
            },
            new()
            {
                CohortId = 2,
                AccountId = 2,
                ProviderId = 1,
                LegalEntityName = "Employer2",
                AccountLegalEntityPublicHashedId = "200B",
                NumberOfDraftApprentices = 200,
                IsDraft = false,
                WithParty = Party.Provider,
                CreatedOn = DateTime.Now.AddHours(-5)
            },
            new()
            {
                CohortId = 3,
                AccountId = 3,
                ProviderId = 1,
                LegalEntityName = "Employer3",
                AccountLegalEntityPublicHashedId = "300C",
                NumberOfDraftApprentices = 300,
                IsDraft = true,
                WithParty = Party.Employer,
                CreatedOn = DateTime.Now.AddHours(-4)
            },
            new()
            {
                CohortId = 4,
                AccountId = 4,
                ProviderId = 1,
                LegalEntityName = "Employer4",
                AccountLegalEntityPublicHashedId = "400D",
                NumberOfDraftApprentices = 400,
                IsDraft = false,
                WithParty = Party.Employer,
                CreatedOn = DateTime.Now.AddHours(-3)
            },
            new()
            {
                CohortId = 5,
                AccountId = 5,
                ProviderId = 1,
                LegalEntityName = "Employer5",
                AccountLegalEntityPublicHashedId = "500E",
                NumberOfDraftApprentices = 500,
                IsDraft = true,
                WithParty = Party.Provider,
                CreatedOn = DateTime.Now.AddHours(-2)
            },
            new()
            {
                CohortId = 6,
                AccountId = 6,
                ProviderId = 1,
                LegalEntityName = "Employer6",
                AccountLegalEntityPublicHashedId = "600F",
                NumberOfDraftApprentices = 600,
                IsDraft = true,
                WithParty = Party.Provider,
                CreatedOn = DateTime.Now.AddHours(-1)
            },
            new()
            {
                CohortId = 7,
                AccountId = 7,
                ProviderId = 1,
                LegalEntityName = "Employer7",
                AccountLegalEntityPublicHashedId = "700F",
                NumberOfDraftApprentices = 700,
                IsDraft = true,
                WithParty = Party.Provider,
                CreatedOn = DateTime.Now.AddHours(-1),
                IsLinkedToChangeOfPartyRequest = true
            }
        };

        return new GetCohortsResponse(cohorts);
    }

    private static long GetCohortId(string cohortReference)
    {
        return long.Parse(cohortReference.Replace("_Encoded", ""));
    }

    private ChooseCohortSummaryViewModel GetCohortInReviewViewModel(long id)
    {
        return _chooseCohortViewModel.Cohorts.FirstOrDefault(x => GetCohortId(x.CohortReference) == id);
    }
}