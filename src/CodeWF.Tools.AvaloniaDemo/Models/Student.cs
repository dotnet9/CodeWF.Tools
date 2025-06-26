using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace CodeWF.Tools.AvaloniaDemo.Models;

public class Student
{
    public string? Name { get; set; }
    public Gender Gender { get; set; }
    public int Year { get; set; }

    public string[]? Tags { get; set; }

    public Project? MainProject { get; set; }

    public List<Project>? Projects { get; set; }

    public Dictionary<string, string> Keys { get; set; }
    public Dictionary<string, double> Scords { get; set; }

    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 表示性别的枚举
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Gender>))]
public enum Gender
{
    /// <summary>
    /// 未知性别
    /// </summary>
    [Description("未知")] Unknown = 0,

    /// <summary>
    /// 男性
    /// </summary>
    [Description("男")] Male = 1,

    /// <summary>
    /// 女性
    /// </summary>
    [Description("女")] Female = 2,

    /// <summary>
    /// 其他性别
    /// </summary>
    [Description("其他")] Other = 3
}

public class Project
{
    public string Id { get; set; }

    public string Name { get; set; }

    public int Record { get; set; }

    [XmlArray(ElementName = "Members")]
    [XmlArrayItem(typeof(Member))]
    public List<Member> Members { get; set; }
}

public class Member
{
    public string? Name { get; set; }

    public int Age { get; set; }
}