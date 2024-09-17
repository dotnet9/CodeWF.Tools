namespace CodeWF.Tools.Test;

public class JsonExtensionsTest
{
    [Fact]
    public void Test_Json2Object_Success()
    {
        var obj = School.GetStudent();

        Assert.True(obj.ToJson(out var jsonString, out var errorMsg));

        Assert.True(jsonString.FromJson<Student>(out var newObj, out errorMsg));
    }


    [Fact]
    public void Test_Json2Yaml_Success()
    {
        var obj = School.GetStudent();

        Assert.True(obj.ToJson(out var jsonString, out var errorMsg));
        Assert.True(jsonString.JsonToYaml(out var yamlString, out errorMsg));
    }
}