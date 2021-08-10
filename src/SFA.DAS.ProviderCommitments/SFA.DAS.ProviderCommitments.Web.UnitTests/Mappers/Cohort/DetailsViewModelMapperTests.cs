﻿using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.Encoding;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderCommitments.Web.Mappers.Cohort;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Cohort
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DetailsViewModelMapperTests
    {
        [Test]
        public async Task ProviderIdIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Source.ProviderId, result.ProviderId);
        }

        [Test]
        public async Task WithPartyIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.WithParty, result.WithParty);
        }

        [Test]
        public async Task LegalEntityNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.LegalEntityName, result.LegalEntityName);
        }

        [Test]
        public async Task ProviderNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.ProviderName, result.ProviderName);
        }

        [Test]
        public async Task MessageIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.LatestMessageCreatedByProvider, result.Message);
        }

        [TestCase(true, true, "No, send to employer to review or add details")]
        [TestCase(true, false, "Yes, send to employer to review or add details")]
        [TestCase(false, true, "Yes, send to employer to review or add details")]
        [TestCase(false, false, "Yes, send to employer to review or add details")]
        public async Task SendBackToEmployerOptionMessageIsMappedCorrectly(bool isAgreementSigned, bool providerComplete, string expected)
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetIsAgreementSigned(isAgreementSigned).SetProviderComplete(providerComplete);
            var result = await fixture.Map();
            Assert.AreEqual(expected, result.SendBackToEmployerOptionMessage);
        }

        [Test]
        public async Task CohortReferenceIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Source.CohortReference, result.CohortReference);
        }

        [Test]
        public async Task IsApprovedByEmployerIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.IsApprovedByEmployer, result.IsApprovedByEmployer);
        }

        [Test]
        public async Task TransferSenderHashedIdIsEncodedCorrectlyWhenThereIsAValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetTransferSenderIdAndItsExpectedHashedValue(123, "X123X");
            var result = await fixture.Map();
            Assert.AreEqual("X123X", result.TransferSenderHashedId);
        }

        [Test]
        public async Task TransferSenderHashedIdIsNullWhenThereIsNoValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetTransferSenderIdAndItsExpectedHashedValue(null, null);
            var result = await fixture.Map();
            Assert.IsNull(result.TransferSenderHashedId);
        }

        [Test]
        public async Task DraftApprenticeshipTotalCountIsReportedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.DraftApprenticeshipsResponse.DraftApprenticeships.Count, result.DraftApprenticeshipsCount);
        }

        [Test]
        public async Task DraftApprenticeshipCourseCountIsReportedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var course in result.Courses)
            {
                var expectedCount = fixture.DraftApprenticeshipsResponse.DraftApprenticeships
                    .Count(a => a.CourseCode == course.CourseCode && a.CourseName == course.CourseName);

                Assert.AreEqual(expectedCount, course.Count);
            }
        }

        [Test]
        public async Task DraftApprenticeshipCourseOrderIsByCourseName()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var expectedSequence = fixture.DraftApprenticeshipsResponse.DraftApprenticeships
                .Select(c => new { c.CourseName, c.CourseCode })
                .Distinct()
                .OrderBy(c => c.CourseName).ThenBy(c => c.CourseCode)
                .ToList();

            var actualSequence = result.Courses
                .Select(c => new { c.CourseName, c.CourseCode })
                .OrderBy(c => c.CourseName).ThenBy(c => c.CourseCode)
                .ToList();

            fixture.AssertSequenceOrder(expectedSequence, actualSequence, (e, a) => e.CourseName == a.CourseName && e.CourseCode == a.CourseCode);
        }

        [Test]
        public async Task DraftApprenticesOrderIsByApprenticeName()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var course in result.Courses)
            {
                var expectedSequence = fixture.DraftApprenticeshipsResponse.DraftApprenticeships
                    .Where(a => a.CourseName == course.CourseName && a.CourseCode == course.CourseCode)
                    .Select(a => $"{a.FirstName} {a.LastName}")
                    .OrderBy(a => a)
                    .ToList();

                var actualSequence = course.DraftApprenticeships.Select(a => a.DisplayName).ToList();

                fixture.AssertSequenceOrder(expectedSequence, actualSequence, (e, a) => e == a);
            }
        }

        [Test]
        public async Task DraftApprenticeshipsAreMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.DraftApprenticeshipsResponse.DraftApprenticeships)
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                fixture.AssertEquality(draftApprenticeship, draftApprenticeshipResult);
            }
        }

        [Test]
        public async Task FundingBandCapsAreMappedCorrectlyForCoursesStartingOnDefaultdate()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.DraftApprenticeshipsResponse.DraftApprenticeships.Where(x => x.StartDate == fixture.DefaultStartDate))
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                Assert.AreEqual(1000, draftApprenticeshipResult.FundingBandCap);
            }
        }

        [Test]
        public async Task Then_Funding_Cap_Is_Null_When_No_Course_Found()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetNoCourse();

            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.DraftApprenticeshipsResponse.DraftApprenticeships)
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                Assert.AreEqual(null, draftApprenticeshipResult.FundingBandCap);
            }
        }

        [Test]
        public async Task Then_Funding_Cap_Is_Null_When_No_Course_Set()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetNoCourseSet();

            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.DraftApprenticeshipsResponse.DraftApprenticeships)
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                Assert.AreEqual(null, draftApprenticeshipResult.FundingBandCap);
            }
            fixture.CommitmentsApiClient.Verify(x => x.GetTrainingProgramme(It.IsAny<string>(), CancellationToken.None), Times.Never);
        }

        [Test]
        public async Task FundingBandCapsAreNullForCoursesStarting2MonthsAhead()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.DraftApprenticeshipsResponse.DraftApprenticeships.Where(x => x.StartDate == fixture.DefaultStartDate.AddMonths(2)))
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                Assert.AreEqual(null, draftApprenticeshipResult.FundingBandCap);
            }
        }

        [Test]
        public async Task FundingBandExcessModelShowsTwoApprenticeshipsExceedingTheBandForCourse1()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C1").FundingBandExcess;
            Assert.AreEqual(2, excessModel.NumberOfApprenticesExceedingFundingBandCap);
        }

        [Test]
        public async Task FundingBandExcessModelShowsOnlyTheFullStopWhenMultipleFundingCapsAreExceeded()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C1").FundingBandExcess;
            Assert.AreEqual(".", excessModel.DisplaySingleFundingBandCap);
        }

        [Test]
        public async Task FundingBandExcessModelShowsOneApprenticeshipExceedingTheBandForCourse2()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C2").FundingBandExcess;
            Assert.AreEqual(1, excessModel.NumberOfApprenticesExceedingFundingBandCap);
        }

        [Test]
        public async Task FundingBandExcessModelShowsTheSingleFundingBandCapExceeded()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C2").FundingBandExcess;
            Assert.AreEqual(" of £1,000.", excessModel.DisplaySingleFundingBandCap);
        }

        [Test]
        public async Task FundingBandExcessModelIsNullForCourse3()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C3").FundingBandExcess;
            Assert.IsNull(excessModel);
        }

        [TestCase(0, "Approve apprentice details", Party.Provider)]
        [TestCase(1, "Approve apprentice details", Party.Provider)]
        [TestCase(2, "Approve 2 apprentices' details", Party.Provider)]
        [TestCase(0, "View apprentice details", Party.Employer)]
        [TestCase(1, "View apprentice details", Party.Employer)]
        [TestCase(2, "View 2 apprentices' details", Party.Employer)]
        [TestCase(0, "View apprentice details", Party.TransferSender)]
        [TestCase(1, "View apprentice details", Party.TransferSender)]
        [TestCase(2, "View 2 apprentices' details", Party.TransferSender)]
        public async Task PageTitleIsSetCorrectlyForTheNumberOfApprenticeships(int numberOfApprenticeships, string expectedPageTitle, Party withParty)
        {
            var fixture = new DetailsViewModelMapperTestsFixture().CreateThisNumberOfApprenticeships(numberOfApprenticeships);
            fixture.Cohort.WithParty = withParty;

            var result = await fixture.Map();

            Assert.AreEqual(expectedPageTitle, result.PageTitle);
        }

        [TestCase("C2", "1 apprenticeship above funding band maximum")]
        [TestCase("C1", "2 apprenticeships above funding band maximum")]
        public async Task FundingBandCapExcessHeaderIsSetCorrectlyForTheNumberOfApprenticeshipsOverFundingCap(string courseCode, string expectedFundingBandCapExcessHeader)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            Assert.AreEqual(expectedFundingBandCapExcessHeader, result.Courses.FirstOrDefault(x => x.CourseCode == courseCode).FundingBandExcess.FundingBandCapExcessHeader);
        }

        [TestCase("C2", "The price for this apprenticeship is above its")]
        [TestCase("C1", "The price for these apprenticeships is above the")]
        public async Task FundingBandCapExcessLabelIsSetCorrectlyForTheNumberOfApprenticeshipsOverFundingCap(string courseCode, string expectedFundingBandCapExcessLabel)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            Assert.AreEqual(expectedFundingBandCapExcessLabel, result.Courses.FirstOrDefault(x => x.CourseCode == courseCode).FundingBandExcess.FundingBandCapExcessLabel);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task IsAgreementSignedIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedIsAgreementSigned)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedIsAgreementSigned, result.IsAgreementSigned);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task IsAgreementSignedIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool expectedIsAgreementSigned)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedIsAgreementSigned, result.IsAgreementSigned);
        }

        [TestCase(true, "Approve these details?")]
        [TestCase(false, "Submit to employer?")]
        public async Task OptionsTitleIsMappedCorrectlyWithATransfer(bool isAgreementSigned, string expectedOptionsTitle)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedOptionsTitle, result.OptionsTitle);
        }

        [TestCase(true, "Approve these details?")]
        [TestCase(false, "Submit to employer?")]
        public async Task OptionsTitleIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, string expectedOptionsTitle)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedOptionsTitle, result.OptionsTitle);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowViewAgreementOptionIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedShowViewAgreementOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowViewAgreementOption, result.ShowViewAgreementOption);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOption, result.ProviderCanApprove);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowAddAnotherApprenticeOptionIsMappedCorrectly(bool isChangeOfParty, bool expectedShowAddAnotherOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsChangeOfParty(isChangeOfParty);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowAddAnotherOption, result.ShowAddAnotherApprenticeOption);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOption, result.ProviderCanApprove);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWhenOverlap(bool hasOverlap, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetUlnOverlap(hasOverlap);
            var result = await fixture.Map();

            Assert.AreEqual(expectedShowApprovalOption, result.ProviderCanApprove);
        }

        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, false, false, false)]
        public async Task ShowApprovalOptionMessageIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool showApprovalOption,
            bool isApprovedByEmployer, bool expectedShowApprovalOptionMessage)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.Cohort.IsApprovedByEmployer = isApprovedByEmployer;
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOptionMessage, result.ShowApprovalOptionMessage);
        }

        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, false, false, false)]
        public async Task ShowApprovalOptionMessageIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool showApprovalOption,
            bool isApprovedByEmployer, bool expectedShowApprovalOptionMessage)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.Cohort.IsApprovedByEmployer = isApprovedByEmployer;
            fixture.SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOptionMessage, result.ShowApprovalOptionMessage);
        }

        [Test]
        public async Task IsCompleteForProviderIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.Cohort.IsCompleteForProvider, result.IsCompleteForProvider);
        }

        [Test]
        public async Task FundingCapIsMappedCorrectlyForChangeOfPartyApprentice()
        {
            var result = await new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetupChangeOfPartyScenario()
                .Map();

            Assert.AreEqual(1000, result.Courses.First().DraftApprenticeships.First().FundingBandCap);
            Assert.AreEqual(true, result.Courses.First().DraftApprenticeships.First().ExceedsFundingBandCap);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_PendingApproval_From_TransferSender()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetTransferSender()
                .SetCohortWithParty(Party.TransferSender);

            fixture.Cohort.IsApprovedByEmployer = fixture.Cohort.IsApprovedByProvider = true;
            fixture.Cohort.TransferApprovalStatus = TransferApprovalStatus.Pending;

            var result = await fixture.Map();
            Assert.AreEqual("Pending - with funding employer", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithProvider_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider); 

            fixture.Cohort.LastAction = LastAction.None;
            
            var result = await fixture.Map();
            Assert.AreEqual("New request", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Provider_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.Cohort.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.AreEqual("Ready for review", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Provider_With_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.Cohort.LastAction = LastAction.Approve;
            fixture.Cohort.IsApprovedByEmployer = true;

            var result = await fixture.Map();
            Assert.AreEqual("Ready for approval", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithEmployer_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.Cohort.LastAction = LastAction.None;

            var result = await fixture.Map();
            Assert.AreEqual("New request", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.Cohort.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.AreEqual("Under review with employer", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_With_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.Cohort.LastAction = LastAction.Approve;

            var result = await fixture.Map();
            Assert.AreEqual("With Employer for approval", result.Status);
        }
    }

    public class DetailsViewModelMapperTestsFixture
    {
        public DetailsViewModelMapper Mapper;
        public DetailsRequest Source;
        public DetailsViewModel Result;
        public Mock<ICommitmentsApiClient> CommitmentsApiClient;
        public Mock<IPasAccountApiClient> PasAccountApiClient;
        public Mock<IEncodingService> EncodingService;
        public GetCohortResponse Cohort;
        public GetDraftApprenticeshipsResponse DraftApprenticeshipsResponse;
        public DateTime DefaultStartDate = new DateTime(2019, 10, 1);
        public AccountLegalEntityResponse AccountLegalEntityResponse;
        public ProviderAgreement ProviderAgreement;

        private Fixture _autoFixture;
        private TrainingProgramme _trainingProgramme;
        private List<TrainingProgrammeFundingPeriod> _fundingPeriods;
        private DateTime _startFundingPeriod = new DateTime(2019, 10, 1);
        private DateTime _endFundingPeriod = new DateTime(2019, 10, 30);

        public DetailsViewModelMapperTestsFixture()
        {
            _autoFixture = new Fixture();

            Cohort = _autoFixture.Build<GetCohortResponse>().Without(x => x.TransferSenderId).With(x => x.IsCompleteForProvider, true).Without(x => x.ChangeOfPartyRequestId).Create();
            AccountLegalEntityResponse = _autoFixture.Create<AccountLegalEntityResponse>();
            ProviderAgreement = new ProviderAgreement { Status = ProviderAgreementStatus.Agreed };

            var draftApprenticeships = CreateDraftApprenticeshipDtos(_autoFixture);
            _autoFixture.Register(() => draftApprenticeships);
            DraftApprenticeshipsResponse = _autoFixture.Create<GetDraftApprenticeshipsResponse>();

            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Cohort);
            CommitmentsApiClient.Setup(x => x.GetDraftApprenticeships(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(DraftApprenticeshipsResponse);
            CommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountLegalEntityResponse);

            PasAccountApiClient = new Mock<IPasAccountApiClient>();
            PasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), CancellationToken.None)).ReturnsAsync(ProviderAgreement);

            _fundingPeriods = new List<TrainingProgrammeFundingPeriod>
            {
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, FundingCap = 1000},
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), FundingCap = 500}
            };
            _trainingProgramme = new TrainingProgramme { EffectiveFrom = DefaultStartDate, EffectiveTo = DefaultStartDate.AddYears(1), FundingPeriods = _fundingPeriods };

            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme(It.Is<string>(c => !string.IsNullOrEmpty(c)), CancellationToken.None))
                .ReturnsAsync(new GetTrainingProgrammeResponse { TrainingProgramme = _trainingProgramme });

            CommitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), CancellationToken.None))
               .ReturnsAsync(new ValidateUlnOverlapResult { HasOverlappingEndDate = false, HasOverlappingStartDate = false});
            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme("no-course", CancellationToken.None))
                .ThrowsAsync(new RestHttpClientException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    RequestMessage = new HttpRequestMessage(),
                    ReasonPhrase = "Url not found"
                }, "Course not found"));

            EncodingService = new Mock<IEncodingService>();
            SetEncodingOfApprenticeIds();

            Mapper = new DetailsViewModelMapper(CommitmentsApiClient.Object, EncodingService.Object, PasAccountApiClient.Object);
            Source = _autoFixture.Create<DetailsRequest>();
        }

        public DetailsViewModelMapperTestsFixture SetCohortWithParty(Party party)
        {
            Cohort.WithParty = party;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetTransferSenderIdAndItsExpectedHashedValue(long? transferSenderId, string expectedHashedId)
        {
            Cohort.TransferSenderId = transferSenderId;
            if (transferSenderId.HasValue)
            {
                EncodingService.Setup(x => x.Encode(transferSenderId.Value, EncodingType.PublicAccountId))
                    .Returns(expectedHashedId);
            }

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetEncodingOfApprenticeIds()
        {
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId))
                .Returns((long p, EncodingType t) => $"X{p}X");

            return this;
        }

        public DetailsViewModelMapperTestsFixture CreateThisNumberOfApprenticeships(int numberOfApprenticeships)
        {
            var draftApprenticeships = _autoFixture.CreateMany<DraftApprenticeshipDto>(numberOfApprenticeships).ToArray();
            DraftApprenticeshipsResponse.DraftApprenticeships = draftApprenticeships;
            return this;
        }

        public Task<DetailsViewModel> Map()
        {
            return Mapper.Map(TestHelper.Clone(Source));
        }

        public void AssertEquality(DraftApprenticeshipDto source, CohortDraftApprenticeshipViewModel result)
        {
            Assert.AreEqual(source.Id, result.Id);
            Assert.AreEqual(source.FirstName, result.FirstName);
            Assert.AreEqual(source.LastName, result.LastName);
            Assert.AreEqual(source.DateOfBirth, result.DateOfBirth);
            Assert.AreEqual(source.Cost, result.Cost);
            Assert.AreEqual(source.StartDate, result.StartDate);
            Assert.AreEqual(source.EndDate, result.EndDate);
            Assert.AreEqual($"X{source.Id}X", result.DraftApprenticeshipHashedId);
        }

        public void AssertSequenceOrder<T>(List<T> expected, List<T> actual, Func<T, T, bool> evaluator)
        {
            Assert.AreEqual(expected.Count, actual.Count, "Expected and actual sequences are different lengths");

            for (int i = 0; i < actual.Count; i++)
            {
                Assert.IsTrue(evaluator(expected[i], actual[i]), "Actual sequence are not in same order as expected");
            }
        }

        private IReadOnlyCollection<DraftApprenticeshipDto> CreateDraftApprenticeshipDtos(Fixture autoFixture)
        {
            var draftApprenticeships = autoFixture.CreateMany<DraftApprenticeshipDto>(6).ToArray();
            SetCourseDetails(draftApprenticeships[0], "Course1", "C1", 1000);
            SetCourseDetails(draftApprenticeships[1], "Course1", "C1", 2100);
            SetCourseDetails(draftApprenticeships[2], "Course1", "C1", 2000, DefaultStartDate.AddMonths(1));

            SetCourseDetails(draftApprenticeships[3], "Course2", "C2", 1500);
            SetCourseDetails(draftApprenticeships[4], "Course2", "C2", null);

            SetCourseDetails(draftApprenticeships[5], "Course3", "C3", null, DefaultStartDate.AddMonths(2));

            return draftApprenticeships;
        }

        private void SetCourseDetails(DraftApprenticeshipDto draftApprenticeship, string courseName, string courseCode, int? cost, DateTime? startDate = null, DateTime? originalStartDate = null)
        {
            startDate = startDate ?? DefaultStartDate;

            draftApprenticeship.CourseName = courseName;
            draftApprenticeship.CourseCode = courseCode;
            draftApprenticeship.Cost = cost;
            draftApprenticeship.StartDate = startDate;
            draftApprenticeship.OriginalStartDate = originalStartDate;
        }

        public DetailsViewModelMapperTestsFixture SetProviderComplete(bool providerComplete)
        {
            Cohort.IsCompleteForProvider = providerComplete;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetTransferSender()
        {
            Cohort.TransferSenderId = _autoFixture.Create<long>();
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetupChangeOfPartyScenario()
        {
            Cohort.ChangeOfPartyRequestId = 1;
            var draftApprenticeship = DraftApprenticeshipsResponse.DraftApprenticeships.First();
            draftApprenticeship.OriginalStartDate = _fundingPeriods.First().EffectiveFrom;
            draftApprenticeship.StartDate = _fundingPeriods.Last().EffectiveFrom;
            draftApprenticeship.Cost = _fundingPeriods.First().FundingCap + 500;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetIsChangeOfParty(bool isChangeOfParty)
        {
            Cohort.ChangeOfPartyRequestId = isChangeOfParty ? _autoFixture.Create<long>() : default(long?);

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetNoCourse()
        {
            DraftApprenticeshipsResponse.DraftApprenticeships = DraftApprenticeshipsResponse.DraftApprenticeships.Select(c =>
            {
                c.CourseCode = "no-course";
                return c;
            }).ToList();
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetNoCourseSet()
        {
            DraftApprenticeshipsResponse.DraftApprenticeships = DraftApprenticeshipsResponse.DraftApprenticeships.Select(c =>
            {
                c.CourseCode = "";
                return c;
            }).ToList();
            return this;
        }

        internal DetailsViewModelMapperTestsFixture SetIsAgreementSigned(bool isAgreementSigned)
        {
            var agreementStatus = isAgreementSigned ? ProviderAgreementStatus.Agreed : ProviderAgreementStatus.NotAgreed;
            PasAccountApiClient
               .Setup(x => x.GetAgreement(It.IsAny<long>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ProviderAgreement { Status = agreementStatus });
            return this;
        }

        internal DetailsViewModelMapperTestsFixture SetUlnOverlap(bool hasOverlap)
        {
            CommitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), CancellationToken.None))
             .ReturnsAsync(new ValidateUlnOverlapResult { HasOverlappingEndDate = hasOverlap, HasOverlappingStartDate = hasOverlap });

            return this;
        }
    }
}
