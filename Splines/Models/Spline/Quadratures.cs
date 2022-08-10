namespace SplinesConsoleVersion;

public class QuadratureNode<T> where T : notnull
{
    public T Node { get; }
    public double Weight { get; }

    public QuadratureNode(T node, double weight)
    {
        Node = node;
        Weight = weight;
    }
}

public static class Quadratures
{
    public static IEnumerable<QuadratureNode<double>> GaussOrder5()
    {
        const int n = 3;

        double[] points = { 0.0, Math.Sqrt(3.0 / 5.0), -Math.Sqrt(3.0 / 5.0) };
        double[] weights = { 8.0 / 9.0, 5.0 / 9.0, 5.0 / 9.0 };

        for (int i = 0; i < n; i++)
        {
            yield return new QuadratureNode<double>(points[i], weights[i]);
        }
    }
}