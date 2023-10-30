using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Interfaces;

namespace SFA.DAS.ProviderCommitments.Infrastructure.CookieService;

public class HttpCookieService<T> : ICookieService<T>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDataProtector _protector;

    public HttpCookieService(IDataProtectionProvider provider, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _protector = provider.CreateProtector("SFA.DAS.ProviderCommitments.Services.HttpCookieService");
    }

    public void Create(string name, T content, int expireDays)
    {
        var cookieContent = JsonConvert.SerializeObject(content);

        var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));

        _httpContextAccessor.HttpContext.Response.Cookies.Append(name, encodedContent, new CookieOptions
        {
            Expires = DateTime.Now.AddDays(expireDays),
            Secure = true,
            HttpOnly = true,
        });
    }

    public void Update(string name, T content)
    {
        var cookie = _httpContextAccessor.HttpContext.Request.Cookies[name];

        if (cookie != null)
        {
            var cookieContent = JsonConvert.SerializeObject(content);

            var encodedContent = Convert.ToBase64String(_protector.Protect(new UTF8Encoding().GetBytes(cookieContent)));
            _httpContextAccessor.HttpContext.Response.Cookies.Append(name, encodedContent);
        }
    }

    public void Delete(string name)
    {
        if (_httpContextAccessor.HttpContext.Request.Cookies[name] != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(name);
        }
    }

    public T Get(string name)
    {
        if (_httpContextAccessor.HttpContext.Request.Cookies[name] == null)
            return default(T);

        var base64EncodedBytes = Convert.FromBase64String(_httpContextAccessor.HttpContext.Request.Cookies[name]);
        byte[] unprotect;
        try
        {
            unprotect = _protector.Unprotect(base64EncodedBytes);
        }
        catch (CryptographicException)
        {
            Delete(name);
            return default(T);
        }
        
        return JsonConvert.DeserializeObject<T>(new UTF8Encoding().GetString(unprotect));
    }
}