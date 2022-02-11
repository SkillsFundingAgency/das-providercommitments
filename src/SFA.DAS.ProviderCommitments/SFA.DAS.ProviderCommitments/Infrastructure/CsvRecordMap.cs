using CsvHelper.Configuration;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort
{
    public sealed class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            AutoMap();
        }
    }
}
