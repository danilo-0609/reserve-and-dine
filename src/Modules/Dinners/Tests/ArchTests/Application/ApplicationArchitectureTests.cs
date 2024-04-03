using Dinners.Application;
using Dinners.Application.Common;
using FluentAssertions;
using MediatR;
using NetArchTest.Rules;
using System.Reflection;

namespace Dinners.Tests.ArchTests.Application;

public sealed class ApplicationArchitectureTests
{
    private readonly Assembly _applicationAssembly = typeof(ApplicationAssemblyReference).Assembly;

    [Fact]
    public void Handlers_Should_BeInternal()
    {
        var testResult = Types
            .InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryRequests_Should_HaveNamesEndingWithQuery()
    {
        var testResult = Types
            .InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
}
