using System.Reflection;

namespace Dinners.Application;

internal sealed class ApplicationAssemblyReference
{
    internal static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
}