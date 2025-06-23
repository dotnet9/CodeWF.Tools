using System.Collections.Generic;
using System.Xml.Serialization;

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
    [XmlAttribute]
    public string Id { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public int Record { get; set; }

    [XmlArray(ElementName = nameof(Members))]

    [XmlArrayItem(typeof(Member))]
    [XmlAttribute]
    public List<Member> Members { get; set; }
}

public class Member
{

    [XmlAttribute]
    public string? Name { get; set; }

    [XmlAttribute]
    public int Age { get; set; }
}