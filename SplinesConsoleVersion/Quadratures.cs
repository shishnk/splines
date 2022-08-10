namespace SplinesConsoleVersion;

public class QuadratureNode<T> where T : notnull {
    public T Node { get; init; }
    public double Weight { get; init; }

    public QuadratureNode(T node, double weight) {
        Node = node;
        Weight = weight;
    }
}

public class Quadrature<T> where T : notnull {
    private readonly QuadratureNode<T>[] _nodes = default!;
    public ImmutableArray<QuadratureNode<T>> Nodes => _nodes.ToImmutableArray();

    public Quadrature(QuadratureNode<T>[] nodes)
        => _nodes = nodes;
}

public static class Quadratures {
    public static IEnumerable<QuadratureNode<double>> SegmentGaussOrder9() {
        const int n = 5;
        double[] points = { -1.0 / 3.0 * Math.Sqrt(5 + (2 * Math.Sqrt(10.0 / 7.0))),
                            1.0 / 3.0 * Math.Sqrt(5 - (2 * Math.Sqrt(10.0 / 7.0))),
                            0.0,
                            -1.0 / 3.0 * Math.Sqrt(5 - (2 * Math.Sqrt(10.0 / 7.0))),
                            1.0 / 3.0 * Math.Sqrt(5 + (2 * Math.Sqrt(10.0 / 7.0)))};

        double[] weights = { (322.0 - (13.0 * Math.Sqrt(70.0))) / 900.0, (322.0 + (13.0 * Math.Sqrt(70.0))) / 900.0,
                              128.0 / 225.0,
                             (322.0 + (13.0 * Math.Sqrt(70.0))) / 900.0,
                             (322.0 - (13.0 * Math.Sqrt(70.0))) / 900.0 };

        for (int i = 0; i < n; i++) {
            yield return new QuadratureNode<double>(points[i], weights[i]);
        }
    }
}