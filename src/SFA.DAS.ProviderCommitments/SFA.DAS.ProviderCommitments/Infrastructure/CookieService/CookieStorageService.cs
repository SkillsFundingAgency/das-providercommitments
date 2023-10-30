using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure.CookieService;

public class CookieStorageService<T> : ICookieStorageService<T>
{
    private readonly ICookieService<T> _cookieService;
    public CookieStorageService(ICookieService<T> cookieService) => _cookieService = cookieService;

    public void Create(T item, string cookieName, int expiryDays = 1)
    {
        _cookieService.Create(cookieName, item, expiryDays);
    }

    public T Get(string cookieName)
    {
        return _cookieService.Get(cookieName);
    }

    public void Delete(string cookieName)
    {
        _cookieService.Delete(cookieName);
    }

    public void Update(string cookieName, T item, int expiryDays = 1)
    {
        Delete(cookieName);
        Create(item, cookieName, expiryDays);
    }
}