using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Commitments.Shared.Configuration;
using StructureMap;

namespace SFA.DAS.Commitments.Shared.DependencyInjection
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            AddConfiguration<CourseApiClientConfiguration>(ConfigurationKeys.CourseApiClientConfiguration);
        }

        private void AddConfiguration<T>(string key) where T : class
        {
            For<T>().Use(c => GetConfiguration<T>(c, key)).Singleton();
        }

        private T GetConfiguration<T>(IContext context, string name)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var section = configuration.GetSection(name);
            var value = section.Get<T>();

            return value;
        }
    }
}
