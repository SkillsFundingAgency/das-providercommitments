﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Shared.Models;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using SFA.DAS.ProviderCommitments.Web.Validators.Apprentice;
using System;
using System.Linq.Expressions;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Apprentice
{
    [TestFixture]
    public class DatesViewModelValidatorTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        public void ThenProviderIdIsValidated(long providerId, bool expectedValid)
        {
            var request = new DatesViewModel { ProviderId = providerId };

            AssertValidationResult(x => x.ProviderId, request, expectedValid);
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("XYZ", true)]
        public void ThenEmployerAccountLegalEntityPublicHashedIdIsValidated(string employerAccountLegalEntityPublicHashedId, bool expectedValid)
        {
            var model = new DatesViewModel { EmployerAccountLegalEntityPublicHashedId = employerAccountLegalEntityPublicHashedId };

            AssertValidationResult(request => request.EmployerAccountLegalEntityPublicHashedId, model, expectedValid);
        }

        [TestCase(0, false)]
        [TestCase(102, true)]
        public void ThenAccountLegalEntityIdIsValidated(long accountLegalEntityId, bool expectedValid)
        {
            var model = new DatesViewModel { AccountLegalEntityId = accountLegalEntityId };
            AssertValidationResult(request => request.AccountLegalEntityId, model, expectedValid);
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("AB76V", true)]
        public void ThenApprenticeshipHashedIdIsValidated(string apprenticeshipHashedId, bool expectedValid)
        {
            var model = new DatesViewModel { ApprenticeshipHashedId = apprenticeshipHashedId };
            AssertValidationResult(request => request.ApprenticeshipHashedId, model, expectedValid);
        }

        [Test]
        public void AndStopDateIsValid_ThenShouldNotHaveError()
        {
            DateTime? date = new DateTime(2020, 1, 1);
            var model = new DatesViewModel { StopDate = date };
            AssertValidationResult(request => request.StopDate, model, true);
        }

        [Test]
        public void AndStopDateIsNull_ThenShouldHaveError()
        {
            var model = new DatesViewModel { StopDate = null };
            AssertValidationResult(request => request.StopDate, model, false);
        }


        [Test]
        public void AndStartDateIsValid_ThenShouldNotHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2020;
            var model = new DatesViewModel {StartDate = startDate, StopDate = stopDate};
            AssertValidationResult(request => request.StartDate, model, true);
        }

        [Test]
        public void AndStartDateIsEmpty_ThenShouldHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            var model = new DatesViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [TestCase(2020, 15)]
        [TestCase(2020, null)]
        [TestCase(0, 12)]
        [TestCase(null, 12)]
        public void AndStartDateIsInvalid_ThenShouldHaveError(int? year, int? month)
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = month;
            startDate.Year = year;
            var model = new DatesViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        [Test]
        public void AndStartDateIsBeforeStopDate_ThenShouldHaveError()
        {
            DateTime? stopDate = new DateTime(2019, 1, 1);
            MonthYearModel startDate = new MonthYearModel("");
            startDate.Month = 1;
            startDate.Year = 2018;
            var model = new DatesViewModel { StartDate = startDate, StopDate = stopDate };
            AssertValidationResult(request => request.StartDate, model, false);
        }

        private void AssertValidationResult<T>(Expression<Func<DatesViewModel, T>> property, DatesViewModel instance, bool expectedValid)
        {
            var validator = new DatesViewModelValidator();

            if (expectedValid)
            {
                validator.ShouldNotHaveValidationErrorFor(property, instance);
            }
            else
            {
                validator.ShouldHaveValidationErrorFor(property, instance);
            }
        }
    }
}