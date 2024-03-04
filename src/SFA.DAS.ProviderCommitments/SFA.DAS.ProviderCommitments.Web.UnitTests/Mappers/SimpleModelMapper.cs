using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using System;
using System.Linq;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    internal class SimpleModelMapper : IModelMapper
    {
        private readonly object[] _mappers;

        public SimpleModelMapper(params object[] mappers)
        {
            foreach (var mapper in mappers)
                EnsureIsMapper(mapper);

            _mappers = mappers;
        }

        private static void EnsureIsMapper(object mapper)
        {
            if (!IsIMapInstance(mapper))
                throw new ArgumentException($"`{mapper?.GetType()}` is not a valid IMapper<,>");
        }

        private static bool IsIMapInstance(object mapper)
           => mapper.GetType().GetInterfaces().Any(IsIMapType);

        public Task<T> Map<T>(object source) where T : class
        {
            var map = GetMapperMethod<T>(source);
            return map(source);
        }

        private Func<object, Task<T>> GetMapperMethod<T>(object source) where T : class
        {
            var mapper = GetMapper<T>(source);
            return s => Map<T>(mapper, source);
        }

        private object GetMapper<T>(object source) where T : class
            => FindMapper(source.GetType(), typeof(T))
            ?? throw new NotImplementedException($"No mapper for `{source?.GetType()}` -> `{typeof(T)}`");

        private object FindMapper(Type sourceType, Type destinationType)
        {
            return _mappers.FirstOrDefault(mapper =>
                mapper.GetType().GetInterfaces().Any(IsAppropriateMapper));

            bool IsAppropriateMapper(Type mapperType)
                => CanMapBetween(mapperType, sourceType, destinationType);
        }

        private static bool CanMapBetween(Type possibleMapperType, Type sourceType, Type destinationType)
        {
            if (!IsIMapType(possibleMapperType)) return false;

            return possibleMapperType.GetGenericArguments().SequenceEqual(
                new[] { sourceType, destinationType });
        }

        private static bool IsIMapType(Type possibleMapperType)
        {
            if (!possibleMapperType.IsInterface) return false;
            if (!possibleMapperType.IsGenericType) return false;
            if (possibleMapperType.GetGenericTypeDefinition() != typeof(IMapper<,>)) return false;
            return true;
        }

        private static Task<T> Map<T>(object mapper, object source) where T : class
        {
            var mapMethod = mapper.GetType().GetMethod(nameof(IMapper<T, T>.Map));
            var result = mapMethod.Invoke(mapper, new[] { source });
            return (Task<T>)result;
        }
    }
}