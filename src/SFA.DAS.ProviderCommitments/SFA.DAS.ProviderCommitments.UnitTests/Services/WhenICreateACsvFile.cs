using System;
using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using SFA.DAS.ProviderCommitments.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderCommitments.UnitTests.Services
{
    public class WhenICreateACsvFile
    {
        [Test, MoqAutoData]
        public void Then_The_First_Line_Of_The_File_Is_The_Headers(
            List<SomethingToCsv> listToWriteToCsv,
            CreateCsvService createCsvService)
        {
            var actual = createCsvService.GenerateCsvContent(listToWriteToCsv);

            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
            Assert.IsAssignableFrom<byte[]>(actual);
            var fileString = System.Text.Encoding.Default.GetString(actual);
            var headerLine = fileString.Split(Environment.NewLine)[0];

            Assert.AreEqual(3,headerLine.Split(',').Length);
            Assert.That(headerLine.Contains(nameof(SomethingToCsv.Id)));
            Assert.That(headerLine.Contains(nameof(SomethingToCsv.Description)));
            Assert.That(headerLine.Contains(nameof(SomethingToCsv.MoreInternals)));
            Assert.That(!headerLine.Contains(nameof(SomethingToCsv.InternalStuff)));
        }

        [Test, MoqAutoData]
        public void Then_The_Csv_File_Content_Is_Generated(
            List<SomethingToCsv> listToWriteToCsv,
            CreateCsvService createCsvService)
        {
            var actual = createCsvService.GenerateCsvContent(listToWriteToCsv);

            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
            Assert.IsAssignableFrom<byte[]>(actual);
            var fileString = System.Text.Encoding.Default.GetString(actual);
            var lines = fileString.Split(Environment.NewLine);
            Assert.AreEqual(listToWriteToCsv.Count + 2,lines.Length);
            Assert.AreEqual(3,lines[0].Split(',').Length);
            Assert.AreEqual(listToWriteToCsv[0].Id.ToString(),lines[1].Split(',')[0]);
            Assert.AreEqual(listToWriteToCsv[0].Description,lines[1].Split(',')[1]);
            Assert.AreEqual(listToWriteToCsv[0].MoreInternals,lines[1].Split(',')[2]);
        }

        [Test, MoqAutoData]
        public void And_Nothing_Is_Passed_To_The_Content_Generator_Then_Exception_Is_Thrown(
            CreateCsvService createCsvService)
        {
            List<SomethingToCsv> nullList = null;

            Assert.Throws<WriterException>(() => createCsvService.GenerateCsvContent(nullList));
        }

        [Test, MoqAutoData]
        public void And_Mapper_Used_Then_Headers_According_To_Mapper(
            List<SomethingToCsv> listToWriteToCsv,
            CreateCsvService createCsvService)
        {
            var actual = createCsvService.GenerateCsvContent<SomethingToCsv, SomethingToCsvMap>(listToWriteToCsv);

            var fileString = System.Text.Encoding.Default.GetString(actual);
            var headerLine = fileString.Split(Environment.NewLine)[0];

            Assert.AreEqual(2,headerLine.Split(',').Length);
            Assert.That(!headerLine.Contains(nameof(SomethingToCsv.MoreInternals)));
        }

        [Test, MoqAutoData]
        public void And_Mapper_Used_Then_Maps_List_To_Column(
            List<SomethingToCsvWithList> listToWriteToCsv,
            CreateCsvService createCsvService)
        {
            var actual = createCsvService.GenerateCsvContent<SomethingToCsvWithList, SomethingToCsvWithListMap>(listToWriteToCsv);

            var fileString = System.Text.Encoding.Default.GetString(actual);
            var lines = fileString.Split(Environment.NewLine);
            var headerLine = lines[0];

            Assert.AreEqual(4,headerLine.Split(',').Length);
            Assert.That(headerLine.Contains(nameof(SomethingToCsvWithList.NastyList)));
            Assert.AreEqual(listToWriteToCsv[0].NastyList.Aggregate((a,b) => $"{a}|{b}"),lines[1].Split(',')[3]);
        }

        [Test, MoqAutoData]
        public void And_Mapper_Used_Then_Maps_List_Of_Single_Value_To_Column(
            List<SomethingToCsvWithList> listToWriteToCsv,
            string singleItem,
            CreateCsvService createCsvService)
        {
            foreach (var somethingToCsvWithList in listToWriteToCsv)
            {
                somethingToCsvWithList.NastyList = new List<string>{singleItem};
            }

            var actual = createCsvService.GenerateCsvContent<SomethingToCsvWithList, SomethingToCsvWithListMap>(listToWriteToCsv);

            var fileString = System.Text.Encoding.Default.GetString(actual);
            var lines = fileString.Split(Environment.NewLine);
            var headerLine = lines[0];

            Assert.AreEqual(4,headerLine.Split(',').Length);
            Assert.That(headerLine.Contains(nameof(SomethingToCsvWithList.NastyList)));
            Assert.AreEqual(singleItem,lines[1].Split(',')[3]);
        }
    }

    public class SomethingToCsv
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public long InternalStuff { get; set; }
        public string MoreInternals { get; set; }
    }

    public class SomethingToCsvMap : ClassMap<SomethingToCsv>
    {
        public SomethingToCsvMap()
        {
            AutoMap();
            Map(m => m.MoreInternals).Ignore();
        }
    }

    public class SomethingToCsvWithList : SomethingToCsv
    {
        public IEnumerable<string> NastyList { get; set; }
    }

    public class SomethingToCsvWithListMap : ClassMap<SomethingToCsvWithList>
    {
        public SomethingToCsvWithListMap()
        {
            AutoMap();
            Map(m => m.NastyList).ConvertUsing(m => m.NastyList.Aggregate((a, b) => $"{a}|{b}"));
        }
    } 
}