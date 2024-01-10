using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Web.Models.Cohort;
using System;
using System.IO;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.UnitTests.Infrastructure;

[TestFixture]
public class BulkUploadFileParserTests
{
    private const string Headers = "CohortRef,AgreementID,ULN,FamilyName,GivenNames,DateOfBirth,EmailAddress,StdCode,StartDate,EndDate,TotalPrice,EPAOrgID,ProviderRef,RecognisePriorLearning,DurationReducedBy,PriceReducedBy,TrainingTotalHours,IsDurationReducedByRPL,TrainingHoursReduction";

    BulkUploadFileParser _bulkUploadFileParser;
    string _fileContent;
    long _proivderId;
    IFormFile _file;

    [SetUp]
    public void Setup()
    {
        _proivderId = 1;
        _bulkUploadFileParser = new BulkUploadFileParser(Mock.Of<ILogger<BulkUploadFileParser>>());
        _fileContent = Headers + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99,1000,TRUE,100" + Environment.NewLine +
                       "P9DD4P,XEGE5X,6347198567,Smith,Mark,2002-02-02,abc2@abc.com,58,2018-06-01,2019-06,3333,EPA0001,ZB657,false,,,,,";

        var fileName = "test.pdf";
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
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().CohortRef, Is.EqualTo("P9DD4P"));
        Assert.That(result.Last().CohortRef, Is.EqualTo("P9DD4P"));
    }

    [Test]
    public void AgreementIdParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().AgreementId, Is.EqualTo("XEGE5X"));
        Assert.That(result.Last().AgreementId, Is.EqualTo("XEGE5X"));
    }

    [Test]
    public void ULNParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().ULN, Is.EqualTo("8652496047"));
        Assert.That(result.Last().ULN, Is.EqualTo("6347198567"));
    }

    [Test]
    public void FamilyNameParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().FamilyName, Is.EqualTo("Jones"));
        Assert.That(result.Last().FamilyName, Is.EqualTo("Smith"));
    }

    [Test]
    public void GivenNamesParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().GivenNames, Is.EqualTo("Louise"));
        Assert.That(result.Last().GivenNames, Is.EqualTo("Mark"));
    }


    [Test]
    public void DateOfBirthParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().DateOfBirth, Is.EqualTo("2000-01-01"));
        Assert.That(result.Last().DateOfBirth, Is.EqualTo("2002-02-02"));
    }

    [Test]
    public void EmailAddressParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().EmailAddress, Is.EqualTo("abc1@abc.com"));
        Assert.That(result.Last().EmailAddress, Is.EqualTo("abc2@abc.com"));
    }

    [Test]
    public void StdCodeParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().StdCode, Is.EqualTo("57"));
        Assert.That(result.Last().StdCode, Is.EqualTo("58"));
    }

    [Test]
    public void StartDateParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().StartDate, Is.EqualTo("2017-05-03"));
        Assert.That(result.Last().StartDate, Is.EqualTo("2018-06-01"));
    }

    [Test]
    public void EndDateParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().EndDate, Is.EqualTo("2018-05"));
        Assert.That(result.Last().EndDate, Is.EqualTo("2019-06"));
    }

    [Test]
    public void TotalPriceParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().TotalPrice, Is.EqualTo("2000"));
        Assert.That(result.Last().TotalPrice, Is.EqualTo("3333"));
    }

    [Test]
    public void ProviderRefIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().ProviderRef, Is.EqualTo("CX768"));
        Assert.That(result.Last().ProviderRef, Is.EqualTo("ZB657"));
    }

    [Test]
    public void RecognisePriorLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().RecognisePriorLearning, Is.EqualTo("true"));
        Assert.That(result.Last().RecognisePriorLearning, Is.EqualTo("false"));
    }

    [Test]
    public void DurationReducedByLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().DurationReducedBy, Is.EqualTo("12"));
        Assert.That(result.Last().DurationReducedBy, Is.EqualTo(""));
    }

    [Test]
    public void PriceReducedByLearningIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().PriceReducedBy, Is.EqualTo("99"));
        Assert.That(result.Last().PriceReducedBy, Is.EqualTo(""));
    }

    [Test]
    public void TrainingTotalHoursIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().TrainingTotalHours, Is.EqualTo("1000"));
        Assert.That(result.Last().TrainingTotalHours, Is.EqualTo(""));
    }

    [Test]
    public void IsDurationReducedByRPLIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().IsDurationReducedByRPL, Is.EqualTo("TRUE"));
        Assert.That(result.Last().IsDurationReducedByRPL, Is.EqualTo(""));
    }

    [Test]
    public void TrainingHoursReductionIsParsedCorrectly()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.First().TrainingHoursReduction, Is.EqualTo("100"));
        Assert.That(result.Last().TrainingHoursReduction, Is.EqualTo(""));
    }

    [Test]
    public void CorrectNumberOfApprenticeshipMapped()
    {
        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.Count, Is.EqualTo(2));
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

        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public void VerifyNoRowsReturnedFromEmptyFile()
    {
        //Arrange          
        _fileContent = Headers + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,," + Environment.NewLine +
                       ",,,,,,,,,,,,,,,,,,";

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void OptionalFieldsMayBeOmitted()
    {
        //Arrange          
        _fileContent = Headers.Replace(",TrainingTotalHours,IsDurationReducedByRPL,TrainingHoursReduction", "") + Environment.NewLine +
                       "P9DD4P,XEGE5X,8652496047,Jones,Louise,2000-01-01,abc1@abc.com,57,2017-05-03,2018-05,2000,,CX768,true,12,99" + Environment.NewLine;

        CreateFile();

        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
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

        var result = _bulkUploadFileParser.GetCsvRecords(_proivderId, _file);
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
        var fileName = "test.pdf";
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(_fileContent);
        writer.Flush();
        stream.Position = 0;

        _file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);
    }
}