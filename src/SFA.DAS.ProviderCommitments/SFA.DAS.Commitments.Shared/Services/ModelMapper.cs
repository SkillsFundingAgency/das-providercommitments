using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Shared.Interfaces;

namespace SFA.DAS.Commitments.Shared.Services
{
    public class ModelMapper : IModelMapper
    {
        private readonly IServiceProvider _serviceProvider;

        public ModelMapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<T> Map<T>(object source) where T : class
        {
            var sourceType = source.GetType();
            var destinationType = typeof(T);

            Type[] typeArgs = { sourceType, destinationType };
            var mapperType = typeof(IMapper<,>).MakeGenericType(typeArgs);

            object mapper;

            try
            {
                mapper = _serviceProvider.GetRequiredService(mapperType);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException($"Unable to locate implementation of IMapper<{sourceType.Name},{destinationType.Name}>", e);
            }

            var mapMethod = mapper.GetType().GetMethod(nameof(IMapper<T, T>.Map));
            var result = mapMethod.Invoke(mapper, new[] { source });

            return result as Task<T>;
        }
    }
}
