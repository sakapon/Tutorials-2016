using System;
using System.Collections.Generic;
using System.Linq;

namespace SortViewerWpf
{
    public static class SortHelper
    {
        public static void BubbleSort<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare)
        {
            for (var i = 0; i < source.Count; i++)
                for (var j = source.Count - 1; i < j; j--)
                    if (compare(source[j - 1], source[j]) > 0)
                        source.Swap(j - 1, j);
        }

        public static void QuickSort<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare) =>
            QuickSort(source, compare, 0, source.Count);

        public static void QuickSort<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare, int start, int count)
        {
            if (count < 2) return;

            // 長さが 2 の場合は最適化します。
            if (count == 2)
            {
                if (compare(source[start], source[start + 1]) > 0)
                    source.Swap(start, start + 1);
                return;
            }

            var m = source[start];
            // cursor1: x <= m が保証されるインデックス。
            // cursor2: x > m が保証されるインデックス。
            var cursor1 = start;
            var cursor2 = start + count;

            while (true)
            {
                cursor1 = AssertLessThanOrEqual(source, compare, cursor1 + 1, cursor2 - 1, m);
                if (cursor1 == cursor2 - 1)
                    break;

                cursor2 = AssertGreaterThan(source, compare, cursor2 - 1, cursor1 + 2, m);
                if (cursor2 == cursor1 + 2)
                {
                    cursor2--;
                    break;
                }

                cursor1++;
                cursor2--;
                source.Swap(cursor1, cursor2);
            }

            if (start != cursor1)
                source.Swap(start, cursor1);

            // start <= i < cursor1
            source.QuickSort(compare, start, cursor1 - start);
            // cursor2 <= i < start + count
            source.QuickSort(compare, cursor2, start + count - cursor2);
        }

        static int AssertLessThanOrEqual<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare, int start, int end, TSource m)
        {
            for (var i = start; i <= end; i++)
                if (compare(source[i], m) > 0)
                    return i - 1;

            return end;
        }

        static int AssertGreaterThan<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare, int start, int end, TSource m)
        {
            for (var i = start; i >= end; i--)
                if (compare(source[i], m) <= 0)
                    return i + 1;

            return end;
        }

        public static void MergeSort<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare) =>
            MergeSort(source, compare, 0, source.Count);

        public static void MergeSort<TSource>(this IList<TSource> source, Func<TSource, TSource, int> compare, int start, int count)
        {
            if (count < 2) return;

            // 長さが 2 の場合は最適化します。
            if (count == 2)
            {
                if (compare(source[start], source[start + 1]) > 0)
                    source.Swap(start, start + 1);
                return;
            }

            var m = count / 2;
            source.MergeSort(compare, start, m);
            source.MergeSort(compare, start + m, count - m);

            var a1 = Enumerable.Range(start, m).Select(i => source[i]).ToArray();
            var a2 = Enumerable.Range(start + m, count - m).Select(i => source[i]).ToArray();
            var i1 = 0;
            var i2 = 0;
            while (i1 < a1.Length || i2 < a2.Length)
            {
                if (i2 == a2.Length || i1 < a1.Length && compare(a1[i1], a2[i2]) <= 0)
                {
                    source[start + i1 + i2] = a1[i1];
                    i1++;
                }
                else
                {
                    source[start + i1 + i2] = a2[i2];
                    i2++;
                }
            }
        }

        public static void Swap<TSource>(this IList<TSource> source, int index1, int index2)
        {
            var e = source[index1];
            source[index1] = source[index2];
            source[index2] = e;
        }
    }
}
