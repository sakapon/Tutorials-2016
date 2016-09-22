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

        public Task BubbleSort(int maxNumber) => ExecuteSort(maxNumber, SortHelper.BubbleSort);
        public Task QuickSort(int maxNumber) => ExecuteSort(maxNumber, SortHelper.QuickSort);
        public Task MergeSort(int maxNumber) => ExecuteSort(maxNumber, SortHelper.MergeSort);

        Task ExecuteSort(int maxNumber, Action<IList<int>, Func<int, int, int>> sort)
        {
            Numbers = RandomHelper.ShuffleRange(1, maxNumber).ToArray();
            ComparisonsCount = 0;

            return Task.Run(() =>
            {
                Thread.Sleep(800);
                sort(Numbers, (x1, x2) =>
                {
                    Thread.Sleep(ComparisonsSpan);
                    ComparisonsCount++;
                    return x1.CompareTo(x2);
                });
                Thread.Sleep(800);
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
