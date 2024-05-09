using System.Reflection;

namespace Users.Application;

internal sealed class ApplicationAssemblyReference
{
    internal static Assembly Assembly => typeof(ApplicationAssemblyReference).Assembly;
}