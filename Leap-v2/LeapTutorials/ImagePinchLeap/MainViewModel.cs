using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Reactive.Bindings;

namespace ImagePinchLeap
{
    public class MainViewModel
    {
        public AppModel AppModel { get; } = new AppModel();

        public ReadOnlyReactiveProperty<Vector> Translation { get; }
        public ReadOnlyReactiveProperty<double> Scale { get; }
        public ReadOnlyReactiveProperty<Point> TransformOrigin { get; }

        public MainViewModel()
        {
            Translation = AppModel.DraggedDelta
                .Select(v => 5 * new Vector(v.X, -v.Y))
                .ToReadOnlyReactiveProperty();
            Scale = AppModel.DraggedDelta
                .Select(v => Math.Pow(2, 0.02 * v.Z))
                .ToReadOnlyReactiveProperty();
            TransformOrigin = Translation
                .Select(v => new Point((2880 - v.X) / 5760, (1800 - v.Y) / 3600))
                .ToReadOnlyReactiveProperty();
        }
    }
}
