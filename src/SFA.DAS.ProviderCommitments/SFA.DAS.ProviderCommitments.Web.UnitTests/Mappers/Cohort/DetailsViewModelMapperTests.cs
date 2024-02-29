using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts;
using SFA.DAS.ProviderCommitments.Web.Services;
using Party = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.Party;
using TransferApprovalStatus = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.TransferApprovalStatus;
using LastAction = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Responses.LastAction;
using DraftApprenticeshipDto = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.DraftApprenticeshipDto;
using DeliveryModel = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Types.DeliveryModel;
using ApprenticeshipEmailOverlap = SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Cohorts.ApprenticeshipEmailOverlap;

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
            Assert.AreEqual(fixture.CohortDetails.WithParty, result.WithParty);
        }

        [Test]
        public async Task LegalEntityNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.CohortDetails.LegalEntityName, result.LegalEntityName);
        }

        [Test]
        public async Task ProviderNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.CohortDetails.ProviderName, result.ProviderName);
        }

        [Test]
        public async Task MessageIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.CohortDetails.LatestMessageCreatedByEmployer, result.Message);
        }

        [TestCase(true, true, "No, send to employer to review or add details")]
        [TestCase(true, false, "Yes, send to employer to review or add details")]
        [TestCase(false, true, "Yes, send to employer to review or add details")]
        [TestCase(false, false, "Yes, send to employer to review or add details")]
        public async Task SendBackToEmployerOptionMessageIsMappedCorrectly(bool isAgreementSigned, bool providerComplete, string expected)
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetIsAgreementSigned(isAgreementSigned).SetProviderComplete(providerComplete).SetRplErrorsToNone();
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
            Assert.AreEqual(fixture.CohortDetails.IsApprovedByEmployer, result.IsApprovedByEmployer);
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
        public async Task PledgeApplicationIdIsEncodedCorrectlyWhenThereIsAValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetPledgeApplicationIdAndItsExpectedHashedValue(567, "Z567Z");
            var result = await fixture.Map();
            Assert.AreEqual("Z567Z", result.EncodedPledgeApplicationId);
        }

        [Test]
        public async Task PledgeApplicationIdIsNullWhenThereIsNoValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetPledgeApplicationIdAndItsExpectedHashedValue(null, null);
            var result = await fixture.Map();
            Assert.IsNull(result.EncodedPledgeApplicationId);
        }

        [Test]
        public async Task DraftApprenticeshipTotalCountIsReportedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.CohortDetails.DraftApprenticeships.Count, result.DraftApprenticeshipsCount);
        }

        [Test]
        public async Task DraftApprenticeshipCourseCountIsReportedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var course in result.Courses)
            {
                var expectedCount = fixture.CohortDetails.DraftApprenticeships
                    .Count(a => a.CourseCode == course.CourseCode && a.CourseName == course.CourseName && a.DeliveryModel == course.DeliveryModel);

                Assert.AreEqual(expectedCount, course.Count);
            }
        }

        [Test]
        public async Task DraftApprenticeshipCourseOrderIsByCourseName()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var expectedSequence = fixture.CohortDetails.DraftApprenticeships
                .Select(c => new { c.CourseName, c.CourseCode, DeliveryModel = c.DeliveryModel })
                .Distinct()
                .OrderBy(c => c.CourseName).ThenBy(c => c.CourseCode).ThenBy(c => c.DeliveryModel)
                .ToList();

            var actualSequence = result.Courses
                .Select(c => new { c.CourseName, c.CourseCode, c.DeliveryModel })
                .OrderBy(c => c.CourseName).ThenBy(c => c.CourseCode).ThenBy(c => c.DeliveryModel)
                .ToList();

            fixture.AssertSequenceOrder(expectedSequence, actualSequence, (e, a) => e.CourseName == a.CourseName && e.CourseCode == a.CourseCode && e.DeliveryModel == a.DeliveryModel);
        }

        [Test]
        public async Task DraftApprenticesOrderIsByApprenticeName()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var course in result.Courses)
            {
                var expectedSequence = fixture.CohortDetails.DraftApprenticeships
                    .Where(a => a.CourseName == course.CourseName && a.CourseCode == course.CourseCode && a.DeliveryModel == course.DeliveryModel)
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

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships)
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

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships.Where(x => x.StartDate == fixture.DefaultStartDate))
            {
                var draftApprenticeshipResult =
                    result.Courses.SelectMany(c => c.DraftApprenticeships).Single(x => x.Id == draftApprenticeship.Id);

                Assert.AreEqual(1000, draftApprenticeshipResult.FundingBandCap);
            }
        }

        [Test]
        public async Task FundingBandCapsAreMappedCorrectlyForPilotApprenticeships()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships.Where(x => x.IsOnFlexiPaymentPilot.GetValueOrDefault()))
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

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships)
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

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships)
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

            foreach (var draftApprenticeship in fixture.CohortDetails.DraftApprenticeships.Where(x => x.StartDate == fixture.DefaultStartDate.AddMonths(2)))
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
            fixture.CohortDetails.WithParty = withParty;

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
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.AreEqual(expectedOptionsTitle, result.OptionsTitle);
        }

        [TestCase(true, "Approve these details?")]
        [TestCase(false, "Submit to employer?")]
        public async Task OptionsTitleIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, string expectedOptionsTitle)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
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
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
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
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOption, result.ProviderCanApprove);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithAnInvalidCourse(bool hasInvalidCourse, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetHasInvalidCourse(hasInvalidCourse).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOption, result.ProviderCanApprove);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWhenOverlap(bool hasOverlap, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetUlnOverlap(hasOverlap).SetRplErrorsToNone();
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
            var fixture = new DetailsViewModelMapperTestsFixture().SetRplErrorsToNone();
            fixture.CohortDetails.IsApprovedByEmployer = isApprovedByEmployer;
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
            fixture.CohortDetails.IsApprovedByEmployer = isApprovedByEmployer;
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.AreEqual(expectedShowApprovalOptionMessage, result.ShowApprovalOptionMessage);
        }

        [Test]
        public async Task ShowApprovalOfCohortAsTrueWhenNoRplErrors()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetRplErrorsToNone();
            fixture.CohortDetails.IsApprovedByEmployer = true;
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.IsTrue(result.ShowApprovalOptionMessage);
        }

        [Test]
        [Ignore("Ignore this test until RPL is no longer just a warning")]
        public async Task ShowApprovalOfCohortAsFalseWhenRplErrorsExist()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.CohortDetails.IsApprovedByEmployer = true;
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.IsFalse(result.ShowApprovalOptionMessage);
        }

        [Test]
        public async Task ShowApprovalOfCohortAsTrueWhenRplErrorsExist()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.CohortDetails.IsApprovedByEmployer = true;
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.IsTrue(result.ShowApprovalOptionMessage);
        }

        [Test]
        public async Task IsCompleteForProviderIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(fixture.CohortDetails.IsCompleteForProvider, result.IsCompleteForProvider);
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
        public async Task EmailOverlapIsMappedCorrectlyToDraftApprenticeshipAndToSummaryLine()
        {
            var f = new DetailsViewModelMapperTestsFixture().WithOneEmailOverlapping();
            var apprenticeshipId = f.CohortDetails.ApprenticeshipEmailOverlaps.First().Id;

            var result = await f.Map();
            var course = result.Courses.FirstOrDefault(x => x.DraftApprenticeships.Any(y => y.Id == apprenticeshipId));

            Assert.NotNull(course);
            Assert.NotNull(course.EmailOverlaps);
            Assert.AreEqual(1, course.EmailOverlaps.NumberOfEmailOverlaps);
            Assert.AreEqual(1, course.DraftApprenticeships.Count(x => x.HasOverlappingEmail));
            Assert.AreEqual(apprenticeshipId, course.DraftApprenticeships.First(x => x.HasOverlappingEmail).Id);
        }

        [Test]
        public async Task EmailOverlapIsMappedCorrectlyToDraftApprenticeshipsAndToSummaryLineWhenTwoEmailOverlapsExistOnSameCourse()
        {
            var f = new DetailsViewModelMapperTestsFixture().WithTwoEmailOverlappingOnSameCourse();
            var apprenticeshipId1 = f.CohortDetails.ApprenticeshipEmailOverlaps.First().Id;
            var apprenticeshipId2 = f.CohortDetails.ApprenticeshipEmailOverlaps.Last().Id;

            var result = await f.Map();
            var course = result.Courses.FirstOrDefault();

            Assert.NotNull(course);
            Assert.NotNull(course.EmailOverlaps);
            Assert.AreEqual(2, course.EmailOverlaps.NumberOfEmailOverlaps);
            Assert.AreEqual(2, course.DraftApprenticeships.Count(x => x.HasOverlappingEmail));
            Assert.IsTrue(course.DraftApprenticeships.First(x => x.Id == apprenticeshipId1).HasOverlappingEmail);
            Assert.IsTrue(course.DraftApprenticeships.First(x => x.Id == apprenticeshipId2).HasOverlappingEmail);
        }

        [Test]
        public async Task HasEmailOverlapsIsMappedCorrectlyWhenThereAreEmailOverlaps()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().WithOneEmailOverlapping();
            var result = await fixture.Map();
            Assert.IsTrue(result.HasEmailOverlaps);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_PendingApproval_From_TransferSender()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetTransferSender()
                .SetCohortWithParty(Party.TransferSender);

            fixture.CohortDetails.IsApprovedByEmployer = fixture.CohortDetails.IsApprovedByProvider = true;
            fixture.CohortDetails.TransferApprovalStatus = TransferApprovalStatus.Pending;

            var result = await fixture.Map();
            Assert.AreEqual("Pending - with funding employer", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_rejected_From_TransferSender()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetTransferSender()
                .SetCohortWithParty(Party.TransferSender)
                .SetTransferApprovalStatus(TransferApprovalStatus.Rejected)
                .SetCohortApprovedStatus(true);

            fixture.CohortDetails.IsApprovedByEmployer = fixture.CohortDetails.IsApprovedByProvider = true;
            fixture.CohortDetails.TransferApprovalStatus = TransferApprovalStatus.Rejected;

            var result = await fixture.Map();
            Assert.AreEqual("Rejected by transfer sending employer", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithProvider_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.CohortDetails.LastAction = LastAction.None;

            var result = await fixture.Map();
            Assert.AreEqual("New request", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Provider_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.CohortDetails.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.AreEqual("Ready for review", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Provider_With_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.CohortDetails.LastAction = LastAction.Approve;
            fixture.CohortDetails.IsApprovedByEmployer = true;

            var result = await fixture.Map();
            Assert.AreEqual("Ready for approval", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithEmployer_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.None;

            var result = await fixture.Map();
            Assert.AreEqual("New request", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.AreEqual("Under review with employer", result.Status);
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_With_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.Approve;


            var result = await fixture.Map();
            Assert.AreEqual("With Employer for approval", result.Status);
        }

        [TestCase(nameof(DraftApprenticeshipDto.FirstName))]
        [TestCase(nameof(DraftApprenticeshipDto.LastName))]
        [TestCase(nameof(DraftApprenticeshipDto.CourseName))]
        [TestCase(nameof(DraftApprenticeshipDto.DateOfBirth))]
        [TestCase(nameof(DraftApprenticeshipDto.EndDate))]
        [TestCase(nameof(DraftApprenticeshipDto.Cost))]
        [TestCase(nameof(DraftApprenticeshipDto.Uln))]
        public async Task IsCompleteMappedCorrectlyWhenAManadatoryFieldIsNull(string propertyName)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship()
                .SetValueOfDraftApprenticeshipProperty(propertyName, null);
            var result = await fixture.Map();
            Assert.IsFalse(result.Courses.First().DraftApprenticeships.First().IsComplete);
        }

        [TestCase(nameof(DraftApprenticeshipDto.TrainingPrice))]
        [TestCase(nameof(DraftApprenticeshipDto.EndPointAssessmentPrice))]
        public async Task IsCompleteIsFalseWhenPilotApprenticeshipMissingEitherPriceComponent(string propertyName)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship()
                .SetIsOnFlexiPaymentPilotFlag(true)
                .SetValueOfDraftApprenticeshipProperty(propertyName, null);
            var result = await fixture.Map();
            Assert.IsFalse(result.Courses.First().DraftApprenticeships.First().IsComplete);
        }

        [TestCase(nameof(DraftApprenticeshipDto.TrainingPrice))]
        [TestCase(nameof(DraftApprenticeshipDto.EndPointAssessmentPrice))]
        public async Task IsCompleteIsTrueWhenNonPilotApprenticeshipMissingEitherPriceComponent(string propertyName)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship()
                .SetIsOnFlexiPaymentPilotFlag(false)
                .SetValueOfDraftApprenticeshipProperty(propertyName, null);
            var result = await fixture.Map();
            Assert.IsTrue(result.Courses.First().DraftApprenticeships.First().IsComplete);
        }

        [Test]
        public async Task IsCompleteIsFalseWhenStartDatesAreBothNull()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship()
                .SetValueOfDraftApprenticeshipProperty("StartDate", null)
                .SetValueOfDraftApprenticeshipProperty("ActualStartDate", null);
            var result = await fixture.Map();
            Assert.IsFalse(result.Courses.First().DraftApprenticeships.First().IsComplete);
        }

        [Test]
        public async Task Course4Has2CourseLines()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(2, result.Courses.Count(c => c.CourseCode == "C4"));
        }

        [TestCase(DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular)]
        public async Task Course4HasCorrectDeployMethod(DeliveryModel dm)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.AreEqual(1, result.Courses.Count(c => c.CourseCode == "C4" && c.DeliveryModel == dm));
        }

        [Test]
        public async Task Employment_dates_are_mapped_correctly_when_flexijob()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship(build => build
                    .With(x => x.DeliveryModel, DeliveryModel.PortableFlexiJob)
                    .With(x => x.StartDate, new DateTime(2019, 11, 01))
                    .With(x => x.EmploymentEndDate, new DateTime(2019, 12, 01)));
            var result = await fixture.Map();
            result
                .Courses.First().DraftApprenticeships.Single().DisplayEmploymentDates.Should().Be("Nov 2019 to Dec 2019");
        }

        [TestCase(DeliveryModel.Regular, nameof(DraftApprenticeshipDto.EmploymentEndDate), true)]
        [TestCase(DeliveryModel.PortableFlexiJob, nameof(DraftApprenticeshipDto.EmploymentEndDate), false)]
        [TestCase(DeliveryModel.Regular, nameof(DraftApprenticeshipDto.EmploymentPrice), true)]
        [TestCase(DeliveryModel.PortableFlexiJob, nameof(DraftApprenticeshipDto.EmploymentPrice), false)]
        public async Task IsCompleteMappedCorrectlyWhenMandatoryFlexibleFieldIsNull(DeliveryModel deliveryModel, string propertyName, bool isComplete)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship(build => build.With(x => x.DeliveryModel, deliveryModel))
                .SetValueOfDraftApprenticeshipProperty(propertyName, null);

            var result = await fixture.Map();

            result.Courses.First().DraftApprenticeships.First().IsComplete.Should().Be(isComplete);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task IsCompleteMappedCorrectlyWhenRecognisingPriorLearningStillNeedsToBeConsideredIsSet(bool recognisingPriorLearningStillNeedsConsideration, bool isComplete)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship(build => build.With(x => x.RecognisingPriorLearningStillNeedsToBeConsidered, recognisingPriorLearningStillNeedsConsideration));

            var result = await fixture.Map();

            result.Courses.First().DraftApprenticeships.First().IsComplete.Should().Be(isComplete);
        }


        [TestCase(false, true)]
        public async Task IsCompleteMappedCorrectlyWhenExtendedRecognisingPriorLearningStillNeedsToBeConsideredIsSet(bool recognisingPriorLearningStillNeedsConsideration, bool isComplete)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship(build => build.With(x => x.RecognisingPriorLearningExtendedStillNeedsToBeConsidered, recognisingPriorLearningStillNeedsConsideration));

            var result = await fixture.Map();

            result.Courses.First().DraftApprenticeships.First().IsComplete.Should().Be(isComplete);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShowRofjaaRemovalBanner(bool hasUnavailableFlexiJobAgencyDeliveryModel, bool expectShowBanner)
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .UnavailableFlexiJobAgencyDeliveryModel(hasUnavailableFlexiJobAgencyDeliveryModel);

            var result = await fixture.Map();

            result.ShowRofjaaRemovalBanner.Should().Be(expectShowBanner);
        }
    }

    public class DetailsViewModelMapperTestsFixture
    {
        public DetailsViewModelMapper Mapper;
        public DetailsRequest Source;
        public DetailsViewModel Result;
        public Mock<ICommitmentsApiClient> CommitmentsApiClient;
        public Mock<IPasAccountApiClient> PasAccountApiClient;
        public Mock<IOuterApiClient> OuterApiClient;
        public Mock<IEncodingService> EncodingService;
        public GetCohortDetailsResponse CohortDetails;
        public DateTime DefaultStartDate = new DateTime(2019, 10, 1);
        public AccountLegalEntityResponse AccountLegalEntityResponse;
        public ProviderAgreement ProviderAgreement;
        private Fixture _autoFixture;
        private TrainingProgramme _trainingProgramme;
        private List<TrainingProgrammeFundingPeriod> _fundingPeriods;
        private DateTime _startFundingPeriod = new DateTime(2019, 10, 1);
        private DateTime _endFundingPeriod = new DateTime(2019, 10, 30);
        private Mock<IAuthorizationService> _providerFeatureToggle;

        public DetailsViewModelMapperTestsFixture()
        {
            _autoFixture = new Fixture();

            var draftApprenticeships = CreateDraftApprenticeshipDtos(_autoFixture);
            _autoFixture.Register(() => draftApprenticeships);

            AccountLegalEntityResponse = _autoFixture.Create<AccountLegalEntityResponse>();
            ProviderAgreement = new ProviderAgreement { Status = ProviderAgreementStatus.Agreed };
            CohortDetails = _autoFixture.Build<GetCohortDetailsResponse>()
                .Without(x => x.TransferSenderId)
                .With(x => x.IsCompleteForProvider, true)
                .Without(x => x.ChangeOfPartyRequestId)
                .With(x => x.HasUnavailableFlexiJobAgencyDeliveryModel, false)
                .With(x => x.HasNoDeclaredStandards, false)
                .With(x => x.InvalidProviderCourseCodes, Enumerable.Empty<string>())
                .With(x=>x.DraftApprenticeships, draftApprenticeships)
                .With(x=>x.ApprenticeshipEmailOverlaps, new List<ApprenticeshipEmailOverlap>())
                .Create();

            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(AccountLegalEntityResponse);

            PasAccountApiClient = new Mock<IPasAccountApiClient>();
            PasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), CancellationToken.None)).ReturnsAsync(ProviderAgreement);

            OuterApiClient = new Mock<IOuterApiClient>();
            OuterApiClient.Setup(x => x.Get<GetCohortDetailsResponse>(It.IsAny<GetCohortDetailsRequest>()))
                .ReturnsAsync(CohortDetails);

            _providerFeatureToggle = new Mock<IAuthorizationService>();
            _providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            _fundingPeriods = new List<TrainingProgrammeFundingPeriod>
            {
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, FundingCap = 1000},
                new TrainingProgrammeFundingPeriod{ EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), FundingCap = 500}
            };
            _trainingProgramme = new TrainingProgramme { EffectiveFrom = DefaultStartDate, EffectiveTo = DefaultStartDate.AddYears(1), FundingPeriods = _fundingPeriods };

            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme(It.Is<string>(c => !string.IsNullOrEmpty(c)), CancellationToken.None))
                .ReturnsAsync(new GetTrainingProgrammeResponse { TrainingProgramme = _trainingProgramme });

            CommitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), CancellationToken.None))
               .ReturnsAsync(new ValidateUlnOverlapResult { HasOverlappingEndDate = false, HasOverlappingStartDate = false });
            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme("no-course", CancellationToken.None))
                .ThrowsAsync(new RestHttpClientException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    RequestMessage = new HttpRequestMessage(),
                    ReasonPhrase = "Url not found"
                }, "Course not found"));

            EncodingService = new Mock<IEncodingService>();
            SetEncodingOfApprenticeIds();
            

            Mapper = new DetailsViewModelMapper(CommitmentsApiClient.Object, EncodingService.Object, PasAccountApiClient.Object, OuterApiClient.Object, Mock.Of<ITempDataStorageService>(), _providerFeatureToggle.Object);
            Source = _autoFixture.Create<DetailsRequest>();
        }

        public DetailsViewModelMapperTestsFixture SetCohortWithParty(Party party)
        {
            CohortDetails.WithParty = party;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetTransferSenderIdAndItsExpectedHashedValue(long? transferSenderId, string expectedHashedId)
        {
            CohortDetails.TransferSenderId = transferSenderId;
            if (transferSenderId.HasValue)
            {
                EncodingService.Setup(x => x.Encode(transferSenderId.Value, EncodingType.PublicAccountId))
                    .Returns(expectedHashedId);
            }

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetPledgeApplicationIdAndItsExpectedHashedValue(int? pledgeApplicationId, string expectedEncodedId)
        {
            CohortDetails.PledgeApplicationId = pledgeApplicationId;
            if (pledgeApplicationId.HasValue)
            {
                EncodingService.Setup(x => x.Encode(pledgeApplicationId.Value, EncodingType.PledgeApplicationId))
                    .Returns(expectedEncodedId);
            }

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetEncodingOfApprenticeIds()
        {
            EncodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId))
                .Returns((long p, EncodingType t) => $"X{p}X");

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetIsOnFlexiPaymentPilotFlag(bool? flag)
        {
            var draftApprenticeship = CohortDetails.DraftApprenticeships.First();
            draftApprenticeship.IsOnFlexiPaymentPilot = flag;
            return this;
        }

        public DetailsViewModelMapperTestsFixture CreateThisNumberOfApprenticeships(int numberOfApprenticeships)
        {
            var draftApprenticeships = _autoFixture.CreateMany<DraftApprenticeshipDto>(numberOfApprenticeships).ToArray();
            CohortDetails.DraftApprenticeships = draftApprenticeships;
            return this;
        }

        public DetailsViewModelMapperTestsFixture WithOneEmailOverlapping()
        {
            CreateThisNumberOfApprenticeships(10);
            var first = CohortDetails.DraftApprenticeships.First();
            var emailOverlap = _autoFixture.Build<ApprenticeshipEmailOverlap>().With(x => x.Id, first.Id).Create();
            CohortDetails.ApprenticeshipEmailOverlaps = new List<ApprenticeshipEmailOverlap> { emailOverlap };

            return this;
        }

        public DetailsViewModelMapperTestsFixture WithTwoEmailOverlappingOnSameCourse()
        {
            var draftApprenticeships = _autoFixture.CreateMany<DraftApprenticeshipDto>(5).ToArray();
            foreach (var draftApprenticeship in draftApprenticeships)
            {
                draftApprenticeship.CourseCode = "ABC";
                draftApprenticeship.CourseName = "ABC Name";
                draftApprenticeship.DeliveryModel = DeliveryModel.Regular;
            }
            CohortDetails.DraftApprenticeships = draftApprenticeships;
            var first = CohortDetails.DraftApprenticeships.First();
            var last = CohortDetails.DraftApprenticeships.Last();
            var emailOverlap1 = _autoFixture.Build<ApprenticeshipEmailOverlap>().With(x => x.Id, first.Id).Create();
            var emailOverlap2 = _autoFixture.Build<ApprenticeshipEmailOverlap>().With(x => x.Id, last.Id).Create();
            CohortDetails.ApprenticeshipEmailOverlaps = new List<ApprenticeshipEmailOverlap> { emailOverlap1, emailOverlap2 };

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetValueOfDraftApprenticeshipProperty(string propertyName, object value)
        {
            var draftApprenticeship = CohortDetails.DraftApprenticeships.First();
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                PropertyInfo propertyInfo = draftApprenticeship.GetType().GetProperty(propertyName);
                // make sure object has the property we are after
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(draftApprenticeship, value, null);
                }
            }

            return this;
        }

        public DetailsViewModelMapperTestsFixture CreateDraftApprenticeship(
            Func<ICustomizationComposer<DraftApprenticeshipDto>,
                IPostprocessComposer<DraftApprenticeshipDto>> build = null)
        {
            build ??= x => x;
            var draftApprenticeshipBuilder = _autoFixture.Build<DraftApprenticeshipDto>();
            var draftApprenticeship = build(draftApprenticeshipBuilder).Create();

            CohortDetails.DraftApprenticeships = new List<DraftApprenticeshipDto>() { draftApprenticeship };
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
            Assert.AreEqual(source.TrainingPrice, result.TrainingPrice);
            Assert.AreEqual(source.EndPointAssessmentPrice, result.EndPointAssessmentPrice);
            Assert.AreEqual(source.EmploymentPrice, result.EmploymentPrice);
            Assert.AreEqual(source.EmploymentEndDate, result.EmploymentEndDate);
            Assert.AreEqual(source.StartDate, result.StartDate);
            Assert.AreEqual(source.ActualStartDate, result.ActualStartDate);
            Assert.AreEqual(source.EndDate, result.EndDate);
            Assert.AreEqual($"X{source.Id}X", result.DraftApprenticeshipHashedId);
            Assert.AreEqual(source.IsOnFlexiPaymentPilot, result.IsOnFlexiPaymentPilot);
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
            var draftApprenticeships = autoFixture.CreateMany<DraftApprenticeshipDto>(8).ToArray();
            SetCourseDetails(draftApprenticeships[0], "Course1", "C1", 1000);
            SetCourseDetails(draftApprenticeships[1], "Course1", "C1", 2100);
            SetCourseDetails(draftApprenticeships[2], "Course1", "C1", 2000, DefaultStartDate.AddMonths(1));

            SetCourseDetails(draftApprenticeships[3], "Course2", "C2", 1500);
            SetCourseDetails(draftApprenticeships[4], "Course2", "C2", null);

            SetCourseDetails(draftApprenticeships[5], "Course3", "C3", null, DefaultStartDate.AddMonths(2));

            SetCourseDetails(draftApprenticeships[6], "Course4", "C4", null, null, null, DeliveryModel.PortableFlexiJob);
            SetCourseDetails(draftApprenticeships[7], "Course4", "C4", null, null, null, DeliveryModel.Regular);

            return draftApprenticeships;
        }

        private void SetCourseDetails(DraftApprenticeshipDto draftApprenticeship, string courseName, string courseCode, int? cost, DateTime? startDate = null, DateTime? originalStartDate = null, DeliveryModel dm = DeliveryModel.Regular)
        {
            startDate = startDate ?? DefaultStartDate;

            draftApprenticeship.CourseName = courseName;
            draftApprenticeship.CourseCode = courseCode;
            draftApprenticeship.Cost = cost;
            draftApprenticeship.StartDate = startDate;
            draftApprenticeship.OriginalStartDate = originalStartDate;
            draftApprenticeship.DeliveryModel = dm;
            draftApprenticeship.IsOnFlexiPaymentPilot = false;
        }

        public DetailsViewModelMapperTestsFixture SetProviderComplete(bool providerComplete)
        {
            CohortDetails.IsCompleteForProvider = providerComplete;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetTransferSender()
        {
            CohortDetails.TransferSenderId = _autoFixture.Create<long>();
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetRplErrorsToNone()
        {
            CohortDetails.RplErrorDraftApprenticeshipIds = new List<long>();
            return this;
        }


        public DetailsViewModelMapperTestsFixture SetTransferApprovalStatus(TransferApprovalStatus transferApprovalStatus)
        {
            CohortDetails.TransferApprovalStatus = transferApprovalStatus; ;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetCohortApprovedStatus(bool isApproved)
        {
            CohortDetails.IsApprovedByEmployer = CohortDetails.IsApprovedByProvider = isApproved; ;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetupChangeOfPartyScenario()
        {
            CohortDetails.ChangeOfPartyRequestId = 1;
            var draftApprenticeship = CohortDetails.DraftApprenticeships.First();
            draftApprenticeship.OriginalStartDate = _fundingPeriods.First().EffectiveFrom;
            draftApprenticeship.StartDate = _fundingPeriods.Last().EffectiveFrom;
            draftApprenticeship.Cost = _fundingPeriods.First().FundingCap + 500;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetIsChangeOfParty(bool isChangeOfParty)
        {
            CohortDetails.ChangeOfPartyRequestId = isChangeOfParty ? _autoFixture.Create<long>() : default(long?);

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetNoCourse()
        {
            CohortDetails.DraftApprenticeships = CohortDetails.DraftApprenticeships.Select(c =>
            {
                c.CourseCode = "no-course";
                return c;
            }).ToList();
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetNoCourseSet()
        {
            CohortDetails.DraftApprenticeships = CohortDetails.DraftApprenticeships.Select(c =>
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

        public DetailsViewModelMapperTestsFixture UnavailableFlexiJobAgencyDeliveryModel(bool hasUnavailableFlexiJobAgencyDeliveryModel)
        {
            CohortDetails.HasUnavailableFlexiJobAgencyDeliveryModel = hasUnavailableFlexiJobAgencyDeliveryModel;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetHasInvalidCourse(bool hasInvalidCourse)
        {
            CohortDetails.InvalidProviderCourseCodes = hasInvalidCourse ? new List<string> { "test-invalid-course-code" } : Enumerable.Empty<string>();
            return this;
        }
    }
}
