namespace SplinesConsoleVersion;

public static class Integration
{
    public static double GaussOrder5(Func<double, double, double> psi1, Func<double, double, double> psi2, double a, double b)
    {
        double h = Math.Abs(b - a);
        double result = Quadratures.GaussOrder5().Sum(q => q.Weight * psi1((a + b + q.Node * h) / 2.0, h) * psi2((a + b + q.Node * h) / 2.0, h));

        return result * h / 2.0;
    }
}