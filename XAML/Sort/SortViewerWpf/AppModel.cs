using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SortViewerWpf
{
    public class AppModel
    {
        static readonly TimeSpan ComparisonsSpan = TimeSpan.FromSeconds(1 / 1000.0);

        public int[] Numbers { get; private set; }
        public int ComparisonsCount { get; private set; }

        public void BubbleSort(int maxNumber)
        {
            Numbers = RandomHelper.ShuffleRange(1, maxNumber).ToArray();
            ComparisonsCount = 0;

            Task.Run(() =>
            {
                Thread.Sleep(500);
                Numbers.QuickSort((x1, x2) =>
                {
                    Thread.Sleep(ComparisonsSpan);
                    ComparisonsCount++;
                    return x1.CompareTo(x2);
                });
            });
        }
    }

    public static class RandomHelper
    {
        static readonly Random Random = new Random();

        public static IEnumerable<int> ShuffleRange(int start, int count)
        {
            var l = Enumerable.Range(start, count).ToList();

            while (l.Count > 0)
            {
                var index = Random.Next(l.Count);
                yield return l[index];
                l.RemoveAt(index);
            }
        }
    }
}
