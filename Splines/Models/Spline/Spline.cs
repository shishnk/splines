namespace Splines.Models.Spline;

public class Spline
{
    public class SplineBuilder
    {
        private readonly Spline _spline = new();

        public SplineBuilder SetParameters((double Alpha, double Beta) parameters)
        {
            _spline._parameters = parameters;
            return this;
        }

        public SplineBuilder SetPartitions(int partitions)
        {
            _spline._partitions = partitions;
            return this;
        }

        public SplineBuilder SetPoints(Point[] points)
        {
            _spline._points = points;
            return this;
        }

        public static implicit operator Spline(SplineBuilder builder)
            => builder._spline;
    }

    private delegate double Basis(double x, double h);

    private Basis[] _basis = default!, _dBasis = default!, _ddBasis = default!;
    private Point[] _points = default!;
    private FiniteElement[] _elements = default!;
    private Matrix _matrix = default!;
    private Vector<double> _vector = default!;
    private List<Point> _result = default!;
    private (double Alpha, double Beta) _parameters;
    private int _partitions;

    public List<Point> Result => _result;

    private void Init()
    {
        _matrix = new(_elements.Length * 2 + 2);
        _vector = new(_matrix.Size);
        _result = new();

        _basis = new Basis[]
        {
            HermiteBasis.Psi1, HermiteBasis.Psi2,
            HermiteBasis.Psi3, HermiteBasis.Psi4
        };
        _dBasis = new Basis[]
        {
            HermiteBasis.DPsi1, HermiteBasis.DPsi2,
            HermiteBasis.DPsi3, HermiteBasis.DPsi4
        };
        _ddBasis = new Basis[]
        {
            HermiteBasis.DdPsi1, HermiteBasis.DdPsi2,
            HermiteBasis.DdPsi3, HermiteBasis.DdPsi4
        };
    }

    private void FormingElements()
    {
        _elements = new FiniteElement[_partitions];
        _points = _points.OrderBy(p => p.X).ToArray();

        if (_partitions == 1)
        {
            _elements[0] = new(_points[0].X, _points[^1].X);
            return;
        }

        double step = (_points.MaxBy(p => p.X)!.X - _points.MinBy(p => p.X)!.X) / _partitions;
        _elements[0] = new(_points[0].X, _points[0].X + step);

        for (int i = 1; i < _elements.Length; i++)
        {
            _elements[i] = new(_elements[i - 1].RightBorder, _elements[i - 1].RightBorder + step);
        }
    }

    public void Compute()
    {
        FormingElements();
        Init();
        AssemblyMatrix();
        //_matrix.PrintDense("_matrix.txt");
        _matrix.LU();
        _vector = SLAE.Compute(_matrix, _vector);
        ValuesAtPoints();
    }

    private void AssemblyMatrix()
    {
        int[] checker = new int[_points.Length];
        checker.Fill(1);

        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            for (int ipoint = 0; ipoint < _points.Length; ipoint++)
            {
                if (!_elements[ielem].Contain(_points[ipoint]) ||
                    checker[ipoint] != 1) continue;

                checker[ipoint] = -1;
                double x = (_points[ipoint].X - _elements[ielem].LeftBorder) /
                           _elements[ielem].Lenght;

                for (int i = 0; i < _basis.Length; i++)
                {
                    _vector[2 * ielem + i] += _points[ipoint].Value *
                                              _basis[i](x, _elements[ielem].Lenght);

                    for (int j = 0; j < _basis.Length; j++)
                    {
                        _matrix[2 * ielem + i, 2 * ielem + j] +=
                            _basis[i](x, _elements[ielem].Lenght) * _basis[j](x, _elements[ielem].Lenght) +
                            _parameters.Alpha * Integration.GaussOrder5(_dBasis[i].Invoke, _dBasis[j].Invoke,
                                _elements[ielem].LeftBorder, _elements[ielem].RightBorder) +
                            _parameters.Beta * Integration.GaussOrder5(_ddBasis[i].Invoke, _ddBasis[j].Invoke,
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

    public static SplineBuilder CreateBuilder()
        => new();
}