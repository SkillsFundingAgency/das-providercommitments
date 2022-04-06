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
        ConfirmEditApprenticeshipViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            viewModel = fixture.Build<ConfirmEditApprenticeshipViewModel>()
                 .With(x => x.StartMonth, DateTime.Now.Month)
                 .With(x => x.StartYear, DateTime.Now.Year)
                 .With(x => x.EndMonth, DateTime.Now.Month)
                 .With(x => x.EndYear, DateTime.Now.Year)
                 .With(x => x.EmploymentEndMonth, DateTime.Now.Month)
                 .With(x => x.EmploymentEndYear, DateTime.Now.Year)
                 .With(x => x.BirthMonth, DateTime.Now.Month)
                 .With(x => x.BirthYear, DateTime.Now.Year)
                 .With(x => x.BirthDay, DateTime.Now.Day)
                 .Create();

            mapper = new ConfirmEditApprenticeshipViewModelToEditApiRequestMapper();
        }

        [Test]
        public async Task ApprenticeshipId_IsMapped()
        {
            var result = await mapper.Map(viewModel);
            Assert.AreEqual(viewModel.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test]
        public async Task FirstName_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.FirstName, result.FirstName);
        }

        [Test]
        public async Task LastName_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.LastName, result.LastName);
        }

        [Test]
        public async Task Email_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.Email, result.Email);
        }

        [Test]
        public async Task Dob_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.DateOfBirth, result.DateOfBirth);
        }

        [Test]
        public async Task StartDate_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.StartDate, result.StartDate);
        }


        [Test]
        public async Task EndDate_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.EndDate, result.EndDate);
        }

        [Test, MoqAutoData]
        public async Task DeliveryModel_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.DeliveryModel, result.DeliveryModel);
        }

        [Test, MoqAutoData]
        public async Task EmploymentEndDate_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.EmploymentEndDate, result.EmploymentEndDate);
        }

        [Test, MoqAutoData]
        public async Task EmploymentPrice_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.EmploymentPrice, result.EmploymentPrice);
        }

        [Test]
        public async Task Course_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.CourseCode, result.CourseCode);
        }

        [Test]
        public async Task Cost_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.Cost, result.Cost);
        }

        [Test]
        public async Task Version_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.Version, result.Version);
        }

        [Test, MoqAutoData]
        public async Task Option_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.Option, result.Option);
        }

        [Test]
        public async Task When_OptionIsTBC_Option_IsMapped()
        {
            viewModel.Option = "TBC";
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(string.Empty, result.Option);
        }

        [Test]
        public async Task Reference_IsMapped()
        {
            var result = await mapper.Map(viewModel);

            Assert.AreEqual(viewModel.ProviderReference, result.ProviderReference);
        }
    }
}
