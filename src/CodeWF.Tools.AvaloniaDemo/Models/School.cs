using System.Collections.Generic;

namespace CodeWF.Tools.AvaloniaDemo.Models;

public static class School
{
    public static Student ManualMockStudent()
    {
        var student = new Student()
        {
            Name = "Liu",
            Gender = Gender.Male,
            Year = 35,
            Tags = new[] { "Math", "Science", "History", "Art", "Computer Science" },
            MainProject = new Project
            {
                Id = "Math",
                Name = "Software Engineer",
                Record = 100
            },
        };
        student.Projects = new List<Project>();
        for (var i = 0; i < 10; i++)
        {
            student.Projects.Add(new Project
            {
                Id = "Math",
                Name = "Software Engineer",
                Record = 100
            });
        }
        student.Scords = new Dictionary<string, double>();
        for (var i = 0; i < 10; i++)
        {
            student.Scords.Add($"Subject{i}", i * 10.0);
        }
        student.Keys = new Dictionary<string, string>();
        for (var i = 0; i < 10; i++)
        {
            student.Keys.Add($"Key{i}", $"Value{i}");
        }
        return student;
    }
}