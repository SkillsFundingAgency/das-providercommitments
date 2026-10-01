using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.ErrorHandling;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure.OuterApi.ErrorHandling
{
    [TestFixture]
    public class BulkUploadDomainExceptionMapperTests
    {
        [Test]
        public void ToBulkUploadValidationErrors_Maps_Field_And_Message_To_File_Level_Error()
        {
            var exception = new CommitmentsApiModelException(
            [
                new ErrorDetail("TrainingTotalHours", "You must enter the total off-the-job training time"),
                new ErrorDetail("PriceReducedBy", "Enter the total price reduction due to RPL")
            ]);

            var result = BulkUploadDomainExceptionMapper.ToBulkUploadValidationErrors(exception);

            result.Should().HaveCount(1);
            result[0].RowNumber.Should().Be(0);
            result[0].Errors.Should().BeEquivalentTo(new List<Error>
            {
                new("TrainingTotalHours", "You must enter the total off-the-job training time"),
                new("PriceReducedBy", "Enter the total price reduction due to RPL")
            });
        }

        [Test]
        public void ToFieldMessageLog_Joins_Field_And_Message()
        {
            var exception = new CommitmentsApiModelException(
            [
                new ErrorDetail("TrainingTotalHours", "You must enter the total off-the-job training time"),
                new ErrorDetail("PriceReducedBy", "Enter the total price reduction due to RPL")
            ]);

            var result = BulkUploadDomainExceptionMapper.ToFieldMessageLog(exception);

            result.Should().Be("TrainingTotalHours: You must enter the total off-the-job training time; PriceReducedBy: Enter the total price reduction due to RPL");
        }
    }
}
