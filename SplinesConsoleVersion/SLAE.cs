namespace SplinesConsoleVersion;

public static class SLAE
{
    public static Vector<double> Compute(Matrix matrix, Vector<double> f)
    {
        Vector<double> x = new(f.Length);
        Vector<double>.Copy(f, x);

        for (int i = 0; i < f.Length; i++)
        {
            double sum = 0;

            for (int k = 0; k < i; k++)
                sum += matrix[i, k] * x[k];

            x[i] = (f[i] - sum) / matrix[i, i];
        }

        for (int i = x.Length - 1; i >= 0; i--)
        {
            double sum = 0;

            for (int k = i + 1; k < x.Length; k++)
                sum += matrix[i, k] * x[k];

            x[i] -= sum;
        }

        return x;
    }
}