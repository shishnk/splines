namespace SplinesConsoleVersion;

public readonly record struct FiniteElement
{
    [JsonProperty("Left Border")]
    public double LeftBorder { get; }

    [JsonProperty("Right Border")]
    public double RightBorder { get; }

    [JsonIgnore]
    public double Lenght { get; }

    [JsonConstructor]
    public FiniteElement(double leftBorder, double rightBorder)
    {
        (LeftBorder, RightBorder) = (leftBorder, rightBorder);
        Lenght = Math.Abs(rightBorder - leftBorder);
    }

    public static FiniteElement[]? ReadJson(string jsonPath)
    {
        try
        {
            if (!File.Exists(jsonPath))
            {
                throw new Exception("File does not exist");
            }

            using var sr = new StreamReader(jsonPath);
            return JsonConvert.DeserializeObject<FiniteElement[]>(sr.ReadToEnd());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"We had problem: {ex.Message}");
            return null;
        }
    }

    public bool Contain(Point point)
        => point.X >= LeftBorder && point.X <= RightBorder;

    public override string ToString()
        => $"Element interval is [{LeftBorder}, {RightBorder}]";
}