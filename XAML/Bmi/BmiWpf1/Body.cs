using System;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace BmiWpf1
{
    public class Body
    {
        public ReactiveProperty<double> Height { get; } = new ReactiveProperty<double>(170.0);
        public ReactiveProperty<double> Weight { get; } = new ReactiveProperty<double>(70.0);
        public ReadOnlyReactiveProperty<double> Bmi { get; }

        public Body()
        {
            Bmi = Height.Merge(Weight)
                .Select(_ => Weight.Value / Math.Pow(Height.Value / 100, 2))
                //.Throttle(TimeSpan.FromSeconds(0.1))
                .ToReadOnlyReactiveProperty();
        }
    }
}
