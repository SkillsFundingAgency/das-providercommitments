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

        return MapResponse(source, response, new InvalidIlrChangesViewModel(), ApplyInvalidIlrCopy);
    }

    public static TViewModel MapResponse<TViewModel>(
        InvalidIlrChangesRequest source,
        GetInvalidIlrChangesResponse response,
        TViewModel viewModel,
        Action<TViewModel, string> applyCopy)
        where TViewModel : InvalidIlrChangesViewModel
    {
        response ??= new GetInvalidIlrChangesResponse();
        response.RequestSets ??= [];

        var learnerName = $"{response.FirstName} {response.LastName}".Trim();

        viewModel.ProviderId = source.ProviderId;
        viewModel.ApprenticeshipHashedId = source.ApprenticeshipHashedId;
        viewModel.ApprenticeshipId = source.ApprenticeshipId;
        viewModel.LearnerName = learnerName;
        viewModel.RequestSets = response.RequestSets.ConvertAll(set => new InvalidIlrChangeSetViewModel
        {
            ApprovalRequestId = set.ApprovalRequestId,
            Decision = set.Decision,
            Fields = ApprovalChangeFieldFormatter.ToDisplayFields(set.Fields)
        });

        applyCopy(viewModel, learnerName);
        return viewModel;
    }

    public static void ApplyInvalidIlrCopy(InvalidIlrChangesViewModel viewModel, string learnerName)
    {
        viewModel.Heading = $"Invalid ILR changes for {learnerName}";
        viewModel.Intro = "These changes were automatically rejected. This is because one or more fields in the ILR file were invalid.";
        viewModel.NextSteps = "Correct any invalid fields and resubmit the ILR file.";
        viewModel.FieldColumnHeader = "Field";
        viewModel.NewValueColumnHeader = "Rejected";
        viewModel.LegendHint = null;
        viewModel.ChangeSetCaption = "Invalid ILR changes";
        viewModel.ChangeSetCaptionPrefix = "Invalid ILR change";
        viewModel.GaVpv = "/apprentices/apprentice/invalid-ilr-changes";
    }

    public static void ApplyDeclinedCopy(InvalidIlrChangesViewModel viewModel, string learnerName)
    {
        viewModel.Heading = $"Changes declined for {learnerName}";
        viewModel.Intro = "These changes were declined by your employer.";
        viewModel.NextSteps = "Contact your employer to check the details you've entered are correct, then resubmit the changes to your employer.";
        viewModel.FieldColumnHeader = string.Empty;
        viewModel.NewValueColumnHeader = "Declined";
        viewModel.LegendHint = "You will still see the changes in your change history but the alert will disappear.";
        viewModel.ChangeSetCaption = "Declined changes";
        viewModel.ChangeSetCaptionPrefix = "Declined change";
        viewModel.GaVpv = "/apprentices/apprentice/declined-changes";
    }
}
