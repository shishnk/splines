namespace Splines.Models.Spline;

public class Spline : IDataService
{
    public class SplineBuilder
    {
        private readonly Spline _spline = new();

        public SplineBuilder SetParameters((double Alpha, double Beta) parameters)
        {
            _spline._parameters = parameters;
            return this;
        }

        public SplineBuilder SetElements(FiniteElement[] elements)
        {
            _spline._elements = elements;
            return this;
        }

        public static implicit operator Spline(SplineBuilder builder)
            => builder._spline;
    }

    private delegate double Basis(double x, double h);
    private Basis[] _basis = default!, _dBasis = default!, _ddBasis = default!;
    private FiniteElement[] _elements = default!;
    private Matrix _matrix = default!;
    private Vector<double> _vector = default!;
    private List<Point> _result = default!;
    private (double Alpha, double Beta) _parameters;
    public IEnumerable<Point> GetData() => _result ??
                                           throw new ArgumentNullException("Точки для построения сплайна не были сформированы");

    private void Init()
    {
        _matrix = new((_elements.Length * 2) + 2);
        _vector = new(_matrix.Size);
        _result = new();

        _basis = new Basis[]{HermiteBasis.Psi1, HermiteBasis.Psi2,
                                         HermiteBasis.Psi3, HermiteBasis.Psi4};
        _dBasis = new Basis[]{HermiteBasis.DPsi1, HermiteBasis.DPsi2,
                                          HermiteBasis.DPsi3, HermiteBasis.DPsi4};
        _ddBasis = new Basis[]{HermiteBasis.DdPsi1, HermiteBasis.DdPsi2,
                                           HermiteBasis.DdPsi3, HermiteBasis.DdPsi4};
    }

    public void Compute()
    {
        Init();
        AssemblyMatrix();
        //_matrix.PrintDense("_matrix.txt");
        _matrix.LU();
        _vector = SLAE.Compute(_matrix, _vector);
        ValuesAtPoints();
    }

    private void AssemblyMatrix()
    {
        int[] checker = new int[_elements.Select(element => element.Points).Count()];
        checker.Fill(1);

        for (int ielem = 0; ielem < _elements.Length; ielem++)
        {
            for (int ipoint = 0; ipoint < _elements[ielem].Points!.Count(); ipoint++)
            {
                if (!_elements[ielem].Contain(_elements[ielem].PointsAsArray![ipoint]) || checker[ipoint] != 1) continue;

                checker[ipoint] = -1;
                double x = (_elements[ielem].PointsAsArray![ipoint].X - _elements[ielem].LeftBorder) / _elements[ielem].Lenght;

                for (int i = 0; i < _basis.Length; i++)
                {
                    _vector[2 * ielem + i] += _elements[ielem].PointsAsArray![ipoint].Value * _basis[i](x, _elements[ielem].Lenght);

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

                sum += _basis.Select((t, i) => _vector[(2 * ielem) + i] * t(x, _elements[ielem].Lenght)).Sum();
                _result.Add(changedPoint with { Value = sum });

                changedPoint += (0.2, 0.0);
                sum = 0.0;
            } while (_elements[ielem].Contain(changedPoint));
        }
    }

    public static SplineBuilder CreateBuilder()
        => new();
}