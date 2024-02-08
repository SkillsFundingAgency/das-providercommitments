using System;
using FluentValidation.Results;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Application.Commands.CreateCohort;

namespace SFA.DAS.ProviderCommitments.UnitTests.Commands.CreateCohort;

[TestFixture]
public class WhenCreateCohortRequestIsValidated
{
    private CreateCohortValidator _validator;
    private CreateCohortRequest _validRequest;
    private Func<ValidationResult> _act;

    [SetUp]
    public void Arrange()
    {
        _validRequest = new CreateCohortRequest
        {
            ProviderId = 123,
            AccountLegalEntityId = 456,
            ReservationId = Guid.NewGuid()
        };

        _validator = new CreateCohortValidator();
        _act = () =>_validator.Validate(_validRequest);
    }

    [Test]
    public void ThenAValidRequestValidatesSuccessfully()
    {
        var result = _act();
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void ThenProviderIdIsRequired()
    {
        _validRequest.ProviderId = 0;
        var result = _act();
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public void ThenReservationIdIsRequired()
    {
        _validRequest.ReservationId = Guid.Empty;
        var result = _act();
        Assert.That(result.IsValid, Is.False);
    }


    [Test]
    public void ThenEmployerAccountIdIsRequired()
    {
        _validRequest.AccountLegalEntityId = 0;
        var result = _act();
        Assert.That(result.IsValid, Is.False);
    }
}