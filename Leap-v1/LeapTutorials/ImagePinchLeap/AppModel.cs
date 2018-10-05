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
                .Select(h => h?.IsValid == true && false) // Implement Pinch
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

        // StabilizedPalmPosition and StabilizedTipPosition are lazy.
        static Point3D GetPosition(Leap.Hand hand)
        {
            return ToPoint3D(hand.PalmPosition);
        }

        static Point3D ToPoint3D(Leap.Vector v) => new Point3D(v.x, v.y, v.z);
        static Vector3D ToVector3D(Leap.Vector v) => new Vector3D(v.x, v.y, v.z);
    }
}
