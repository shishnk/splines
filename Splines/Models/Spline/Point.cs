namespace Splines.Models.Spline;

public readonly record struct Point(double X, double Value)
{
    public static Point operator +(Point point, (double X, double Value) v)
         => new(point.X + v.X, point.Value + v.Value);

    public static Point[]? ReadJson(string jsonPath)
    {
        try
        {
            if (!File.Exists(jsonPath))
            {
                throw new Exception("File does not exist");
            }

            using var sr = new StreamReader(jsonPath);
            return JsonConvert.DeserializeObject<Point[]>(sr.ReadToEnd());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"We had problem: {ex.Message}");
            return null;
        }
    }

    public override string ToString() => $"({X},{Value}";
}