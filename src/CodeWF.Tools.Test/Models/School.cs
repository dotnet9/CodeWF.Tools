using Bogus;

namespace CodeWF.Tools.Test.Models;

public static class School
{
    public static Student GetStudent()
    {
        var faker = new Faker<Student>()
            .RuleFor(s => s.Name, f => f.Name.FullName())
            .RuleFor(s => s.Year, f => f.Random.Int(1, 12)) // 假设学生年级在1到12之间  
            .RuleFor(x => x.BirthDate, f=>f.Date.Past(20, DateTime.Now)) // 生成一个20年前的日期
            .RuleFor(s => s.Tags, f => { return new[] { "Math", "Science", "History", "Art", "Computer Science" }; })
            .RuleFor(s => s.MainProject, f =>
            {
                var project = new Project
                {
                    Id = f.Address.Country(),
                    Name = f.Name.JobTitle(),
                    Record = f.Random.Int(1, 100)
                };
                // 注意：这里我们没有为MainProject的Id设置规则，因为它在Project类中已经被初始化为Guid  
                return project;
            })
            .RuleFor(s => s.Projects, f =>
            {
                var projectCount = f.Random.Int(5, 10); // 生成1到5个随机数量的项目  
                var projects = new List<Project>();
                for (int i = 0; i < projectCount; i++)
                {
                    projects.Add(new Project
                    {
                        Id = f.Address.Country(),
                        Name = f.Name.JobTitle(),
                        Record = f.Random.Int(1, 100)
                    });
                }

                return projects;
            })
            .RuleFor(s => s.Keys, f =>
            {
                var keyCount = f.Random.Int(5, 10); // 生成1到5个随机数量的键值对  
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < keyCount; i++)
                {
                    dict[f.Random.Word()] = f.Random.Word();
                }

                return dict;
            });

        var student = faker.Generate();
        return student;
    }
}