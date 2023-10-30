namespace SFA.DAS.ProviderCommitments.Interfaces;

public interface ICookieService<T>
{
    void Create(string name, T content, int expireDays);

    void Update(string name, T content);

    void Delete(string name);

    T Get(string name);
}