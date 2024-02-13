using AutoFixture.Dsl;
using FluentAssertions;
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
            Assert.That(result.ProviderId, Is.EqualTo(fixture.Source.ProviderId));
        }

        [Test]
        public async Task WithPartyIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.WithParty, Is.EqualTo(fixture.CohortDetails.WithParty));
        }

        [Test]
        public async Task LegalEntityNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.LegalEntityName, Is.EqualTo(fixture.CohortDetails.LegalEntityName));
        }

        [Test]
        public async Task ProviderNameIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.ProviderName, Is.EqualTo(fixture.CohortDetails.ProviderName));
        }

        [Test]
        public async Task MessageIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.Message, Is.EqualTo(fixture.CohortDetails.LatestMessageCreatedByEmployer));
        }

        [TestCase(true, true, "No, send to employer to review or add details")]
        [TestCase(true, false, "Yes, send to employer to review or add details")]
        [TestCase(false, true, "Yes, send to employer to review or add details")]
        [TestCase(false, false, "Yes, send to employer to review or add details")]
        public async Task SendBackToEmployerOptionMessageIsMappedCorrectly(bool isAgreementSigned, bool providerComplete, string expected)
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetIsAgreementSigned(isAgreementSigned).SetProviderComplete(providerComplete).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.SendBackToEmployerOptionMessage, Is.EqualTo(expected));
        }

        [Test]
        public async Task CohortReferenceIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.CohortReference, Is.EqualTo(fixture.Source.CohortReference));
        }

        [Test]
        public async Task IsApprovedByEmployerIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.IsApprovedByEmployer, Is.EqualTo(fixture.CohortDetails.IsApprovedByEmployer));
        }

        [Test]
        public async Task TransferSenderHashedIdIsEncodedCorrectlyWhenThereIsAValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetTransferSenderIdAndItsExpectedHashedValue(123, "X123X");
            var result = await fixture.Map();
            Assert.That(result.TransferSenderHashedId, Is.EqualTo("X123X"));
        }

        [Test]
        public async Task TransferSenderHashedIdIsNullWhenThereIsNoValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetTransferSenderIdAndItsExpectedHashedValue(null, null);
            var result = await fixture.Map();
            Assert.That(result.TransferSenderHashedId, Is.Null);
        }

        [Test]
        public async Task PledgeApplicationIdIsEncodedCorrectlyWhenThereIsAValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetPledgeApplicationIdAndItsExpectedHashedValue(567, "Z567Z");
            var result = await fixture.Map();
            Assert.That(result.EncodedPledgeApplicationId, Is.EqualTo("Z567Z"));
        }

        [Test]
        public async Task PledgeApplicationIdIsNullWhenThereIsNoValue()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetPledgeApplicationIdAndItsExpectedHashedValue(null, null);
            var result = await fixture.Map();
            Assert.That(result.EncodedPledgeApplicationId, Is.Null);
        }

        [Test]
        public async Task DraftApprenticeshipTotalCountIsReportedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.DraftApprenticeshipsCount, Is.EqualTo(fixture.CohortDetails.DraftApprenticeships.Count));
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

                Assert.That(course.Count, Is.EqualTo(expectedCount));
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

            DetailsViewModelMapperTestsFixture.AssertSequenceOrder(expectedSequence, actualSequence, (e, a) => e.CourseName == a.CourseName && e.CourseCode == a.CourseCode && e.DeliveryModel == a.DeliveryModel);
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

                DetailsViewModelMapperTestsFixture.AssertSequenceOrder(expectedSequence, actualSequence, (e, a) => e == a);
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

                DetailsViewModelMapperTestsFixture.AssertEquality(draftApprenticeship, draftApprenticeshipResult);
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

                Assert.That(draftApprenticeshipResult.FundingBandCap, Is.EqualTo(1000));
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

                Assert.That(draftApprenticeshipResult.FundingBandCap, Is.EqualTo(null));
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

                Assert.That(draftApprenticeshipResult.FundingBandCap, Is.EqualTo(null));
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

                Assert.That(draftApprenticeshipResult.FundingBandCap, Is.EqualTo(null));
            }
        }

        [Test]
        public async Task FundingBandExcessModelShowsTwoApprenticeshipsExceedingTheBandForCourse1()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C1").FundingBandExcess;
            Assert.That(excessModel.NumberOfApprenticesExceedingFundingBandCap, Is.EqualTo(2));
        }

        [Test]
        public async Task FundingBandExcessModelShowsOnlyTheFullStopWhenMultipleFundingCapsAreExceeded()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C1").FundingBandExcess;
            Assert.That(excessModel.DisplaySingleFundingBandCap, Is.EqualTo("."));
        }

        [Test]
        public async Task FundingBandExcessModelShowsOneApprenticeshipExceedingTheBandForCourse2()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C2").FundingBandExcess;
            Assert.That(excessModel.NumberOfApprenticesExceedingFundingBandCap, Is.EqualTo(1));
        }

        [Test]
        public async Task FundingBandExcessModelShowsTheSingleFundingBandCapExceeded()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C2").FundingBandExcess;
            Assert.That(excessModel.DisplaySingleFundingBandCap, Is.EqualTo(" of £1,000."));
        }

        [Test]
        public async Task FundingBandExcessModelIsNullForCourse3()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            var excessModel = result.Courses.FirstOrDefault(x => x.CourseCode == "C3").FundingBandExcess;
            Assert.That(excessModel, Is.Null);
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

            Assert.That(result.PageTitle, Is.EqualTo(expectedPageTitle));
        }

        [TestCase("C2", "1 apprenticeship above funding band maximum")]
        [TestCase("C1", "2 apprenticeships above funding band maximum")]
        public async Task FundingBandCapExcessHeaderIsSetCorrectlyForTheNumberOfApprenticeshipsOverFundingCap(string courseCode, string expectedFundingBandCapExcessHeader)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            Assert.That(result.Courses.FirstOrDefault(x => x.CourseCode == courseCode).FundingBandExcess.FundingBandCapExcessHeader, Is.EqualTo(expectedFundingBandCapExcessHeader));
        }

        [TestCase("C2", "The price for this apprenticeship is above its")]
        [TestCase("C1", "The price for these apprenticeships is above the")]
        public async Task FundingBandCapExcessLabelIsSetCorrectlyForTheNumberOfApprenticeshipsOverFundingCap(string courseCode, string expectedFundingBandCapExcessLabel)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();

            Assert.That(result.Courses.FirstOrDefault(x => x.CourseCode == courseCode).FundingBandExcess.FundingBandCapExcessLabel, Is.EqualTo(expectedFundingBandCapExcessLabel));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task IsAgreementSignedIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedIsAgreementSigned)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.That(result.IsAgreementSigned, Is.EqualTo(expectedIsAgreementSigned));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task IsAgreementSignedIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool expectedIsAgreementSigned)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.That(result.IsAgreementSigned, Is.EqualTo(expectedIsAgreementSigned));
        }

        [TestCase(true, "Approve these details?")]
        [TestCase(false, "Submit to employer?")]
        public async Task OptionsTitleIsMappedCorrectlyWithATransfer(bool isAgreementSigned, string expectedOptionsTitle)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.OptionsTitle, Is.EqualTo(expectedOptionsTitle));
        }

        [TestCase(true, "Approve these details?")]
        [TestCase(false, "Submit to employer?")]
        public async Task OptionsTitleIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, string expectedOptionsTitle)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.OptionsTitle, Is.EqualTo(expectedOptionsTitle));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowViewAgreementOptionIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedShowViewAgreementOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned);
            var result = await fixture.Map();
            Assert.That(result.ShowViewAgreementOption, Is.EqualTo(expectedShowViewAgreementOption));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithATransfer(bool isAgreementSigned, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetTransferSender().SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.ProviderCanApprove, Is.EqualTo(expectedShowApprovalOption));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowAddAnotherApprenticeOptionIsMappedCorrectly(bool isChangeOfParty, bool expectedShowAddAnotherOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsChangeOfParty(isChangeOfParty);
            var result = await fixture.Map();
            Assert.That(result.ShowAddAnotherApprenticeOption, Is.EqualTo(expectedShowAddAnotherOption));
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.ProviderCanApprove, Is.EqualTo(expectedShowApprovalOption));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWithAnInvalidCourse(bool hasInvalidCourse, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetHasInvalidCourse(hasInvalidCourse).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.ProviderCanApprove, Is.EqualTo(expectedShowApprovalOption));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task ShowApprovalOptionIsMappedCorrectlyWhenOverlap(bool hasOverlap, bool expectedShowApprovalOption)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            fixture.SetUlnOverlap(hasOverlap).SetRplErrorsToNone();
            var result = await fixture.Map();

            Assert.That(result.ProviderCanApprove, Is.EqualTo(expectedShowApprovalOption));
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
            Assert.That(result.ShowApprovalOptionMessage, Is.EqualTo(expectedShowApprovalOptionMessage));
        }

        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(true, true, false, false)]
        [TestCase(false, false, false, false)]
        public async Task ShowApprovalOptionMessageIsMappedCorrectlyWithoutATransfer(bool isAgreementSigned, bool showApprovalOption,
            bool isApprovedByEmployer, bool expectedShowApprovalOptionMessage)
        {
            var fixture = new DetailsViewModelMapperTestsFixture
            {
                CohortDetails =
                {
                    IsApprovedByEmployer = isApprovedByEmployer
                }
            };
            fixture.SetIsAgreementSigned(isAgreementSigned).SetRplErrorsToNone();
            var result = await fixture.Map();
            Assert.That(result.ShowApprovalOptionMessage, Is.EqualTo(expectedShowApprovalOptionMessage));
        }

        [Test]
        public async Task ShowApprovalOfCohortAsTrueWhenNoRplErrors()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().SetRplErrorsToNone();
            fixture.CohortDetails.IsApprovedByEmployer = true;
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.That(result.ShowApprovalOptionMessage, Is.True);
        }

        [Test]
        [Ignore("Ignore this test until RPL is no longer just a warning")]
        public async Task ShowApprovalOfCohortAsFalseWhenRplErrorsExist()
        {
            var fixture = new DetailsViewModelMapperTestsFixture
            {
                CohortDetails =
                {
                    IsApprovedByEmployer = true
                }
            };
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.That(result.ShowApprovalOptionMessage, Is.False);
        }

        [Test]
        public async Task ShowApprovalOfCohortAsTrueWhenRplErrorsExist()
        {
            var fixture = new DetailsViewModelMapperTestsFixture
            {
                CohortDetails =
                {
                    IsApprovedByEmployer = true
                }
            };
            fixture.SetTransferSender().SetIsAgreementSigned(true);
            var result = await fixture.Map();
            Assert.That(result.ShowApprovalOptionMessage, Is.True);
        }

        [Test]
        public async Task IsCompleteForProviderIsMappedCorrectly()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.IsCompleteForProvider, Is.EqualTo(fixture.CohortDetails.IsCompleteForProvider));
        }

        [Test]
        public async Task FundingCapIsMappedCorrectlyForChangeOfPartyApprentice()
        {
            var result = await new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetupChangeOfPartyScenario()
                .Map();

            Assert.Multiple(() =>
            {
                Assert.That(result.Courses.First().DraftApprenticeships.First().FundingBandCap, Is.EqualTo(1000));
                Assert.That(result.Courses.First().DraftApprenticeships.First().ExceedsFundingBandCap, Is.EqualTo(true));
            });
        }

        [Test]
        public async Task EmailOverlapIsMappedCorrectlyToDraftApprenticeshipAndToSummaryLine()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().WithOneEmailOverlapping();
            var apprenticeshipId = fixture.CohortDetails.ApprenticeshipEmailOverlaps.First().Id;

            var result = await fixture.Map();
            var course = result.Courses.FirstOrDefault(x => x.DraftApprenticeships.Any(y => y.Id == apprenticeshipId));

            Assert.Multiple(() =>
            {
                Assert.That(course, Is.Not.Null);
                Assert.That(course.EmailOverlaps, Is.Not.Null);
                Assert.That(course.EmailOverlaps.NumberOfEmailOverlaps, Is.EqualTo(1));
                Assert.That(course.DraftApprenticeships.Count(x => x.HasOverlappingEmail), Is.EqualTo(1));
                Assert.That(course.DraftApprenticeships.First(x => x.HasOverlappingEmail).Id, Is.EqualTo(apprenticeshipId));
            });
        }

        [Test]
        public async Task EmailOverlapIsMappedCorrectlyToDraftApprenticeshipsAndToSummaryLineWhenTwoEmailOverlapsExistOnSameCourse()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().WithTwoEmailOverlappingOnSameCourse();
            var apprenticeshipId1 = fixture.CohortDetails.ApprenticeshipEmailOverlaps.First().Id;
            var apprenticeshipId2 = fixture.CohortDetails.ApprenticeshipEmailOverlaps.Last().Id;

            var result = await fixture.Map();
            var course = result.Courses.FirstOrDefault();

           Assert.Multiple(() =>
            {
                Assert.That(course, Is.Not.Null);
                Assert.That(course.EmailOverlaps, Is.Not.Null);
                Assert.That(course.EmailOverlaps.NumberOfEmailOverlaps, Is.EqualTo(2));
                Assert.That(course.DraftApprenticeships.Count(x => x.HasOverlappingEmail), Is.EqualTo(2));
                Assert.That(course.DraftApprenticeships.First(x => x.Id == apprenticeshipId1).HasOverlappingEmail, Is.True);
                Assert.That(course.DraftApprenticeships.First(x => x.Id == apprenticeshipId2).HasOverlappingEmail, Is.True);
            });
        }

        [Test]
        public async Task HasEmailOverlapsIsMappedCorrectlyWhenThereAreEmailOverlaps()
        {
            var fixture = new DetailsViewModelMapperTestsFixture().WithOneEmailOverlapping();
            var result = await fixture.Map();
            Assert.That(result.HasEmailOverlaps, Is.True);
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
            Assert.That(result.Status, Is.EqualTo("Pending - with funding employer"));
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
            Assert.That(result.Status, Is.EqualTo("Rejected by transfer sending employer"));
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithProvider_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.CohortDetails.LastAction = LastAction.None;

            var result = await fixture.Map();
            Assert.That(result.Status, Is.EqualTo("New request"));
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Provider_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Provider);

            fixture.CohortDetails.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.That(result.Status, Is.EqualTo("Ready for review"));
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
            Assert.That(result.Status, Is.EqualTo("Ready for approval"));
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_WithEmployer_And_New_Cohort()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.None;

            var result = await fixture.Map();
            Assert.That(result.Status, Is.EqualTo("New request"));
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_But_Without_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.Amend;

            var result = await fixture.Map();
            Assert.That(result.Status, Is.EqualTo("Under review with employer"));
        }

        [Test]
        public async Task StatusIsMappedCorrectly_When_With_Employer_With_Employer_Approval()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateThisNumberOfApprenticeships(1)
                .SetCohortWithParty(Party.Employer);

            fixture.CohortDetails.LastAction = LastAction.Approve;

            var result = await fixture.Map();
            Assert.That(result.Status, Is.EqualTo("With Employer for approval"));
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
            Assert.That(result.Courses.First().DraftApprenticeships.First().IsComplete, Is.False);
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
            Assert.That(result.Courses.First().DraftApprenticeships.First().IsComplete, Is.False);
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
            Assert.That(result.Courses.First().DraftApprenticeships.First().IsComplete, Is.True);
        }

        [Test]
        public async Task IsCompleteIsFalseWhenStartDatesAreBothNull()
        {
            var fixture = new DetailsViewModelMapperTestsFixture()
                .CreateDraftApprenticeship()
                .SetValueOfDraftApprenticeshipProperty("StartDate", null)
                .SetValueOfDraftApprenticeshipProperty("ActualStartDate", null);
            var result = await fixture.Map();
            Assert.That(result.Courses.First().DraftApprenticeships.First().IsComplete, Is.False);
        }

        [Test]
        public async Task Course4Has2CourseLines()
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.Courses.Count(c => c.CourseCode == "C4"), Is.EqualTo(2));
        }

        [TestCase(DeliveryModel.PortableFlexiJob)]
        [TestCase(DeliveryModel.Regular)]
        public async Task Course4HasCorrectDeployMethod(DeliveryModel dm)
        {
            var fixture = new DetailsViewModelMapperTestsFixture();
            var result = await fixture.Map();
            Assert.That(result.Courses.Count(c => c.CourseCode == "C4" && c.DeliveryModel == dm), Is.EqualTo(1));
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
        private readonly Mock<IPasAccountApiClient> _pasAccountApiClient;
        private readonly DetailsViewModelMapper _mapper;
        private readonly Mock<IEncodingService> _encodingService;
        public ProviderAgreement ProviderAgreement;
        private readonly Fixture _autoFixture;
        private readonly List<TrainingProgrammeFundingPeriod> _fundingPeriods;
        private readonly DateTime _startFundingPeriod = new(2019, 10, 1);
        private readonly DateTime _endFundingPeriod = new(2019, 10, 30);

        public DetailsRequest Source { get; }
        public Mock<ICommitmentsApiClient> CommitmentsApiClient { get; }
        public GetCohortDetailsResponse CohortDetails { get; }
        public DateTime DefaultStartDate = new(2019, 10, 1);

        public DetailsViewModelMapperTestsFixture()
        {
            _autoFixture = new Fixture();

            var draftApprenticeships = CreateDraftApprenticeshipDtos(_autoFixture);
            _autoFixture.Register(() => draftApprenticeships);

            var accountLegalEntityResponse = _autoFixture.Create<AccountLegalEntityResponse>();
            ProviderAgreement = new ProviderAgreement { Status = ProviderAgreementStatus.Agreed };
            CohortDetails = _autoFixture.Build<GetCohortDetailsResponse>()
                .Without(x => x.TransferSenderId)
                .With(x => x.IsCompleteForProvider, true)
                .Without(x => x.ChangeOfPartyRequestId)
                .With(x => x.HasUnavailableFlexiJobAgencyDeliveryModel, false)
                .With(x => x.HasNoDeclaredStandards, false)
                .With(x => x.InvalidProviderCourseCodes, Enumerable.Empty<string>())
                .With(x => x.DraftApprenticeships, draftApprenticeships)
                .With(x => x.ApprenticeshipEmailOverlaps, new List<ApprenticeshipEmailOverlap>())
                .Create();

            CommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            CommitmentsApiClient.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountLegalEntityResponse);

            _pasAccountApiClient = new Mock<IPasAccountApiClient>();
            _pasAccountApiClient.Setup(x => x.GetAgreement(It.IsAny<long>(), CancellationToken.None)).ReturnsAsync(ProviderAgreement);

            var outerApiClient = new Mock<IOuterApiClient>();
            outerApiClient.Setup(x => x.Get<GetCohortDetailsResponse>(It.IsAny<GetCohortDetailsRequest>()))
                .ReturnsAsync(CohortDetails);

            var providerFeatureToggle = new Mock<IAuthorizationService>();
            providerFeatureToggle.Setup(x => x.IsAuthorized(It.IsAny<string>())).Returns(false);

            _fundingPeriods = new List<TrainingProgrammeFundingPeriod>
            {
                new() { EffectiveFrom = _startFundingPeriod, EffectiveTo = _endFundingPeriod, FundingCap = 1000 },
                new() { EffectiveFrom = _startFundingPeriod.AddMonths(1), EffectiveTo = _endFundingPeriod.AddMonths(1), FundingCap = 500 }
            };
            var trainingProgramme = new TrainingProgramme { EffectiveFrom = DefaultStartDate, EffectiveTo = DefaultStartDate.AddYears(1), FundingPeriods = _fundingPeriods };

            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme(It.Is<string>(c => !string.IsNullOrEmpty(c)), CancellationToken.None))
                .ReturnsAsync(new GetTrainingProgrammeResponse { TrainingProgramme = trainingProgramme });

            CommitmentsApiClient.Setup(x => x.ValidateUlnOverlap(It.IsAny<ValidateUlnOverlapRequest>(), CancellationToken.None))
                .ReturnsAsync(new ValidateUlnOverlapResult { HasOverlappingEndDate = false, HasOverlappingStartDate = false });
            CommitmentsApiClient.Setup(x => x.GetTrainingProgramme("no-course", CancellationToken.None))
                .ThrowsAsync(new RestHttpClientException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    RequestMessage = new HttpRequestMessage(),
                    ReasonPhrase = "Url not found"
                }, "Course not found"));

            _encodingService = new Mock<IEncodingService>();
            SetEncodingOfApprenticeIds();


            _mapper = new DetailsViewModelMapper(CommitmentsApiClient.Object, _encodingService.Object, _pasAccountApiClient.Object, outerApiClient.Object, Mock.Of<ITempDataStorageService>(), providerFeatureToggle.Object);
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
                _encodingService.Setup(x => x.Encode(transferSenderId.Value, EncodingType.PublicAccountId))
                    .Returns(expectedHashedId);
            }

            return this;
        }

        public DetailsViewModelMapperTestsFixture SetPledgeApplicationIdAndItsExpectedHashedValue(int? pledgeApplicationId, string expectedEncodedId)
        {
            CohortDetails.PledgeApplicationId = pledgeApplicationId;
            if (pledgeApplicationId.HasValue)
            {
                _encodingService.Setup(x => x.Encode(pledgeApplicationId.Value, EncodingType.PledgeApplicationId))
                    .Returns(expectedEncodedId);
            }

            return this;
        }

        private DetailsViewModelMapperTestsFixture SetEncodingOfApprenticeIds()
        {
            _encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.ApprenticeshipId))
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
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return this;
            }

            var propertyInfo = draftApprenticeship.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(draftApprenticeship, value, null);
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
            return _mapper.Map(TestHelper.Clone(Source));
        }

        public static void AssertEquality(DraftApprenticeshipDto source, CohortDraftApprenticeshipViewModel result)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(source.Id));
                Assert.That(result.FirstName, Is.EqualTo(source.FirstName));
                Assert.That(result.LastName, Is.EqualTo(source.LastName));
                Assert.That(result.DateOfBirth, Is.EqualTo(source.DateOfBirth));
                Assert.That(result.Cost, Is.EqualTo(source.Cost));
                Assert.That(result.TrainingPrice, Is.EqualTo(source.TrainingPrice));
                Assert.That(result.EndPointAssessmentPrice, Is.EqualTo(source.EndPointAssessmentPrice));
                Assert.That(result.EmploymentPrice, Is.EqualTo(source.EmploymentPrice));
                Assert.That(result.EmploymentEndDate, Is.EqualTo(source.EmploymentEndDate));
                Assert.That(result.StartDate, Is.EqualTo(source.StartDate));
                Assert.That(result.ActualStartDate, Is.EqualTo(source.ActualStartDate));
                Assert.That(result.EndDate, Is.EqualTo(source.EndDate));
                Assert.That(result.DraftApprenticeshipHashedId, Is.EqualTo($"X{source.Id}X"));
                Assert.That(result.IsOnFlexiPaymentPilot, Is.EqualTo(source.IsOnFlexiPaymentPilot));
            });
        }

        public static void AssertSequenceOrder<T>(List<T> expected, List<T> actual, Func<T, T, bool> evaluator)
        {
            Assert.That(actual, Has.Count.EqualTo(expected.Count), "Expected and actual sequences are different lengths");

            for (var i = 0; i < actual.Count; i++)
            {
                Assert.That(evaluator(expected[i], actual[i]), Is.True, "Actual sequence are not in same order as expected");
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
            startDate ??= DefaultStartDate;

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
            CohortDetails.TransferApprovalStatus = transferApprovalStatus;
            ;
            return this;
        }

        public DetailsViewModelMapperTestsFixture SetCohortApprovedStatus(bool isApproved)
        {
            CohortDetails.IsApprovedByEmployer = CohortDetails.IsApprovedByProvider = isApproved;
            ;
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
            _pasAccountApiClient
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