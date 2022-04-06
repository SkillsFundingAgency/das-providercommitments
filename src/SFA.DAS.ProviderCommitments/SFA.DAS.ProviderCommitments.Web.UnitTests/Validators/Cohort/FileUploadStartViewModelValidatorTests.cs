using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FluentValidation.TestHelper;
using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using SFA.DAS.ProviderCommitments.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class FileUploadStartViewModelValidatorTests
    {
        private Mock<IFormFile> _file;
        private BulkUploadFileValidationConfiguration _csvConfiguration;

        [SetUp]
        public void SetUp()
        {
            _file = new Mock<IFormFile>();
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.csv");
            _csvConfiguration = new BulkUploadFileValidationConfiguration
            {
                MaxBulkUploadFileSize = 50,
                AllowedFileColumnCount = 13,
                MaxAllowedFileRowCount = 100
            };
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenFileSizeIsMoreThan50kb()
        {
            _file.Setup(m => m.Length).Returns(51 * 1024); //51kb

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenFileTypeIsNotCsv()
        {
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.pdf");

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenIsEmptyFileContent()
        {
            const string fileContents = "";
            var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents));
            _file.Setup(m => m.OpenReadStream()).Returns(textStream);

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenColumnCountIsNot13()
        {
            const string HeaderLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,InvalidColumn";

            const string fileContents = HeaderLine;
            var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents));
            _file.Setup(m => m.OpenReadStream()).Returns(textStream);

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenColumnCountIsLessThan13()
        {
            const string HeaderLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID";

            const string fileContents = HeaderLine;
            var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents));
            _file.Setup(m => m.OpenReadStream()).Returns(textStream);

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public void ShouldReturnInvalidMessageWhenFileRowCountIsInvalid()
        {
            const string headerLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef";

            var builder = new StringBuilder();
            builder.AppendLine(headerLine);
            for (int i = 1; i < 101; i++)
            {
                builder.AppendLine(headerLine);
            }
            string fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };

            var validator = new FileUploadStartViewModelValidator(Mock.Of<ILogger<FileUploadStartViewModelValidator>>(), _csvConfiguration);
            var result = validator.Validate(model);

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ShouldBeValidWhenFileIsValid()
        {
            const string headerLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef";

            var builder = new StringBuilder();
            builder.AppendLine(headerLine);
            for (int i = 1; i < 99; i++)
            {
                builder.AppendLine(headerLine);
            }
            string fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);

            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(Mock.Of<ILogger<FileUploadStartViewModelValidator>>(), _csvConfiguration);
            var result = validator.Validate(model);

            Assert.IsTrue(result.IsValid);
        }

        [TestCase("CohortReff,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgrementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,VLN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyNam,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenName,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenName,BirthDate,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,Email,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,Std,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,Start,End,TotalPrice,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,Start,End,Price,EPAOrgID,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAO,ProviderRef")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,Provider")]
        public void ShouldReturnInvalidMessageWhenColumnHeaderIsNotMatchedWithTemplate(string header)
        {
            //Arrange            
            var fileContents = new StringBuilder().AppendLine(header).ToString();
            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(Mock.Of<ILogger<FileUploadStartViewModelValidator>>(), _csvConfiguration);

            //Act
            var result = validator.Validate(model);

            //Assert
            Assert.AreEqual(1,result.Errors.Count);
            Assert.AreEqual("One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this", result.ToString());
        }

        private void AssertValidationResult<T>(Expression<Func<FileUploadStartViewModel, T>> property, FileUploadStartViewModel instance, bool expectedValid)
        {
            var validator = new FileUploadStartViewModelValidator(Mock.Of<ILogger<FileUploadStartViewModelValidator>>(), _csvConfiguration);

            var validationResult = validator.TestValidate(instance);
            if (expectedValid)
            {
                validationResult.ShouldNotHaveValidationErrorFor(property);
            }
            else
            {
                validationResult.ShouldHaveValidationErrorFor(property);
            }
        }

        private IFormFile CreateFakeFormFile(string fileContents)
        {
            var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents));
            var file = new FormFile(textStream, 0, textStream.Length, "APPDATA-20051030-213855", "APPDATA-20051030-213855.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };

            return file;
        }
    }
}