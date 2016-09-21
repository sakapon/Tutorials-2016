using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;

namespace SortViewerWpf
{
    public class MainViewModel
    {
        static readonly TimeSpan RenderingSpan = TimeSpan.FromSeconds(1 / 60.0);

        public AppModel AppModel { get; } = new AppModel();

        public ReactiveCollection<int> Numbers { get; }
        public ReactiveProperty<int> ComparisonsCount { get; }

        public ReactiveProperty<int> MaxNumber { get; } = new ReactiveProperty<int>(AppModel.DefaultMaxNumber);
        public ReactiveProperty<bool> IsRunning { get; } = new ReactiveProperty<bool>();
        public ReadOnlyReactiveProperty<bool> IsStopped { get; }

        public MainViewModel()
        {
            Numbers = AppModel.Numbers.ToObservable().ToReactiveCollection();
            ComparisonsCount = new ReactiveProperty<int>(AppModel.ComparisonsCount);

            IsStopped = IsRunning.Select(b => !b).ToReadOnlyReactiveProperty();
        }

        public async void BubbleSort()
        {
            IsRunning.Value = true;

            Numbers.Clear();
            Numbers.AddRangeOnScheduler(new int[MaxNumber.Value]);

            var subscription = Observable.Interval(RenderingSpan)
                .Do(_ => ComparisonsCount.Value = AppModel.ComparisonsCount)
                .Select(_ => AppModel.Numbers.ToArray())
                .Subscribe(ns =>
                {
                    var maxCount = Math.Min(Numbers.Count, ns.Length);
                    for (var i = 0; i < maxCount; i++)
                        if (Numbers[i] != ns[i])
                            Numbers.SetOnScheduler(i, ns[i]);
                });

            await AppModel.BubbleSort(MaxNumber.Value);

            subscription.Dispose();
            IsRunning.Value = false;
        }
    }
}
