using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice
{
    public class EditApprenticeshipViewModelToValidateApprenticeshipForEditMapperTests
    {
        EditApprenticeshipRequestViewModel request;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();
            fixture.Customize(new DateCustomisation());
            request = fixture.Create<EditApprenticeshipRequestViewModel>();
        }

        [Test, MoqAutoData]
        public async Task ApprenticeshipId_IsMapped(
          EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.ApprenticeshipId, result.ApprenticeshipId);
        }

        [Test, MoqAutoData]
        public async Task FirstName_IsMapped(
            EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.FirstName, result.FirstName);
        }

        [Test, MoqAutoData]
        public async Task LastName_IsMapped(
            EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.LastName, result.LastName);
        }

        [Test, MoqAutoData]
        public async Task Email_IsMapped(
            EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Email, result.Email);
        }

        [Test, MoqAutoData]
        public async Task DateOfBirth_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.DateOfBirth.Date, result.DateOfBirth);
        }

        [Test, MoqAutoData]
        public async Task ULN_IsMapped(
              EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.ULN, result.ULN);
        }

        [Test, MoqAutoData]
        public async Task ProviderId_IsMapped(
              EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.ProviderId, result.ProviderId);
        }

        [Test, MoqAutoData]
        public async Task Cost_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Cost, result.Cost);
        }

        [Test, MoqAutoData]
        public async Task ProviderReference_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.ProviderReference, result.ProviderReference);
        }

        [Test, MoqAutoData]
        public async Task StartDate_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.StartDate.Date, result.StartDate);
        }

        [Test, MoqAutoData]
        public async Task EndDate_IsMapped(
             EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.EndDate.Date, result.EndDate);
        }

        [Test, MoqAutoData]
        public async Task DeliveryModel_IsMapped(
            EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.DeliveryModel, result.DeliveryModel);
        }

        [Test, MoqAutoData]
        public async Task CourseCode_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.CourseCode, result.TrainingCode);
        }

        [Test, MoqAutoData]
        public async Task Version_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Version, result.Version);
        }

        [Test, MoqAutoData]
        public async Task Option_IsMapped(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.Option, result.Option);
        }

        [Test, MoqAutoData]
        public async Task WhenOptionIsTBC_OptionIsMappedToEmptyString(
        EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            request.Option = "TBC";

            var result = await mapper.Map(request);

            Assert.AreEqual(string.Empty, result.Option);
        }

        [Test, MoqAutoData]
        public async Task EmploymentEndDate_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.EmploymentEndDate.Date, result.EmploymentEndDate);
        }

        [Test, MoqAutoData]
        public async Task EmploymentPrice_IsMapped(EditApprenticeshipViewModelToValidateApprenticeshipForEditMapper mapper)
        {
            var result = await mapper.Map(request);

            Assert.AreEqual(request.EmploymentPrice, result.EmploymentPrice);
        }

        public class DateCustomisation : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Customize<EditApprenticeshipRequestViewModel>(composer =>
                    composer.Without(p => p.DateOfBirth)
                            .Without(p => p.EndDate)
                            .Without(p => p.StartDate)
                            .With(p => p.BirthDay, 1)
                            .With(p => p.BirthMonth, 12)
                            .With(p => p.BirthYear, 2000)
                            .With(p => p.StartMonth, 4)
                            .With(p => p.StartYear, 2019)
                            .With(p => p.EndMonth, 6)
                            .With(p => p.EndYear, 2021)
                    );
            }
        }
    }
}
