using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Media3D;
using Monsoon.Reactive.Leap;
using Reactive.Bindings;

namespace ImagePinchLeap
{
    public class AppModel
    {
        public LeapManager LeapManager { get; } = new LeapManager();

        public ReadOnlyReactiveProperty<Leap.Hand> FrontHand { get; }
        public ReadOnlyReactiveProperty<bool> IsPinched { get; }

        public IObservable<IObservable<Vector3D>> PinchDrag { get; }
        public ReactiveProperty<Vector3D> DraggedDelta { get; } = new ReactiveProperty<Vector3D>(new Vector3D());

        public AppModel()
        {
            FrontHand = LeapManager.FrameArrived
                .Select(f => f.Hands.Frontmost)
                .ToReadOnlyReactiveProperty();
            IsPinched = FrontHand
                .Select(GetIsPinched)
                .Where(b => b.HasValue)
                .Select(b => b.Value)
                .ToReadOnlyReactiveProperty();

            PinchDrag = IsPinched
                .Where(b => b == true)
                .Select(_ => GetPosition(FrontHand.Value))
                .Select(p0 => FrontHand
                    .TakeWhile(_ => IsPinched.Value == true)
                    .Select(_ => GetPosition(FrontHand.Value) - p0));
            PinchDrag
                .Select(d => new { v0 = DraggedDelta.Value, d })
                .Subscribe(_ => _.d.Subscribe(v => DraggedDelta.Value = _.v0 + v));
        }

        const double PinchRange = 25; // in millimeters

        static bool? GetIsPinched(Leap.Hand hand)
        {
            if (!hand.IsValid) return false;
            if (hand.Pointables.Count < 2) return null;

            var pointable0 = hand.Pointables[0];
            var pointable1 = hand.Pointables[1];
            if (!pointable0.IsValid || !pointable1.IsValid) return null;

            var p0 = ToPoint3D(pointable0.TipPosition);
            var p1 = ToPoint3D(pointable1.TipPosition);
            return (p1 - p0).Length < PinchRange;
        }

        // StabilizedPalmPosition and StabilizedTipPosition are lazy.
        static Point3D GetPosition(Leap.Hand hand)
        {
            return ToPoint3D(hand.PalmPosition);
        }

        static Point3D ToPoint3D(Leap.Vector v) => new Point3D(v.x, v.y, v.z);
        static Vector3D ToVector3D(Leap.Vector v) => new Vector3D(v.x, v.y, v.z);
    }
}
