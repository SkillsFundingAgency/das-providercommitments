using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi;
using SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests.Apprentices;
using SFA.DAS.ProviderCommitments.Web.Models.Apprentice;

namespace SFA.DAS.ProviderCommitments.Web.Mappers.Apprentice;

public class InvalidIlrChangesRequestToViewModelMapper(IOuterApiClient outerApiClient)
    : IMapper<InvalidIlrChangesRequest, InvalidIlrChangesViewModel>
{
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
                Fields = (set.Fields ?? []).ConvertAll(field => new InvalidIlrChangeFieldViewModel
                {
                    Field = field.Field,
                    FieldDisplayName = ToFieldDisplayName(field.Field),
                    Old = field.Old,
                    New = field.New,
                    EffectiveFrom = field.EffectiveFrom,
                    Reason = field.Reason
                })
            })
        };
    }

    private static string ToFieldDisplayName(string field)
    {
        return field switch
        {
            "TNP1" => "Training price",
            "TNP2" => "End-point assessment price",
            _ => field
        };
    }
}
