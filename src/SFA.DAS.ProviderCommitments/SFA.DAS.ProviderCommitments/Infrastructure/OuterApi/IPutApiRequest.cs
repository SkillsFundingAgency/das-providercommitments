namespace SFA.DAS.ProviderCommitments.Infrastructure.OuterApi
{
    public interface IPutApiRequest
    {
        public string PutUrl { get; }
        public object Data { get; set; }
    }
}
