namespace SplinesConsoleVersion;

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

        public SplineBuilder SetSpaceGrid(Point[] points)
        {
            _spline._points = points;
            return this;
        }

        public static implicit operator Spline(SplineBuilder builder)
            => builder._spline;
    }

    private delegate double Basis(double x, double h);
    private Basis[] _basis, _dBasis, _ddBasis = default!;
    private FiniteElement[] _elements = default!;
    private Point[] _points = default!;
    private Matrix _matrix = default!;
    private Vector<double> _vector = default!;
    private List<Point> _result = default!;
    private (double Alpha, double Beta) _parameters;

    private void Init()
    {
        _matrix = new(_elements.Length * 2 + 2);
        _vector = new(_matrix.Size);
        _result = new();

        _basis = new Basis[4]{HermiteBasis.Psi1, HermiteBasis.Psi2,
                                         HermiteBasis.Psi3, HermiteBasis.Psi4};
        _dBasis = new Basis[4]{HermiteBasis.DPsi1, HermiteBasis.DPsi2,
                                          HermiteBasis.DPsi3, HermiteBasis.DPsi4};
        _ddBasis = new Basis[4]{HermiteBasis.DDPsi1, HermiteBasis.DDPsi2,
                                           HermiteBasis.DDPsi3, HermiteBasis.DDPsi4};
    }

    public void Compute()
    {
        Init();
        Assembly();
        //_matrix.PrintDense("_matrix.txt");
        _matrix.LU();
        _vector = SLAE.Compute(_matrix, _vector);
        ValueAtPoint();
    }

    private void Assembly()
    {
        int[] checker = new int[_points.Length];
        checker.Fill(1);

        double x;

        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            for (int ipoint = 0; ipoint < _points.Length; ipoint++)
            {
                if (_elements[ielem].Contain(_points[ipoint]) && checker[ipoint] == 1)
                {
                    checker[ipoint] = -1;
                    x = (_points[ipoint].X - _elements[ielem].LeftBorder) /
                        _elements[ielem].Lenght; // $\xi(x) = \dfrac{x - x_i}{h_i}$

                    for (int i = 0; i < _basis.Length; i++)
                    {
                        _vector[2 * ielem + i] += _points[ipoint].Value * _basis[i](x, _elements[ielem].Lenght);

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
    }

    private void ValueAtPoint()
    {
        double x;
        double sum = 0;
        Point changed;

        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            changed = new(_elements[ielem].LeftBorder, 0);

            do
            {
                x = (changed.X - _elements[ielem].LeftBorder) / _elements[ielem].Lenght;

                for (int i = 0; i < _basis.Length; i++)
                    sum += _vector[2 * ielem + i] * _basis[i](x, _elements[ielem].Lenght);

                _result.Add(new(changed.X, sum));
                changed += (0.2, 0);
                sum = 0;

            } while (_elements[ielem].Contain(changed));
        }
    }
}