using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetTrainingCoursesTests
{
    [TestFixture]
    public class WhenIGetTrainingCourses
    {
        private GetTrainingCoursesQueryHandler _handler;

        private Mock<ICommitmentsApiClient> _iTrainingProgrammeApiClient;

        private List<TrainingProgramme> _standards;
        private List<TrainingProgramme> _allTrainingProgrammes;
        [SetUp]
        public void Arrange()
        {
            _standards = new List<TrainingProgramme>();
            _allTrainingProgrammes = new List<TrainingProgramme>();
    
            var standard = new TrainingProgramme
            {
                EffectiveFrom = new DateTime(2016, 01, 01),
                EffectiveTo = new DateTime(2016, 12, 31),
                Name = "Title"
            };

            var framework = new TrainingProgramme
            {
                EffectiveFrom = new DateTime(2017, 01, 01),
                EffectiveTo = new DateTime(2017, 12, 31),
                Name = "Title"
            };

            _standards.Add(standard);
            _allTrainingProgrammes = new List<TrainingProgramme> {standard, framework};

            _iTrainingProgrammeApiClient = new Mock<ICommitmentsApiClient>();
            
            _iTrainingProgrammeApiClient.Setup(x => x.GetAllTrainingProgrammeStandards(CancellationToken.None))
                .ReturnsAsync(new GetAllTrainingProgrammeStandardsResponse
                {
                    TrainingProgrammes = _standards
                });

            _iTrainingProgrammeApiClient.Setup(x => x.GetAllTrainingProgrammes(CancellationToken.None))
                .ReturnsAsync(new GetAllTrainingProgrammesResponse()
                {
                    TrainingProgrammes = _allTrainingProgrammes
                });

            _handler = new GetTrainingCoursesQueryHandler(_iTrainingProgrammeApiClient.Object);
        }

        [Test]
        public async Task ThenOnlyStandardsAreReturnedIfIExcludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = false,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(_standards.Count, result.TrainingCourses.Length);
            result.TrainingCourses[0].Should().BeEquivalentTo(_standards[0]);
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            result.TrainingCourses.Should().BeEquivalentTo(_allTrainingProgrammes);
        }

        [Test]
        public async Task ThenIfISpecifyAnEffectiveDateIOnlyGetCoursesActiveOnThatDay()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = new DateTime(2016, 06, 01)
            }, new CancellationToken());

            Assert.AreEqual(1, result.TrainingCourses.Length);
            result.TrainingCourses[0].Should().BeEquivalentTo(_standards[0]);
        }

        [Test]
        public async Task ThenIfIDoNotSpecifyAnEffectiveDateIGetAllCourses()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(2, result.TrainingCourses.Length);
        }
    }
}