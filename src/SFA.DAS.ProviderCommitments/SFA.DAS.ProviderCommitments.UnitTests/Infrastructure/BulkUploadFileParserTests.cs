using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.IO;
using System.Linq;
using FluentAssertions.Execution;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure;

[TestFixture]
public class BulkUploadFileParserTests
{
    private const string Headers = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy,TrainingTotalHours,IsDurationReducedByRPL,TrainingHoursReduction";

    private BulkUploadFileParser _bulkUploadFileParser;
    private string _fileContent;
    private long _providerId;
    private IFormFile _file;

    [SetUp]
    public void Setup()
    {
        _providerId = 1;
        _bulkUploadFileParser = new BulkUploadFileParser(Mock.Of<ILogger<BulkUploadFileParser>>());
        _fileContent = Headers + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99,1000,TRUE,100" + Environment.NewLine +
                       "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,false,,,,,";

        const string fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(_fileContent);
        writer.Flush();
        stream.Position = 0;

        _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
    }

    [Test]
    public void CohortRefParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().CohortRef.Should().BeEquivalentTo("P9DD4P");
            result.Last().CohortRef.Should().Be("P9DD4P");
        }
    }

    [Test]
    public void AgreementIdParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().AgreementId.Should().Be("XEGE5X");
            result.Last().AgreementId.Should().Be("XEGE5X");
        }
    }

    [Test]
    public void ULNParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().ULN.Should().Be("8652496047");
            result.Last().ULN.Should().Be("6347198567");
        }
    }

    [Test]
    public void FamilyNameParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().FamilyName.Should().Be("Jones");
            result.Last().FamilyName.Should().Be("Smith");
        }
    }

    [Test]
    public void GivenNamesParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().GivenNames.Should().Be("Louise");
            result.Last().GivenNames.Should().Be("Mark");
        }
    }

    [Test]
    public void DateOfBirthParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().DateOfBirth.Should().Be("2000-01-01");
            result.Last().DateOfBirth.Should().Be("2002-02-02");
        }
    }

    [Test]
    public void EmailAddressParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().EmailAddress.Should().Be("abc1@abc.com");
            result.Last().EmailAddress.Should().Be("abc2@abc.com");
        }
    }

    [Test]
    public void StdCodeParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.Last().StdCode.Should().Be("58");
            result.First().StdCode.Should().Be("57");
        }
    }

    [Test]
    public void StartDateParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().StartDate.Should().Be("2017-05-03");
            result.Last().StartDate.Should().Be("2018-06-01");
        }
    }

    [Test]
    public void EndDateParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().EndDate.Should().Be("2018-05");
            result.Last().EndDate.Should().Be("2019-06");
        }
    }

    [Test]
    public void TotalPriceParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().TotalPrice.Should().Be("2000");
            result.Last().TotalPrice.Should().Be("3333");
        }
    }

    [Test]
    public void ProviderRefIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().ProviderRef.Should().Be("CX768");
            result.Last().ProviderRef.Should().Be("ZB657");
        }
    }

    [Test]
    public void RecognisePriorLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().RecognisePriorLearning.Should().Be("true");
            result.Last().RecognisePriorLearning.Should().Be("false");
        }
    }

    [Test]
    public void DurationReducedByLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().DurationReducedBy.Should().Be("12");
            result.Last().DurationReducedBy.Should().Be("");
        }
    }

    [Test]
    public void PriceReducedByLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().PriceReducedBy.Should().Be("99");
            result.Last().PriceReducedBy.Should().Be("");
        }
    }

    [Test]
    public void TrainingTotalHoursIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().TrainingTotalHours.Should().Be("1000");
            result.Last().TrainingTotalHours.Should().Be("");
        }
    }

    [Test]
    public void IsDurationReducedByRPLIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().IsDurationReducedByRPL.Should().Be("TRUE");
            result.Last().IsDurationReducedByRPL.Should().Be("");
        }
    }

    [Test]
    public void TrainingHoursReductionIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        using (new AssertionScope())
        {
            result.First().TrainingHoursReduction.Should().Be("100");
            result.Last().TrainingHoursReduction.Should().Be("");
        }
    }

    [Test]
    public void CorrectNumberOfApprenticeshipMapped()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        result.Count.Should().Be(2);
    }

    [Test]
    public void VerifyEmptyRowsRemovedFromUploadedFile()
    {
        //Arrange          
        _fileContent = Headers + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,," + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,,,,,," + Environment.NewLine +
                       "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,,,,,," + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,,";

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        result.Count.Should().Be(2);
    }

    [Test]
    public void VerifyNoRowsReturnedFromEmptyFile()
    {
        //Arrange          
        _fileContent = Headers + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,," + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,,";

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        result.Should().BeEmpty();
    }

    [Test]
    public void OptionalFieldsMayBeOmitted()
    {
        //Arrange          
        _fileContent = Headers.Replace(",TrainingTotalHours,IsDurationReducedByRPL,TrainingHoursReduction", "") + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine;

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        result.Should().ContainEquivalentOf(new
        {
            CohortRef = "P9DD4P",
            TrainingTotalHours = (string)null,
            IsDurationReducedByRPL = (bool?)null,
            TrainingHoursReduction = (string)null,
        });
    }

    [Test]
    public void OptionalFieldsWithoutHeaderName()
    {
        //Arrange          
        _fileContent = Headers.Replace(",TrainingTotalHours,IsDurationReducedByRPL,TrainingHoursReduction", ",,,") + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99,9000,TRUE,90" + Environment.NewLine;

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_providerId, _file);
        result.Should().ContainEquivalentOf(new
        {
            CohortRef = "P9DD4P",
            TrainingTotalHours = (string)null,
            IsDurationReducedByRPL = (bool?)null,
            TrainingHoursReduction = (string)null,
        });
    }

    private void CreateFile()
    {
        const string fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(_fileContent);
        writer.Flush();
        stream.Position = 0;

        _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
    }
}