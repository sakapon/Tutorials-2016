using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Reactive.Bindings;

namespace ImagePinchLeap
{
    public class MainViewModel
    {
        public AppModel AppModel { get; } = new AppModel();

        public ReactiveProperty<Point> Translation { get; } = new ReactiveProperty<Point>(new Point());
        public ReactiveProperty<double> Scale { get; } = new ReactiveProperty<double>(1.0);

        public MainViewModel()
        {
            AppModel.PinchDrag
                .Select(d => new { p0 = Translation.Value, d })
                .Subscribe(_ => _.d.Subscribe(v => Translation.Value = _.p0 + 5 * ToVector2DForScreen(v)));
            AppModel.PinchDrag
                .Select(d => new { s0 = Scale.Value, d })
                .Subscribe(_ => _.d.Subscribe(v => Scale.Value = _.s0 * Math.Pow(2, 0.02 * v.Z)));
        }

        static Vector ToVector2DForScreen(Vector3D v) => new Vector(v.X, -v.Y);
    }
}
