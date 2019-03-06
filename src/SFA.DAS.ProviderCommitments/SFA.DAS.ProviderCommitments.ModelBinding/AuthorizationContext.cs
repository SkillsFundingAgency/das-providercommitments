using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.ProviderCommitments.ModelBinding
{
    public class AuthorizationContext : IAuthorizationContext
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public T Get<T>(string key)
        {
            if (!_data.TryGetValue(key, out var value))
            {
                throw new KeyNotFoundException($"The key '{key}' was not present in the authorization context");
            }

            return (T)value;
        }

        public void Set<T>(string key, T value)
        {
            _data[key] = value;
        }

        public bool TryGet<T>(string key, out T value)
        {
            var exists = _data.TryGetValue(key, out var obj);

            value = exists ? (T)obj : default;

            return exists;
        }
    }
}
