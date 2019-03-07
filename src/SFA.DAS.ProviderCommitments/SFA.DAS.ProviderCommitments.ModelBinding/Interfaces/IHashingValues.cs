namespace SFA.DAS.ProviderCommitments.ModelBinding.Interfaces
{
    public interface IHashingValues
    {
        T Get<T>(string key);

        void Set<T>(string key, T value);

        bool TryGet<T>(string key, out T value);
    }
}