﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.ProviderCommitments.Web.Extensions;

namespace SFA.DAS.ProviderCommitments.Web.Services
{
    public interface ITempDataStorageService
    {
        T RetrieveFromCache<T>() where T : class;
        T RetrieveFromCache<T>(string key) where T : class;
        void RemoveFromCache<T>() where T: class;
        void AddToCache(object value);
        void AddToCache(object value, string key);
    }

    public class TempDataStorageService : ITempDataStorageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;

        public TempDataStorageService(IHttpContextAccessor httpContextAccessor, ITempDataDictionaryFactory tempDataDictionaryFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _tempDataDictionaryFactory = tempDataDictionaryFactory;
        }

        public T RetrieveFromCache<T>() where T: class
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            return tempData.GetButDontRemove<T>(typeof(T).Name);
        }

        public T RetrieveFromCache<T>(string key) where T : class
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            return tempData.GetButDontRemove<T>(key);
        }

        public void RemoveFromCache<T>() where T: class
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            var key = typeof(T).Name;

            tempData.TryGetValue(key, out var o);

            if (o != null)
            {
                tempData.Remove(new KeyValuePair<string, object>(key, o));
            }
        }

        public void AddToCache(object value)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            var key = value.GetType().Name;
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public void AddToCache(object value, string key)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tempData = _tempDataDictionaryFactory.GetTempData(httpContext);
            tempData[key] = JsonConvert.SerializeObject(value);
        }
    }
}
