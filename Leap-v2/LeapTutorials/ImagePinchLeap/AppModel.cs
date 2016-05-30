using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Monsoon.Setup.Leap;
using Reactive.Bindings;

namespace ImagePinchLeap
{
    public class AppModel
    {
        public LeapManager LeapManager { get; } = new LeapManager();

        public ReadOnlyReactiveProperty<Leap.Hand> FrontHand { get; }
        public ReadOnlyReactiveProperty<double?> PinchStrength { get; }
        public ReadOnlyReactiveProperty<bool?> IsPinched { get; }

        public IObservable<IObservable<Vector3D>> PinchDrag { get; }

        public ReactiveProperty<Point> Translation { get; } = new ReactiveProperty<Point>(new Point());
        public ReactiveProperty<double> Scale { get; } = new ReactiveProperty<double>(1.0);

        public AppModel()
        {
            FrontHand = LeapManager.FrameArrived
                .Select(f => f.Hands.Frontmost)
                .ToReadOnlyReactiveProperty();
            PinchStrength = FrontHand
                .Select(h => h?.IsValid == true ? h.PinchStrength : default(double?))
                .ToReadOnlyReactiveProperty();
            IsPinched = PinchStrength
                .Select(v => v.HasValue ? v.Value == 1.0 : default(bool?))
                .ToReadOnlyReactiveProperty();

            PinchDrag = IsPinched
                .Where(b => b == true)
                .Select(_ => GetPosition(FrontHand.Value))
                .Select(p0 => FrontHand
                    .TakeWhile(_ => IsPinched.Value == true)
                    .Select(_ => GetPosition(FrontHand.Value) - p0));

            PinchDrag
                .Select(d => new { p0 = Translation.Value, d })
                .Subscribe(_ => _.d.Subscribe(v => Translation.Value = _.p0 + 5 * ToVector2DForScreen(v)));
            PinchDrag
                .Select(d => new { s0 = Scale.Value, d })
                .Subscribe(_ => _.d.Subscribe(v => Scale.Value = _.s0 * (1.0 + 0.005 * v.Z)));
        }

        // StabilizedPalmPosition and StabilizedTipPosition are lazy.
        static Point3D GetPosition0(Leap.Hand hand)
        {
            return ToPoint3D(hand.PalmPosition);
        }

        static Point3D GetPosition(Leap.Hand hand)
        {
            var thumb = hand.Fingers.FirstOrDefault(f => f.Type == Leap.Finger.FingerType.TYPE_THUMB);
            return ToPoint3D(thumb.TipPosition);
        }

        static Point3D ToPoint3D(Leap.Vector v) => new Point3D(v.x, v.y, v.z);
        static Vector3D ToVector3D(Leap.Vector v) => new Vector3D(v.x, v.y, v.z);

        static Vector ToVector2DForScreen(Vector3D v) => new Vector(v.X, -v.Y);
    }
}
