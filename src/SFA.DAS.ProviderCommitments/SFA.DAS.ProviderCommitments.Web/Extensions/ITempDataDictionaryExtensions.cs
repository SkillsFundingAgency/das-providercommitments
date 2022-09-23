using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace SFA.DAS.ProviderCommitments.Web.Extensions
{
    public static class ITempDataDictionaryExtensions
    {
        public static readonly string FlashMessageTempDataKey = "FlashMessage";
        public static readonly string FlashMessageTitleTempDataKey = "FlashMessageTitle";        
        public static readonly string FlashMessageLevelTempDataKey = "FlashMessageLevel";

        public enum FlashMessageLevel
        {
            Info,
            Warning,
            Success
        }

        public static void AddFlashMessage(this ITempDataDictionary tempData, string message, FlashMessageLevel level)
        {
            tempData[FlashMessageTempDataKey] = message;
            tempData[FlashMessageLevelTempDataKey] = level;
        }

        public static void AddFlashMessage(this ITempDataDictionary tempData, string title, string message, FlashMessageLevel level)
        {
            tempData[FlashMessageTitleTempDataKey] = title;
            tempData[FlashMessageTempDataKey] = message;            
            tempData[FlashMessageLevelTempDataKey] = level;
        }

        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

        public static T GetButDontRemove<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            var result = tempData.Peek(key);
            return result == null ? null : JsonConvert.DeserializeObject<T>((string)result);
        }
    }
}
