using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ProviderCommitments.Configuration;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using SFA.DAS.ProviderCommitments.Web.Validators.FileUpload;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Validators.Cohort
{
    [TestFixture]
    public class FileUploadStartViewModelValidatorTests
    {
        private const string HeaderLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy";
        private const string RplExtendedHeaderLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,TrainingTotalHours,TrainingHoursReduction,IsDurationReducedByRPL,DurationReducedBy,PriceReducedBy";

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
        public async Task ShouldReturnInvalidMessageWhenFileSizeIsMoreThan50kb()
        {
            _file.Setup(m => m.Length).Returns(51 * 1024); //51kb

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenFileTypeIsNotCsv()
        {
            _file.Setup(m => m.FileName).Returns("APPDATA-20051030-213855.pdf");

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenIsEmptyFileContent()
        {
            const string fileContents = "";
            var textStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContents));
            _file.Setup(m => m.OpenReadStream()).Returns(textStream);

            var model = new FileUploadStartViewModel { Attachment = _file.Object };
            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenColumnCountIsNot13()
        {
            const string headerLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,InvalidColumn";

            var file = CreateFakeFormFile(headerLine);
            var model = new FileUploadStartViewModel { Attachment = file };

            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenColumnCountIsLessThan13()
        {
            const string headerLine = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID";

            var file = CreateFakeFormFile(headerLine);
            var model = new FileUploadStartViewModel { Attachment = file };

            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [TestCase(
@"CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef
1,2,3,4,5,6,7,8,9,10,11,12")]
        [TestCase(
@"CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef
1,2,3,4,5,6,7,8,9,10,11,12,13
1,2,3,4,5,6,7,8,9,10,11,12")]
        [TestCase(
@"CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy
1,2,3,4,5,6,7,8,9,10,11,12,13,14")]
        [TestCase(
@"CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16
1,2,3,4,5,6,7,8,9,10,11,12,13,14,15")]
        public async Task ShouldReturnInvalidMessageWhenAnyRowsColumnCountIsWrong(string fileContents)
        {
            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };
            await AssertValidationResult(vm => vm.Attachment, model, false);
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenFileRowCountIsInvalid()
        {
            var builder = new StringBuilder();
            builder.AppendLine(HeaderLine);
            for (var i = 1; i < 101; i++)
            {
                builder.AppendLine(HeaderLine);
            }
            var fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };

            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);
            var result = await validator.ValidateAsync(model);

            Assert.That(result.IsValid, Is.False);
        }


        [Test]
        public async Task ShouldBeValidWhenFileIsValid()
        {
            var builder = new StringBuilder();
            builder.AppendLine(HeaderLine);
            for (var i = 1; i < 99; i++)
            {
                builder.AppendLine(HeaderLine);
            }
            var fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);

            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);
            var result = await validator.ValidateAsync(model);

            Assert.That(result.IsValid, Is.True);
        }


        [Test]
        public async Task ShouldCheckRplExtendedColumns()
        {
            var builder = new StringBuilder();
            builder.AppendLine(RplExtendedHeaderLine);
            for (var i = 1; i < 99; i++)
            {
                builder.AppendLine(RplExtendedHeaderLine);
            }
            var fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);

            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);
            var result = await validator.ValidateAsync(model);

            Assert.That(result.IsValid, Is.True);
        }

        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,XRecognisePriorLearning,TrainingTotalHours,TrainingHoursReduction,IsDurationReducedByRPL,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,XTrainingTotalHours,TrainingHoursReduction,IsDurationReducedByRPL,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,TrainingTotalHours,XTrainingHoursReduction,IsDurationReducedByRPL,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,TrainingTotalHours,TrainingHoursReduction,XIsDurationReducedByRPL,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,TrainingTotalHours,TrainingHoursReduction,IsDurationReducedByRPL,XDurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,TrainingTotalHours,TrainingHoursReduction,IsDurationReducedByRPL,DurationReducedBy,XPriceReducedBy")]
        public async Task ShouldCheckCorruptRplColumns(string headerLine)
        {
            var builder = new StringBuilder();
            builder.AppendLine(headerLine);
            for (var index = 1; index < 99; index++)
            {
                builder.AppendLine(headerLine);
            }
            var fileContents = builder.ToString();

            var file = CreateFakeFormFile(fileContents);

            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);
            var result = await validator.ValidateAsync(model);

            Assert.That(result.IsValid, Is.False);
        }

        [TestCase("CohortReff,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgrementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,VLN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyNam,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenName,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenName,BirthDate,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,Email,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,Std,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,Start,End,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,Start,End,Price,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAO,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,Provider,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        [TestCase("CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,Provider,,,,RecognisePriorLearning,DurationReducedBy,PriceReducedBy")]
        public async Task ShouldReturnInvalidMessageWhenColumnHeaderIsNotMatchedWithTemplate(string header)
        {
            //Arrange            
            var fileContents = new StringBuilder().AppendLine(header).ToString();
            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);

            //Act
            var result = await validator.ValidateAsync(model);

            Assert.Multiple(() =>
            {
                //Assert
                Assert.That(result.Errors, Has.Count.EqualTo(1));
                Assert.That(result.ToString(), Is.EqualTo("One or more Field Names in the header row are invalid. You need to refer to the template or specification to correct this"));
            });
        }

        [Test]
        public async Task ShouldReturnInvalidMessageWhenFileContainsOnlyColumnHeader()
        {
            //Arrange
            var fileContents = new StringBuilder().AppendLine(HeaderLine).ToString();
            var file = CreateFakeFormFile(fileContents);
            var model = new FileUploadStartViewModel { Attachment = file };
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);

            //Act
            var result = await validator.ValidateAsync(model);

            Assert.Multiple(() =>
            {
                //Assert
                Assert.That(result.Errors, Has.Count.EqualTo(1));
                Assert.That(result.ToString(), Is.EqualTo("The selected file does not contain apprentice details"));
            });
        }

        private async Task AssertValidationResult<T>(Expression<Func<FileUploadStartViewModel, T>> property, FileUploadStartViewModel instance, bool expectedValid)
        {
            var validator = new FileUploadStartViewModelValidator(_csvConfiguration);

            var validationResult = await validator.TestValidateAsync(instance);
            if (expectedValid)
            {
                validationResult.ShouldNotHaveValidationErrorFor(property);
            }
            else
            {
                validationResult.ShouldHaveValidationErrorFor(property);
            }
        }

        private static IFormFile CreateFakeFormFile(string fileContents)
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