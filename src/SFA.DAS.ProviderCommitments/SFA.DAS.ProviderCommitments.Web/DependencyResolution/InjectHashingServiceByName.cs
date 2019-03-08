using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using SFA.DAS.ProviderCommitments.HashingTemp;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.ProviderCommitments.Web.DependencyResolution
{
    /// <summary>
    ///     This will inject the appropriate IHashingService instance into the constructor's parameters.
    /// </summary>
    /// <remarks>
    ///     There are two hashing services currently in use : i) Public Account ID Hashing service and 2) Public Account Legal Entity ID Hashing service.
    ///     A named instance for each of these is registered with IoC with the name from <see cref="HashingServiceNames"/>.
    ///     This policy will inspect each constructor parameter of type IHashingService and will use the one that matches
    ///     the parameter name.
    ///     Some caching of the types is done to minimize the overhead of the policy.
    /// </remarks>
    public class InjectHashingServiceByName : ConfiguredInstancePolicy
    {
        private readonly ConcurrentDictionary<string, ParameterInfo[]> _evaluatedItems = new ConcurrentDictionary<string, ParameterInfo[]>();

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            if (instance.Constructor == null)
            {
                return;
            }

            var hashParameters = _evaluatedItems.GetOrAdd(instance.Name, FindHashes(instance));

            if (hashParameters == null)
            {
                return;
            }

            foreach (var param in hashParameters)
            {
                var hashingService = new ReferencedInstance(param.Name);
                instance.Dependencies.AddForConstructorParameter(param, hashingService);
            }
        }

        private ParameterInfo[] FindHashes(IConfiguredInstance instance)
        {
            var hashParameters = instance.Constructor
                .GetParameters()
                .Where(x => x.ParameterType == typeof(IHashingService))
                .ToArray();

            return hashParameters.Length == 0 ? null : hashParameters;
        }
    }
}