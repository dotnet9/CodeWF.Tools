using System.Text.Json.Serialization;

namespace CodeWF.Tools.AvaloniaDemo.Models;

public class Pen
{
    public string Color { get; set; }

    public double TipSize { get; set; }

    public string Type { get; set; }

    public bool HasInk { get; set; }

    [JsonIgnore]
    public string Description => $"{Type}: {Color}";
    public Pen(string color, double tipSize, string type, bool hasInk)
    {
        Color = color;
        TipSize = tipSize;
        Type = type;
        HasInk = hasInk;
    }

    public Pen(){}
}