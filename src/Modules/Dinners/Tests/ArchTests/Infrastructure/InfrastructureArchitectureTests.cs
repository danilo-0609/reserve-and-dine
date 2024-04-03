using Dinners.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using System.Reflection;

namespace Dinners.Tests.ArchTests.Infrastructure;

public sealed class InfrastructureArchitectureTests
{
    private readonly Assembly _infrastructureAssembly = typeof(InfrastructureAssemblyReference).Assembly;

    [Fact]
    public void EntityTypeConfigurations_Should_BeInternal()
    {
        var testResult = Types
            .InAssembly(_infrastructureAssembly)
            .That()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .NotBePublic()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Repositories_Should_BeInternal()
    {
        var testResult = Types
            .InAssembly(_infrastructureAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Should()
            .NotBePublic()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
}
