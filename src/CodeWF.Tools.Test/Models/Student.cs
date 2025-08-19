using System.Text.Json.Serialization;

namespace CodeWF.Tools.Test.Models;

public class Student
{
    [JsonPropertyName("_Name")]
    public string? Name { get; set; }
    public int Year { get; set; }

    public string[]? Tags { get; set; }

    public DateTime BirthDate { get; set; }

    public Project? MainProject { get; set; }

    public List<Project>? Projects { get; set; }

    public Dictionary<string, string> Keys { get; set; }
}

public class Project
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Record { get; set; }
}