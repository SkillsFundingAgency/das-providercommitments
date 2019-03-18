namespace SFA.DAS.ProviderCommitments.ModelBinding.Interfaces
{
    /// <summary>
    ///     This is a property bag of the hashed values that have been collected from the in-bound request.
    /// </summary>
    public interface IHashingValues
    {
        T Get<T>(string key);

        void Set<T>(string key, T value);

        bool TryGet<T>(string key, out T value);
    }
}