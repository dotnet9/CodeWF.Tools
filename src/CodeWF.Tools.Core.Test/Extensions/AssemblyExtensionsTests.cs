using System.Reflection;
using CodeWF.Tools.Extensions;
using CodeWfAssemblyExtensions = CodeWF.Tools.Extensions.AssemblyExtensions;

namespace CodeWF.Tools.Core.Test.Extensions;

public class AssemblyExtensionsTests
{
    [Fact]
    public void CompileTime_NullAssembly_ReturnsNull()
    {
        Assembly? assembly = null;

        Assert.Null(assembly.CompileTime());
    }

    [Fact]
    public void CompileTime_UsesTheProvidedAssembly()
    {
        var compileTime = typeof(CodeWfAssemblyExtensions).Assembly.CompileTime();

        Assert.NotNull(compileTime);
    }
}
