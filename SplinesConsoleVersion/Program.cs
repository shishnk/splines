using SplinesConsoleVersion;

Spline spline = Spline.CreateBuilder()
    .SetElements(FiniteElement.ReadJson("input/elements.json"))
    .SetPoints(Point.ReadJson("input/points.json")!)
    .SetParameters((1E-07, 1E-07));

spline.Compute();