using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SortViewerWpf
{
    public class AppModel
    {
        public const int DefaultMaxNumber = 100;
        static readonly TimeSpan ComparisonsSpan = TimeSpan.FromSeconds(1 / 1000.0);

        public int[] Numbers { get; private set; } = Enumerable.Range(1, DefaultMaxNumber).ToArray();
        public int ComparisonsCount { get; private set; } = 0;

        public void Initialize(int maxNumber)
        {
            Numbers = RandomHelper.ShuffleRange(1, maxNumber).ToArray();
            ComparisonsCount = 0;
        }

        public Task BubbleSort() => ExecuteSort(SortHelper.BubbleSort);
        public Task QuickSort() => ExecuteSort(SortHelper.QuickSort);
        public Task MergeSort() => ExecuteSort(SortHelper.MergeSort);

        Task ExecuteSort(Action<IList<int>, Func<int, int, int>> sort)
        {
            return Task.Run(() =>
                sort(Numbers, (x1, x2) =>
                {
                    Thread.Sleep(ComparisonsSpan);
                    ComparisonsCount++;
                    return x1.CompareTo(x2);
                }));
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
