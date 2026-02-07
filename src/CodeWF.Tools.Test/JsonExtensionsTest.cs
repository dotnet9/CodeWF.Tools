using System.Collections;

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

    [Fact]
    public void Test_Hashtable2Json_Success()
    {
        Hashtable table1 = new();
        table1[1] = "wq";
        table1["you"] = 2;
        table1[2] = "and me";

        table1.ToJson(out var json, out var errorMsg);

        var convertResult = json.FromJson(out Hashtable table2, out errorMsg);

        Assert.True(convertResult);
        Assert.Equal(table1.Count, table2.Count);
    }
}