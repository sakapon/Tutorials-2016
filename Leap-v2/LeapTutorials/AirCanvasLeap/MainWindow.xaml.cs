using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AirCanvasLeap
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public AppModel AppModel { get; } = new AppModel();

        Dictionary<int, Stroke> strokes = new Dictionary<int, Stroke>();

        public MainWindow()
        {
            InitializeComponent();

            // 非 UI スレッドから UI スレッドに切り替えます。
            AppModel.TouchedPositions
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(DrawStrokes);
        }

        void DrawStrokes(Dictionary<int, StylusPoint> positions)
        {
            var idsToRemove = strokes.Keys.Except(positions.Keys).ToArray();
            foreach (var id in idsToRemove)
            {
                strokes.Remove(id);
            }

            foreach (var item in positions)
            {
                if (strokes.ContainsKey(item.Key))
                {
                    strokes[item.Key].StylusPoints.Add(item.Value);
                }
                else
                {
                    var stroke = new Stroke(new StylusPointCollection(new[] { item.Value }));
                    strokes[item.Key] = stroke;

                    TheCanvas.Strokes.Add(stroke);
                }
            }
        }
    }
}
