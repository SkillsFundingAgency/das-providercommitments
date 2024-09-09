using System;
using System.Linq.Expressions;
using FluentValidation.TestHelper;
using SFA.DAS.ProviderCommitments.Web.Models.OveralppingTrainingDate;
using SFA.DAS.ProviderCommitments.Web.Validators.OverlappingTrainingDate;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.OverlappingTrainingDates;

public class OverlapOptionsForChangeEmployerRequestValidatorTests
{
    [TestCase(0, false)]
    [TestCase(1, true)]
    public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
    {
        var request = new OverlapOptionsForChangeEmployerRequest { ProviderId = providerId };

        AssertValidationResult(x => x.ProviderId, request, expectedValid);
    }

    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase("AB76V", true)]
    public void Validate_ApprenticeshipHashedId_ShouldBeValidated(string apprenticeshipHashedId, bool expectedValid)
    {
        var model = new OverlapOptionsForChangeEmployerRequest { ApprenticeshipHashedId = apprenticeshipHashedId };
        AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
    }

    [TestCase(null, false)]
    [TestCase("00000000-0000-0000-0000-000000000000", false)]
    [TestCase("XYZ", false)]
    [TestCase("12345678-90ab-cdef-0123-456789abcdef", true)]
    public void Validate_CacheKey_ShouldBeValidated(string cacheKey, bool expectedValid)
    {
        if (cacheKey == null)
        {
            expectedValid.Should().BeFalse();
        }

        var isGuid = Guid.TryParse(cacheKey, out var key);
        if (isGuid)
        {
            var model = new OverlapOptionsForChangeEmployerRequest { CacheKey = key };
            AssertValidationResult(request => request.CacheKey, model, expectedValid);
        }
        else
        {
            expectedValid.Should().BeFalse();
        }
    }

    private static void AssertValidationResult<T>(Expression<Func<OverlapOptionsForChangeEmployerRequest, T>> property,
        OverlapOptionsForChangeEmployerRequest instance, bool expectedValid)
    {
        var validator = new OverlapOptionsForChangeEmployerRequestValidator();
        var result = validator.TestValidate(instance);

        if (expectedValid)
        {
            result.ShouldNotHaveValidationErrorFor(property);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(property);
        }
    }
}