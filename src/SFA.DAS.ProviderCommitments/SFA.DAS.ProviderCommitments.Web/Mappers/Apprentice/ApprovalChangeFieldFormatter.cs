using System.Globalization;
using System.Text.Json;
using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public static class ApprovalChangeFieldFormatter
{
    private static readonly string[] PriceFields = ["TNP1", "TNP2"];

    public static List<InvalidIlrChangeFieldViewModel> ToDisplayFields(List<InvalidIlrChangeField> fields)
    {
        fields ??= [];

        var displayFields = new List<InvalidIlrChangeFieldViewModel>();
        var priceFields = fields.Where(field => PriceFields.Contains(field.Field, StringComparer.OrdinalIgnoreCase)).ToList();
        var scheduleField = fields.FirstOrDefault(field => string.Equals(field.Field, "Prices", StringComparison.OrdinalIgnoreCase));
        var otherFields = fields.Where(field =>
            !PriceFields.Contains(field.Field, StringComparer.OrdinalIgnoreCase)
            && !string.Equals(field.Field, "Prices", StringComparison.OrdinalIgnoreCase));

        if (priceFields.Count > 0)
        {
            displayFields.Add(new InvalidIlrChangeFieldViewModel
            {
                Field = "TotalPrice",
                FieldDisplayName = "Total price",
                Old = SumAmounts(priceFields, field => field.Old).ToGdsCostFormat(),
                New = SumAmounts(priceFields, field => field.New).ToGdsCostFormat()
            });
        }

        if (scheduleField != null)
        {
            displayFields.Add(new InvalidIlrChangeFieldViewModel
            {
                Field = "TotalPrice",
                FieldDisplayName = "Total price",
                Old = FormatLatestCost(scheduleField.Old),
                New = FormatLatestCost(scheduleField.New)
            });
        }

        displayFields.AddRange(otherFields.Select(field => new InvalidIlrChangeFieldViewModel
        {
            Field = field.Field,
            FieldDisplayName = ToFieldDisplayName(field.Field),
            Old = FormatValue(field.Old),
            New = FormatValue(field.New)
        }));

        return displayFields;
    }

    private static decimal SumAmounts(IEnumerable<InvalidIlrChangeField> fields, Func<InvalidIlrChangeField, string> selector)
    {
        return fields.Sum(field => ParseAmount(selector(field)));
    }

    private static decimal ParseAmount(string value)
    {
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount)
            ? amount
            : 0;
    }

    private static string FormatValue(string value)
    {
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return date.ToGdsFormat();
        }

        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
        {
            return amount.ToGdsCostFormat();
        }

        return value;
    }

    private static string FormatLatestCost(string schedule)
    {
        if (string.IsNullOrWhiteSpace(schedule))
        {
            return "#error#";
        }

        try
        {
            var latestEpisode = JsonSerializer.Deserialize<List<PriceScheduleEpisode>>(schedule, PriceJsonOptions)?
                .Where(episode => episode != null && !string.IsNullOrWhiteSpace(episode.EffectiveFrom))
                .OrderBy(episode => episode.EffectiveFrom, StringComparer.Ordinal)
                .LastOrDefault();

            return latestEpisode == null
                ? "#error#"
                : latestEpisode.Cost.ToGdsCostFormat();
        }
        catch (JsonException)
        {
            return "#error#";
        }
    }

    private static string ToFieldDisplayName(string field)
    {
        return field switch
        {
            "DateOfBirth" => "Date of birth",
            _ => field
        };
    }

    private static readonly JsonSerializerOptions PriceJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class PriceScheduleEpisode
    {
        public string EffectiveFrom { get; set; }
        public decimal Cost { get; set; }
    }
}
