using System.Reflection;

namespace Dinners.Application;

internal sealed class AssemblyReference
{
    internal static Assembly Assembly => typeof(AssemblyReference).Assembly;
}