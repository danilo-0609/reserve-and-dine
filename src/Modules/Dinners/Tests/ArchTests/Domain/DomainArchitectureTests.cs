using API;
using BuildingBlocks.Domain.Rules;
using Dinners.Application;
using Dinners.Domain;
using Dinners.Infrastructure;
using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace Dinners.Tests.ArchTests.Domain;

public sealed class DomainArchitectureTests
{
    private readonly Assembly _domainAssembly = typeof(DomainAssemblyReference).Assembly;
    private readonly string _applicationNamespace = typeof(ApplicationAssemblyReference).Namespace!;
    private readonly string _infrastructureNamespace = typeof(InfrastructureAssemblyReference).Namespace!;
    private readonly string _presentationNamespace = typeof(PresentationAssemblyReference).Namespace!;

    [Fact]
    public void Domain_ShouldNot_HaveDependenciesOnOtherProjects()
    {
        string[] otherProjects = new[]
        {
            _applicationNamespace,
            _infrastructureNamespace,
            _presentationNamespace
        };


        var testResult = Types
            .InAssembly(_domainAssembly)
            .Should()
            .NotHaveDependencyOnAny(otherProjects)
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void BusinessRules_Should_HaveNamesEndingWith()
    {
        var testResult = Types
            .InAssembly(_domainAssembly)
            .That()
            .ImplementInterface(typeof(IBusinessRule))
            .Should()
            .HaveNameEndingWith("Rule")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();  
    }
}
