using System.Text.Json.Serialization;

namespace CodeWF.Tools.AvaloniaDemo.Models
{
    public class ClassWithEnum
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public TestEnum Type { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<TestEnum>))]
    public enum TestEnum
    {
        Light,
        Dark
    }
}