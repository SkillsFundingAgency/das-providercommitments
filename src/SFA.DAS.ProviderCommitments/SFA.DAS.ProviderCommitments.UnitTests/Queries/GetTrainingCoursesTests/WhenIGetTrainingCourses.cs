using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderCommitments.Domain_Models.ApprenticeshipCourse;
using SFA.DAS.ProviderCommitments.Interfaces;
using SFA.DAS.ProviderCommitments.Queries.GetTrainingCourses;

namespace SFA.DAS.ProviderCommitments.UnitTests.Queries.GetTrainingCoursesTests
{
    [TestFixture]
    public class WhenIGetTrainingCourses
    {
        private GetTrainingCoursesQueryHandler _handler;

        private Mock<ITrainingProgrammeApiClient> _trainingProgrammeApiClientMock;

        [SetUp]
        public void Arrange()
        {
            _trainingProgrammeApiClientMock = new Mock<ITrainingProgrammeApiClient>();

            _trainingProgrammeApiClientMock
                .Setup(x => x.GetTrainingProgrammes())
                .ReturnsAsync(new List<ITrainingProgramme>
                {
                    new Framework
                    {
                        EffectiveFrom = new DateTime(2017,01,01),
                        EffectiveTo = new DateTime(2017,12,31)
                    },
                    new Standard
                    {
                        EffectiveFrom = new DateTime(2016,01,01),
                        EffectiveTo = new DateTime(2016,12,31)
                    }
                    });

            _handler = new GetTrainingCoursesQueryHandler(_trainingProgrammeApiClientMock.Object);
        }

        [Test]
        public async Task ThenOnlyStandardsAreReturnedIfIExcludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = false,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.IsTrue(result.TrainingProgrammes.Any(x => x.ProgrammeType == ProgrammeType.Standard));
            Assert.IsFalse(result.TrainingProgrammes.Any(x => x.ProgrammeType == ProgrammeType.Framework));
        }

        [Test]
        public async Task ThenStandardsAndFrameworksAreReturnedIfIIncludeFrameworks()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.IsTrue(result.TrainingProgrammes.Any(x => x.ProgrammeType == ProgrammeType.Standard));
            Assert.IsTrue(result.TrainingProgrammes.Any(x => x.ProgrammeType == ProgrammeType.Framework));
        }

        [Test]
        public async Task ThenIfISpecifyAnEffectiveDateIOnlyGetCoursesActiveOnThatDay()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = new DateTime(2016, 06, 01)
            }, new CancellationToken());

            Assert.AreEqual(1, result.TrainingProgrammes.Length);
            Assert.IsInstanceOf<Standard>(result.TrainingProgrammes[0]);
        }

        [Test]
        public async Task ThenIfIDoNotSpecifyAnEffectiveDateIGetAllCourses()
        {
            var result = await _handler.Handle(new GetTrainingCoursesQueryRequest
            {
                IncludeFrameworks = true,
                EffectiveDate = null
            }, new CancellationToken());

            Assert.AreEqual(2, result.TrainingProgrammes.Length);
        }
    }
}