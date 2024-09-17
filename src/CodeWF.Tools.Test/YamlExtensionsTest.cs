namespace CodeWF.Tools.Test;

public class YamlExtensionsTest
{
    [Fact]
    public void Test_Yaml2Object_Success()
    {
        var obj = new RequestProducts() { Name = "CodeWF", Year = 5 };

        Assert.True(obj.ToYaml(out var yamlString, out var errorMsg));

        Assert.True(yamlString.FromYaml<RequestProducts>(out var newObj, out errorMsg));
    }

    [Fact]
    public void Test_Yaml2Json_Success()
    {
        var obj = new RequestProducts() { Name = "CodeWF", Year = 5 };

        Assert.True(obj.ToYaml(out var yamlString, out var errorMsg));
        Assert.True(yamlString.YamlToJson(out var jsonString, out errorMsg));
    }
}