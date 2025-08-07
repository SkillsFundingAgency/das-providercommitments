using SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice.Edit;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.Apprentice;

public class EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapperTests
{
    private EditApprenticeshipRequestViewModel _request;

    [SetUp]
    public void SetUp()
    {
        var fixture = new Fixture();
        fixture.Customize(new DateCustomisation());
        _request = fixture.Create<EditApprenticeshipRequestViewModel>();
    }

    [Test, MoqAutoData]
    public async Task FirstName_IsMapped(EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.FirstName.Should().Be(_request.FirstName);
    }

    [Test, MoqAutoData]
    public async Task LastName_IsMapped(EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.LastName.Should().Be(_request.LastName);
    }

    [Test, MoqAutoData]
    public async Task Email_IsMapped(EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.Email.Should().Be(_request.Email);
    }

    [Test, MoqAutoData]
    public async Task DateOfBirth_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.DateOfBirth.Should().Be(_request.DateOfBirth.Date);
    }

    [Test, MoqAutoData]
    public async Task ULN_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.ULN.Should().Be(_request.ULN);
    }

    [Test, MoqAutoData]
    public async Task Cost_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.Cost.Should().Be(_request.Cost);
    }

    [Test, MoqAutoData]
    public async Task ProviderReference_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.ProviderReference.Should().Be(_request.ProviderReference);
    }

    [Test, MoqAutoData]
    public async Task StartDate_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.StartDate.Should().Be(_request.StartDate.Date);
    }

    [Test, MoqAutoData]
    public async Task EndDate_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.EndDate.Should().Be(_request.EndDate.Date);
    }

    [Test, MoqAutoData]
    public async Task DeliveryModel_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.DeliveryModel.Should().Be((int)_request.DeliveryModel);
    }

    [Test, MoqAutoData]
    public async Task CourseCode_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.CourseCode.Should().Be(_request.CourseCode);
    }

    [Test, MoqAutoData]
    public async Task Version_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.Version.Should().Be(_request.Version);
    }

    [Test, MoqAutoData]
    public async Task Option_IsMapped(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.Option.Should().Be(_request.Option);
    }

    [Test, MoqAutoData]
    public async Task WhenOptionIsTBC_OptionIsMappedToEmptyString(
        EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        _request.Option = "TBC";

        var result = await mapper.Map(_request);

        result.Option.Should().Be(string.Empty);
    }

    [Test, MoqAutoData]
    public async Task EmploymentEndDate_IsMapped(EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.EmploymentEndDate.Should().Be(_request.EmploymentEndDate.Date);
    }

    [Test, MoqAutoData]
    public async Task EmploymentPrice_IsMapped(EditApprenticeshipViewModelToValidateEditApprenticeshipRequestMapper mapper)
    {
        var result = await mapper.Map(_request);

        result.EmploymentPrice.Should().Be(_request.EmploymentPrice);
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