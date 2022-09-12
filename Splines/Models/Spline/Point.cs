namespace Splines.Models.Spline;

public class Point
{
    public double X { get; set; }
    public double Value { get; set; }

    public Point(double x, double value) => (X, Value) = (x, value);

    public static Point operator +(Point point, (double X, double Value) v)
        => new(point.X + v.X, point.Value + v.Value);

    public static Point[] ReadJson(string jsonPath)
    {
        try
        {
            if (!File.Exists(jsonPath))
            {
                throw new Exception("File does not exist");
            }

            using var sr = new StreamReader(jsonPath);
            return JsonConvert.DeserializeObject<Point[]>(sr.ReadToEnd()) ?? Array.Empty<Point>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"We had problem: {ex.Message}");
            throw;
        }
    }

    public override string ToString() => $"({X},{Value}";
}