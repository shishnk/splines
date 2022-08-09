namespace Splines.Models.Spline;

public class Integration
{
    private readonly double[] _points;
    private readonly double[] _weights;

    public Integration()
    {
        _points = new double[3];
        _weights = new double[3];

        _points[0] = 0.0;
        _points[1] = Math.Sqrt(3.0 / 5);
        _points[2] = -Math.Sqrt(3.0 / 5);

        _weights[0] = 8.0 / 9;
        _weights[1] = 5.0 / 9;
        _weights[2] = 5.0 / 9;
    }

    public double GaussOrder5(Func<double, double, double> fstPsi, Func<double, double, double> sndPsi, double x1, double x2)
    {
        double h = 0.0;
        double result = 0.0;

        for (int i = 0; i < 3; i++)
        {
            double qi = _weights[i];
            h = Math.Abs(x2 - x1);
            double pi = (x1 + x2 + _points[i] * h) / 2.0;

            result += qi * fstPsi(pi, h) * sndPsi(pi, h);
        }

        return result * h / 2.0;
    }
}