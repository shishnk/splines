namespace Splines.Models.Spline;

public class Matrix
{
    private readonly double[,] _storage;
    public int Size { get; }

    public double this[int i, int j]
    {
        get => _storage[i, j];
        set => _storage[i, j] = value;
    }

    public Matrix(int size)
    {
        Size = size;
        _storage = new double[size, size];
    }

    public void PrintDense(string path)
    {
        using var sw = new StreamWriter(path);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                sw.Write($"{_storage[i, j]:0.000} \t\t");
            }

            sw.WriteLine();
        }
    }

    public void LU()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                double suml = 0.0;
                double sumu = 0.0;

                if (i < j)
                {
                    for (int k = 0; k < i; k++)
                    {
                        sumu += _storage[i, k] * _storage[k, j];
                    }

                    _storage[i, j] = (_storage[i, j] - sumu) / _storage[i, i];
                }
                else
                {
                    for (int k = 0; k < j; k++)
                    {
                        suml += _storage[i, k] * _storage[k, j];
                    }

                    _storage[i, j] -= suml;
                }
            }
        }
    }
}