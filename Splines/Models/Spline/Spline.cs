using System.IO;

namespace Splines.Models.Spline;

public class Spline
{
    public delegate double Basis(double x, double h);
    private readonly Basis[] _basis, _dBasis, _ddBasis;
    private readonly FiniteElement[] _elements;
    private readonly Point[] _points;
    private Matrix _matrix;
    private Vector<double> _vector;
    private List<Point> _result;
    private Integration _integration;
    private double _alpha;
    private double _beta;

    public Spline(string pathElements, string pathPoints, string pathParameters)
    {
        try
        {
            using (var sr = new StreamReader(pathElements))
            {
                _elements = sr.ReadToEnd().Split("\n").Select(stringElements => stringElements.Split())
                .Select(element => new FiniteElement(double.Parse(element[0]), double.Parse(element[1]))).ToArray();
            }

            using (var sr = new StreamReader(pathPoints))
            {
                _points = sr.ReadToEnd().Split("\n").Select(stringPoints => stringPoints.Split())
                .Select(point => new Point(double.Parse(point[0]), double.Parse(point[1]))).ToArray();
            }

            using (var sr = new StreamReader(pathParameters))
            {
                _alpha = double.Parse(sr.ReadLine());
                _beta = double.Parse(sr.ReadLine());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        _matrix = new(_elements.Length * 2 + 2);
        _vector = new(_matrix.Size);
        _integration = new();
        _result = new();

        _basis = new Basis[4]{HermiteBasis.Psi1, HermiteBasis.Psi2,
                                         HermiteBasis.Psi3, HermiteBasis.Psi4};
        _dBasis = new Basis[4]{HermiteBasis.dPsi1, HermiteBasis.dPsi2,
                                          HermiteBasis.dPsi3, HermiteBasis.dPsi4};
        _ddBasis = new Basis[4]{HermiteBasis.ddPsi1, HermiteBasis.ddPsi2,
                                           HermiteBasis.ddPsi3, HermiteBasis.ddPsi4};
    }

    public void Compute()
    {
        Assembly();
        _matrix.PrintDense("_matrix.txt");
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
            for (int ipoint = 0; ipoint < _points.Length; ipoint++)
                if (_elements[ielem].Contain(_points[ipoint]) && checker[ipoint] == 1)
                {
                    checker[ipoint] = -1;
                    x = (_points[ipoint].X - _elements[ielem].LeftBorder) / _elements[ielem].Lenght; // $\xi(x) = \dfrac{x - x_i}{h_i}$

                    for (int i = 0; i < _basis.Length; i++)
                    {
                        _vector[2 * ielem + i] += _points[ipoint].Value * _basis[i](x, _elements[ielem].Lenght);

                        for (int j = 0; j < _basis.Length; j++)
                            _matrix[2 * ielem + i, 2 * ielem + j] +=
                            _basis[i](x, _elements[ielem].Lenght) * _basis[j](x, _elements[ielem].Lenght) +
                            _alpha * _integration.GaussOrder5(_dBasis[i].Invoke, _dBasis[j].Invoke,
                            _elements[ielem].LeftBorder, _elements[ielem].RightBorder) +
                            _beta * _integration.GaussOrder5(_ddBasis[i].Invoke, _ddBasis[j].Invoke,
                            _elements[ielem].LeftBorder, _elements[ielem].RightBorder);
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