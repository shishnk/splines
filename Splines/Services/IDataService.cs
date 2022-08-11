global using Point = Splines.Models.Spline.Point;

namespace Splines.Services;

public interface IDataService
{
    public IEnumerable<Point> GetData();
}
