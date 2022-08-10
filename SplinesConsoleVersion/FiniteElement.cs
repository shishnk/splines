namespace SplinesConsoleVersion;

public readonly record struct FiniteElement(double LeftBorder, double RightBorder)
{
    public double Lenght { get; } = Math.Abs(RightBorder - LeftBorder);

    public bool Contain(Point point)
        => point.X >= LeftBorder && point.X <= RightBorder;

    public override string ToString()
        => $"Element interval is [{LeftBorder}, {RightBorder}]";
}