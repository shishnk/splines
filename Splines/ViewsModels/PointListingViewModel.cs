using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

namespace Splines.ViewsModels;

public class PointListingViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<PointWrapper> _points;
    private ReactiveCommand<double, Unit> DeletePoint { get; }
    private ReactiveCommand<double, Unit> InsertPoint { get; }

    public ReadOnlyObservableCollection<PointWrapper> Points => _points;
    public SourceCache<PointWrapper, double> PointsAsSourceCache { get; } = new(p => p.X);

    public PointListingViewModel()
    {
        DeletePoint = ReactiveCommand.Create<double>(parameter => PointsAsSourceCache.Remove(parameter),
            PointsAsSourceCache.CountChanged.Select(p => p > 1));
        InsertPoint = ReactiveCommand.Create<double>(InsertPointImpl);

        PointsAsSourceCache.AddOrUpdate(new PointWrapper(new(0.0, 0.0))
        {
            DeletePoint = DeletePoint,
            InsertPoint = InsertPoint
        });
        PointsAsSourceCache.Connect().SortBy(p => p.X).Bind(out _points).Subscribe();
    }

    private void InsertPointImpl(double parameter)
    {
        var keys = PointsAsSourceCache.Keys.OrderBy(key => key).ToArray();
        var res = keys.BinarySearch(parameter);

        if (res < 0)
        {
            PointsAsSourceCache.AddOrUpdate(new PointWrapper(new(0.0, 0.0))
            {
                DeletePoint = DeletePoint,
                InsertPoint = InsertPoint
            });

            return;
        }

        double newValue;

        if (res == 0)
        {
            newValue = keys[0] - 1;
        }
        else if (res == keys.Length - 1)
        {
            newValue = keys[^1] + 1;
        }
        else
        {
            newValue = (keys[res] + keys[res + 1]) / 2.0;
        }

        PointsAsSourceCache.AddOrUpdate(new PointWrapper(new(newValue, 0.0))
        {
            DeletePoint = DeletePoint,
            InsertPoint = InsertPoint
        });
    }
}