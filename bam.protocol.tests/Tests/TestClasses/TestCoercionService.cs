namespace Bam.Protocol.Tests;

public enum TestWidgetKind
{
    None = 0,
    Gadget = 1,
    Gizmo = 2
}

public class TestWidget
{
    public string Name { get; set; } = null!;
    public int Count { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}

public class TestCoercionService
{
    public string Describe(TestWidget widget, int times, TestWidgetKind kind, string? note = null)
    {
        return $"{widget.Name} x{times} [{string.Join(",", widget.Tags)}] kind={kind} count={widget.Count} note={note ?? "<none>"}";
    }

    public string Identify(Guid id, DateTimeOffset at, long big, double ratio, bool flag)
    {
        return $"{id:D}|{at:O}|{big}|{ratio.ToString(System.Globalization.CultureInfo.InvariantCulture)}|{flag}";
    }

    public int Sum(int[] values)
    {
        return values.Sum();
    }
}
