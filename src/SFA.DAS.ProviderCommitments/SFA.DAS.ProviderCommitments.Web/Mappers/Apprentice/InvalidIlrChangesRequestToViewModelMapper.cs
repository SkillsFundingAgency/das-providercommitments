using SFA.DAS.CommitmentsV2.Shared.Extensions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;
using System.Globalization;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class InvalidIlrChangesRequestToViewModelMapper(IOuterApiClient outerApiClient)
    : IMapper<InvalidIlrChangesRequest, InvalidIlrChangesViewModel>
{
    private static readonly string[] PriceFields = ["TNP1", "TNP2"];

    public async Task<InvalidIlrChangesViewModel> Map(InvalidIlrChangesRequest source)
    {
        var response = await outerApiClient.Get<GetInvalidIlrChangesResponse>(
            new GetInvalidIlrChangesRequest(source.ProviderId, source.ApprenticeshipId));

        response ??= new GetInvalidIlrChangesResponse();
        response.RequestSets ??= [];

        return new InvalidIlrChangesViewModel
        {
            ProviderId = source.ProviderId,
            ApprenticeshipHashedId = source.ApprenticeshipHashedId,
            ApprenticeshipId = source.ApprenticeshipId,
            LearnerName = $"{response.FirstName} {response.LastName}".Trim(),
            RequestSets = response.RequestSets.ConvertAll(set => new InvalidIlrChangeSetViewModel
            {
                ApprovalRequestId = set.ApprovalRequestId,
                Decision = set.Decision,
                Fields = ToDisplayFields(set.Fields)
            })
        };
    }

    private static List<InvalidIlrChangeFieldViewModel> ToDisplayFields(List<InvalidIlrChangeField> fields)
    {
        fields ??= [];

        var displayFields = new List<InvalidIlrChangeFieldViewModel>();
        var priceFields = fields.Where(field => PriceFields.Contains(field.Field, StringComparer.OrdinalIgnoreCase)).ToList();
        var otherFields = fields.Where(field => !PriceFields.Contains(field.Field, StringComparer.OrdinalIgnoreCase));

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

    private static string ToFieldDisplayName(string field)
    {
        return field switch
        {
            "DateOfBirth" => "Date of birth",
            _ => field
        };
    }
}
