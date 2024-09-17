namespace CodeWF.Tools.Test;

public class YamlExtensionsTest
{
    [Fact]
    public void Test_Yaml2Object_Success()
    {
        var obj = School.GetStudent();

        Assert.True(obj.ToYaml(out var yamlString, out var errorMsg));

        Assert.True(yamlString.FromYaml<Student>(out var newObj, out errorMsg));
    }

    [Fact]
    public void Test_Yaml2Json_Success()
    {
        var obj = School.GetStudent();

        Assert.True(obj.ToYaml(out var yamlString, out var errorMsg));
        Assert.True(yamlString.YamlToJson(out var jsonString, out errorMsg));
    }
}