using System;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace BmiWpf2
{
    public class MainViewModel
    {
        public Body Body { get; } = new Body();

        public ReadOnlyReactiveProperty<double> BmiRectangleWidth { get; }
        public ReadOnlyReactiveProperty<string> BmiRectangleFill { get; }

        public MainViewModel()
        {
            BmiRectangleWidth = Body.Bmi
                .Select(x => 5 * x)
                .ToReadOnlyReactiveProperty();
            BmiRectangleFill = Body.Bmi
                .Select(x => x < 25 ? "#FF009900" : x < 40 ? "#FFDDDD00" : "#FFEE0000")
                .ToReadOnlyReactiveProperty();
        }
    }
}
