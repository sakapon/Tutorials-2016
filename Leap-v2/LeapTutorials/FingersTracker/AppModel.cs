using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Media3D;
using Reactive.Bindings;

namespace FingersTracker
{
    public class AppModel
    {
        const double ScreenWidth = 1920.0;
        const double ScreenHeight = 1080.0;
        const double MappingScale = 3.0;

        public LeapManager LeapManager { get; } = new LeapManager();

        public ReadOnlyReactiveProperty<double> FrameRate { get; }
        public ReadOnlyReactiveProperty<Point3D[]> StabilizedTipPositions { get; }

        public AppModel()
        {
            FrameRate = LeapManager.FrameArrived
                .Select(f => (double)f.CurrentFramesPerSecond)
                .ToReadOnlyReactiveProperty();
            StabilizedTipPositions = LeapManager.FrameArrived
                .Select(GetStabilizedTipPositions)
                .ToReadOnlyReactiveProperty();
        }

        static Point3D[] GetStabilizedTipPositions(Leap.Frame frame) =>
            frame.Pointables
                .Where(p => p.IsValid)
                .Where(p => p.StabilizedTipPosition.IsValid())
                .Select(p => ToScreenPoint(p.StabilizedTipPosition))
                .ToArray();

        static Point3D ToScreenPoint(Leap.Vector v) =>
            new Point3D(ScreenWidth / 2 + MappingScale * v.x, ScreenHeight - MappingScale * v.y, MappingScale * v.z);
    }
}
