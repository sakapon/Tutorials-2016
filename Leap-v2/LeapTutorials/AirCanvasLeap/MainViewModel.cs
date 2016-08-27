using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Ink;
using System.Windows.Input;

namespace AirCanvasLeap
{
    public class MainViewModel
    {
        public AppModel AppModel { get; } = new AppModel();

        public StrokeCollection Strokes { get; } = new StrokeCollection();

        Dictionary<int, Stroke> activeStrokes = new Dictionary<int, Stroke>();

        public MainViewModel()
        {
            // 非 UI スレッドから UI スレッドに切り替えます。
            AppModel.TouchedPositions
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(DrawStrokes);
        }

        void DrawStrokes(Dictionary<int, StylusPoint> positions)
        {
            var idsToRemove = activeStrokes.Keys.Except(positions.Keys).ToArray();
            foreach (var id in idsToRemove)
            {
                activeStrokes.Remove(id);
            }

            foreach (var item in positions)
            {
                if (activeStrokes.ContainsKey(item.Key))
                {
                    activeStrokes[item.Key].StylusPoints.Add(item.Value);
                }
                else
                {
                    var stroke = new Stroke(new StylusPointCollection(new[] { item.Value }));
                    activeStrokes[item.Key] = stroke;

                    Strokes.Add(stroke);
                }
            }
        }
    }
}
