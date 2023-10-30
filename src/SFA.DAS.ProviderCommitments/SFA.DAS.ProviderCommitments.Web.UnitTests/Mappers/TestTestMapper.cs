using FluentAssertions;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System;

namespace SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers
{
    public class TestTestMapper
    {
        [Test, MoqAutoData]
        public async Task Maps_between_types_with_supplied_mapper(A source)
        {
            var sut = new SimpleModelMapper(new AToBMapper());
            var result = await sut.Map<B>(source);
            result.Id.Should().Be(source.Id);
        }

        [Test, MoqAutoData]
        public async Task Cannot_map_between_types_without_a_mapper(B source)
        {
            var sut = new SimpleModelMapper(new AToBMapper());

            await sut.Invoking(s => s.Map<C>(source))
                .Should().ThrowAsync<Exception>()
                .WithMessage($"No mapper for `SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.B` -> `SFA.DAS.ProviderCommitments.Web.UnitTests.Mappers.C`");
        }

        [Test, MoqAutoData]
        public async Task Maps_between_types_with_multiple_mappers(A source)
        {
            var sut = new SimpleModelMapper(new AToBMapper(), new AToCMapper());
            var result = await sut.Map<C>(source);
            result.Id.Should().Be(source.Id);
        }

        [Test]
        public void Fail_fast_when_adding_invalid_mappers()
        {
            Action ctor = () => new SimpleModelMapper("I am not a mapper");

            ctor.Should().Throw<Exception>().WithMessage("`System.String` is not a valid IMapper<,>");
        }
    }

    public class A
    { public int Id { get; set; } }

    public class B
    { public int Id { get; set; } }

    public class C
    { public int Id { get; set; } }

    internal class AToBMapper : IMapper<A, B>
    {
        public Task<B> Map(A source) => Task.FromResult(new B { Id = source.Id });
    }

    internal class AToCMapper : IMapper<A, C>
    {
        public Task<C> Map(A source) => Task.FromResult(new C { Id = source.Id });
    }
}