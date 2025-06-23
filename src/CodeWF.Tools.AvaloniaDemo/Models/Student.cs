using System.Collections.Generic;

namespace CodeWF.Tools.AvaloniaDemo.Models;

public class Student
{
    public string? Name { get; set; }
    public int Year { get; set; }

    public string[]? Tags { get; set; }

    public Project? MainProject { get; set; }

    public List<Project>? Projects { get; set; }

    public Dictionary<string, string> Keys { get; set; }
    public Dictionary<string, double> Scords { get; set; } 
}

public class Project
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Record { get; set; }
}