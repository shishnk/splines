using System.Numerics;

namespace Splines.Models.Spline;

public class Vector<T> where T : INumber<T>
{
    private readonly T[] _storage;
    public int Length { get; }

    public T this[int index]
    {
        get => _storage[index];
        set => _storage[index] = value;
    }

    public Vector(int size)
    {
        _storage = new T[size];
        Length = size;
    }

    public static T operator *(Vector<T> a, Vector<T> b)
    {
        T result = T.Zero;

        for (int i = 0; i < a.Length; i++)
        {
            result += a[i] * b[i];
        }

        return result;
    }

    public static Vector<T> operator *(double constant, Vector<T> vector)
    {
        Vector<T> result = new(vector.Length);

        for (int i = 0; i < vector.Length; i++)
        {
            result[i] = vector[i] * T.CreateChecked(constant);
        }

        return result;
    }

    public static Vector<T> operator +(Vector<T> a, Vector<T> b)
    {
        Vector<T> result = new(a.Length);

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }

        return result;
    }

    public static Vector<T> operator -(Vector<T> a, Vector<T> b)
    {
        Vector<T> result = new(a.Length);

        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] - b[i];
        }

        return result;
    }

    public static void Copy(Vector<T> source, Vector<T> destination)
    {
        for (int i = 0; i < source.Length; i++)
        {
            destination[i] = source[i];
        }
    }
}