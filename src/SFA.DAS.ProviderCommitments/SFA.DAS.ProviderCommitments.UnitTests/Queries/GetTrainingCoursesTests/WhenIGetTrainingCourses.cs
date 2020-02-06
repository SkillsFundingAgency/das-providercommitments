using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetTrainingCoursesTests
{
    [TestFixture]
    public class WhenIGetTrainingCourses
    {
        private GetTrainingCoursesQueryHandler _handler;

        private Mock<ITrainingProgrammeApiClient> _iTrainingProgrammeApiClient;

        private Standard[] _standards;
        private Framework[] _frameworks;
        [SetUp]
        public void Arrange()
        {

            var standard = new Standard
            {
                EffectiveFrom = new DateTime(2016, 01, 01),
                EffectiveTo = new DateTime(2016, 12, 31),
                Title = "Title"
            };

            var framework = new Framework
            {
                EffectiveFrom = new DateTime(2017, 01, 01),
                EffectiveTo = new DateTime(2017, 12, 31),
                Title = "Title",
                FrameworkName = "Name",
                PathwayName = "Name"
            };

            _standards = new Standard[1] {standard};
            _frameworks = new Framework[1] {framework};

            _iTrainingProgrammeApiClient = new Mock<ITrainingProgrammeApiClient>();
            
            _iTrainingProgrammeApiClient.Setup(x => x.GetFrameworkTrainingProgrammes())
                .ReturnsAsync(new List<Framework>
                {
                    framework
                });

            _iTrainingProgrammeApiClient.Setup(x => x.GetStandardTrainingProgrammes())
                .ReturnsAsync(new List<Standard>
                {
                    standard
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

            Assert.AreEqual(_standards.Length, result.TrainingCourses.Length);
            Assert.IsInstanceOf<Standard>(result.TrainingCourses[0]);
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(_standards.Length + _frameworks.Length, result.TrainingCourses.Length);
            Assert.IsTrue(result.TrainingCourses.Any(x => x is Standard));
            Assert.IsTrue(result.TrainingCourses.Any(x => x is Framework));
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
            Assert.IsInstanceOf<Standard>(result.TrainingCourses[0]);
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