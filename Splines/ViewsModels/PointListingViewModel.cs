using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Splines.ViewsModels;

public class PointListingViewModel : ReactiveObject
{
    private readonly ReadOnlyObservableCollection<PointWrapper> _points;
    private (double, double) _watch;
    private ReactiveCommand<double, Unit> DeletePoint { get; }
    private ReactiveCommand<double, Unit> InsertPoint { get; }

    [Reactive] public PointWrapper? SelectedPoint { get; set; }
    public ReadOnlyObservableCollection<PointWrapper> Points => _points;
    public SourceCache<PointWrapper, double> PointsAsSourceCache { get; } = new(p => p.X);

    public PointListingViewModel()
    {
        DeletePoint = ReactiveCommand.Create<double>(parameter =>
            {
                PointsAsSourceCache.Edit(updater =>
                {
                    var pnt = updater.Lookup(_watch.Item1);
                    updater.RemoveKey(_watch.Item1);
                    updater.AddOrUpdate(pnt.Value);
                    updater.RemoveKey(parameter);
                });
            },
            PointsAsSourceCache.CountChanged.Select(p => p > 1));
        InsertPoint = ReactiveCommand.Create<double>(InsertPointImpl);

        PointsAsSourceCache.AddOrUpdate(new PointWrapper(new(0.0, 0.0))
        {
            DeletePoint = DeletePoint,
            InsertPoint = InsertPoint
        });

        PointsAsSourceCache.Connect().SortBy(p => p.X).Bind(out _points).Subscribe();
        this.WhenAnyValue(t => t.SelectedPoint)
            .WhereNotNull()
            .Subscribe(point => _watch = (point.X, point.Value));
    }

    private void InsertPointImpl(double parameter)
    {
        PointsAsSourceCache.Edit(updater =>
        {
            var pnt = updater.Lookup(_watch.Item1);
            updater.RemoveKey(_watch.Item1);
            updater.AddOrUpdate(pnt.Value);
        });

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
            newValue = keys[0] - 1.0;
        }
        else if (res == keys.Length - 1)
        {
            newValue = keys[^1] + 1.0;
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