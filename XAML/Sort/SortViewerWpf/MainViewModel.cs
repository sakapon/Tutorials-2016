using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
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

        public void BubbleSort() => ExecuteSort(AppModel.BubbleSort);
        public void QuickSort() => ExecuteSort(AppModel.QuickSort);
        public void MergeSort() => ExecuteSort(AppModel.MergeSort);

        async void ExecuteSort(Func<Task> sort)
        {
            IsRunning.Value = true;

            AppModel.Initialize(MaxNumber.Value);
            Numbers.Clear();
            Numbers.AddRangeOnScheduler(new int[MaxNumber.Value]);

            var subscription = Observable.Interval(RenderingSpan)
                .Do(_ => ComparisonsCount.Value = AppModel.ComparisonsCount)
                .Subscribe(_ =>
                {
                    var ns = AppModel.Numbers.ToArray();
                    var maxCount = Numbers.Count;
                    for (var i = 0; i < maxCount; i++)
                        if (Numbers[i] != ns[i])
                            Numbers.SetOnScheduler(i, ns[i]);
                });

            await Task.Delay(TimeSpan.FromSeconds(0.8));
            await sort();
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            subscription.Dispose();
            IsRunning.Value = false;
        }
    }
}
