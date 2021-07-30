using AutoFixture;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class ConfirmEditApprenticehsipViewModelToEditApiRequestMapperTests
    {
        private ConfirmEditApprenticeshipViewModelToEditApiRequestMapper mapper;
        ConfirmEditApprenticeshipViewModel request;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            request = fixture.Build<ConfirmEditApprenticeshipViewModel>()
                 .With(x => x.StartMonth, DateTime.Now.Month)
                 .With(x => x.StartYear, DateTime.Now.Year)
                 .With(x => x.EndMonth, DateTime.Now.Month)
                 .With(x => x.EndYear, DateTime.Now.Year)
                 .With(x => x.BirthMonth, DateTime.Now.Month)
                 .With(x => x.BirthYear, DateTime.Now.Year)
                 .With(x => x.BirthDay, DateTime.Now.Day)
                 .Create();

            mapper = new ConfirmEditApprenticeshipViewModelToEditApiRequestMapper();
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            var result = await mapper.Map(request);
            Assert.AreEqual(request.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.FirstName, result.FirstName);
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.LastName, result.LastName);
        }

        [Test]
        public async Task Email_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Email, result.Email);
        }

        [Test]
        public async Task Dob_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.DateOfBirth, result.DateOfBirth);
        }

        [Test]
        public async Task StartDate_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.StartDate, result.StartDate);
        }


        [Test]
        public async Task EndDate_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.EndDate, result.EndDate);
        }

        [Test]
        public async Task Course_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task Cost_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Cost, result.Cost);
        }

        [Test]
        public async Task Reference_IsMapped()
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.ProviderReference, result.ProviderReference);
        }
    }
}
