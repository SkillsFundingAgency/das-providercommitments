using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace SFA.DAS.ProviderCommitments.Web.Models.Cohort;

public class TrimStringConverter : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        return text?.Trim();
    }
}
    
public sealed class CsvRecordMap : ClassMap<CsvRecord>
{
    public CsvRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
            
        var stringProperties = typeof(CsvRecord).GetProperties()
            .Where(p => p.PropertyType == typeof(string));

        foreach (var property in stringProperties)
        {
            var memberMap = MemberMaps.FirstOrDefault(m => m.Data.Member.Name == property.Name);
            if (memberMap != null)
            {
                memberMap.TypeConverter<TrimStringConverter>();
            }
        }
    }
}