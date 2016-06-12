using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Monsoon.Reactive.Leap;

namespace AirCanvasLeap
{
    public class AppModel
    {
        const double ScreenWidth = 1920.0;
        const double ScreenHeight = 1080.0;
        const double MappingScale = 3.0;

        public LeapManager LeapManager { get; } = new LeapManager();

        public IObservable<Dictionary<int, StylusPoint>> TouchedPositions { get; }

        public AppModel()
        {
            TouchedPositions = LeapManager.FrameArrived
                .Select(ToTouchedPositions);
        }

        static Dictionary<int, StylusPoint> ToTouchedPositions(Leap.Frame frame) =>
            frame.Pointables
                .Where(p => p.IsValid && p.IsExtended)
                .Where(p => p.TipPosition.IsValid() && p.TipPosition.z < 0.0)
                .ToDictionary(p => p.Id, p => ToStylusPoint(p.TipPosition));

        static StylusPoint ToStylusPoint(Leap.Vector v) =>
            new StylusPoint(ScreenWidth / 2 + MappingScale * v.x, ScreenHeight - MappingScale * v.y);
    }
}
