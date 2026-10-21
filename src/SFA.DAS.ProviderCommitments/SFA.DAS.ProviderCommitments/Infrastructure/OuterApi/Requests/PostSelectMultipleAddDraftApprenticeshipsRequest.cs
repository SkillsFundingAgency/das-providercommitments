namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi.Requests;

public class PostSelectMultipleAddDraftApprenticeshipsRequest : IPostApiRequest
{
    public string PostUrl => "SelectMultiple/AddDraftApprenticeships";

    public object Data { get; set; }

    public PostSelectMultipleAddDraftApprenticeshipsRequest(SelectMultipleAddDraftApprenticeshipsApimRequest request)
    {
        Data = request;
    }
}
