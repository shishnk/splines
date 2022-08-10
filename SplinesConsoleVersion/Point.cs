namespace SplinesConsoleVersion;

public readonly record struct Point(double X, double Value)
{
    public static Point operator +(Point point, (double X, double Value) v)
         => new(point.X + v.X, point.Value + v.Value);

    public override string ToString() => $"({X},{Value}";
}