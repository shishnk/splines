using System.Numerics;

namespace SplinesConsoleVersion;

public static class SLAE
{
    public static Vector<T> Compute<T>(Matrix matrix, Vector<T> f)
        where T : INumber<T>
    {
        Vector<T> x = new(f.Length);
        Vector<T>.Copy(f, x);

        for (int i = 0; i < f.Length; i++)
        {
            T sum = T.Zero;

            for (int k = 0; k < i; k++)
                sum += T.CreateChecked(matrix[i, k]) * x[k];

            x[i] = (f[i] - sum) / T.CreateChecked(matrix[i, i]);
        }

        for (int i = x.Length - 1; i >= 0; i--)
        {
            T sum = T.Zero;

            for (int k = i + 1; k < x.Length; k++)
            {
                sum += T.CreateChecked(matrix[i, k]) * x[k];
            }

            x[i] -= sum;
        }

        return x;
    }
}