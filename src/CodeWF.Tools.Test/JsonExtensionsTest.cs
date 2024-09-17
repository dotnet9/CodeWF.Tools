using System.ComponentModel;
using System.Dynamic;

namespace CodeWF.Tools.Test;

public class JsonExtensionsTest
{
    [Fact]
    public void Test_Json2Object_Success()
    {
        var obj = new RequestProducts() { Name = "CodeWF", Year = 5 };

        Assert.True(obj.ToJson(out var jsonString, out var errorMsg));

        Assert.True(jsonString.FromJson<RequestProducts>(out var newObj, out errorMsg));
    }


    [Fact]
    public void Test_Json2Yaml_Success()
    {
        var obj = new RequestProducts() { Name = "CodeWF", Year = 5 };
        
        Assert.True(obj.ToJson(out var jsonString, out var errorMsg));
        Assert.True(jsonString.JsonToYaml(out var yamlString, out errorMsg));
    }
}