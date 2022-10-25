namespace Splines.Models.Spline;

public class Spline
{
    private delegate double Basis(double x, double h);

    private FiniteElement[] _elements = default!;
    private Matrix _matrix = default!;
    private Vector<double> _vector = default!;
    private List<Point> _result = default!;

    private readonly Basis[] _basis =
    {
        HermiteBasis.Psi1, HermiteBasis.Psi2,
        HermiteBasis.Psi3, HermiteBasis.Psi4
    };

    private readonly Basis[] _dBasis =
    {
        HermiteBasis.DPsi1, HermiteBasis.DPsi2,
        HermiteBasis.DPsi3, HermiteBasis.DPsi4
    };

    private readonly Basis[] _ddBasis =
    {
        HermiteBasis.DdPsi1, HermiteBasis.DdPsi2,
        HermiteBasis.DdPsi3, HermiteBasis.DdPsi4
    };

    public (double Alpha, double Beta) Parameters { get; set; }
    public int Partitions { get; set; }
    public Point[] Points { get; set; } = default!;
    public IEnumerable<Point> Result => _result;

    public void Compute()
    {
        FormingElements();
        Init();
        AssemblyMatrix();
        //_matrix.PrintAsDense("_matrix.txt");
        _matrix.LU();
        _vector = SLAE.Compute(_matrix, _vector);
        ValuesAtPoints();
    }

    private void Init()
    {
        _matrix = new(_elements.Length * 2 + 2);
        _vector = new(_matrix.Size);
        _result = new();
    }

    private void FormingElements()
    {
        _elements = new FiniteElement[Partitions];
        Points = Points.OrderBy(p => p.X).ToArray();

        if (Partitions == 1)
        {
            _elements[0] = new(Points[0].X, Points[^1].X);
            return;
        }

        double step = (Points.MaxBy(p => p.X)!.X - Points.MinBy(p => p.X)!.X) / Partitions;
        _elements[0] = new(Points[0].X, Points[0].X + step);

        for (int i = 1; i < _elements.Length; i++)
        {
            _elements[i] = new(_elements[i - 1].RightBorder, _elements[i - 1].RightBorder + step);
        }
    }

    private void AssemblyMatrix()
    {
        int[] checker = new int[Points.Length];
        checker.Fill(1);
        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            for (int ipoint = 0; ipoint < Points.Length; ipoint++)
            {
                if (!_elements[ielem].Contain(Points[ipoint]) ||
                    checker[ipoint] != 1) continue;
                checker[ipoint] = -1;
                double x = (Points[ipoint].X - _elements[ielem].LeftBorder) /
                           _elements[ielem].Lenght;
                for (int i = 0; i < _basis.Length; i++)
                {
                    _vector[2 * ielem + i] += Points[ipoint].Value *
                                              _basis[i](x, _elements[ielem].Lenght);
                    for (int j = 0; j < _basis.Length; j++)
                    {
                        _matrix[2 * ielem + i, 2 * ielem + j] +=
                            _basis[i](x, _elements[ielem].Lenght) * _basis[j](x, _elements[ielem].Lenght) +
                            Parameters.Alpha * Integration.GaussOrder5(_dBasis[i].Invoke, _dBasis[j].Invoke,
                                _elements[ielem].LeftBorder, _elements[ielem].RightBorder) +
                            Parameters.Beta * Integration.GaussOrder5(_ddBasis[i].Invoke, _ddBasis[j].Invoke,
                                _elements[ielem].LeftBorder, _elements[ielem].RightBorder);
                    }
                }
            }
        }
    }

    private void ValuesAtPoints()
    {
        double sum = 0.0;
        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            Point changedPoint = new(_elements[ielem].LeftBorder, 0.0);
            do
            {
                double x = (changedPoint.X - _elements[ielem].LeftBorder) / _elements[ielem].Lenght;
                sum += _basis.Select((t, i) => _vector[2 * ielem + i] * t(x, _elements[ielem].Lenght)).Sum();
                _result.Add(new(changedPoint.X, sum));
                changedPoint += (0.05, 0.0);
                sum = 0.0;
            } while (_elements[ielem].Contain(changedPoint));
        }
    }
}