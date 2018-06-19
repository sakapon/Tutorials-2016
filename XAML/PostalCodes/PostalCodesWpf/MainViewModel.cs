using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Reactive.Bindings;

namespace PostalCodesWpf
{
    public class MainViewModel
    {
        static readonly Regex PostalCodePattern = new Regex("^[0-9]{3}-?[0-9]{4}$");

        public AppModel AppModel { get; } = new AppModel();

        public ReactiveProperty<string> InputPostalCode { get; } = new ReactiveProperty<string>("");
        public ReadOnlyReactiveProperty<string> Address { get; }

        public MainViewModel()
        {
            InputPostalCode
                .ObserveOn(TaskPoolScheduler.Default)
                .Select(c => PostalCodePattern.IsMatch(c) ? c : null)
                .Subscribe(c => AppModel.PostalCode.Value = c);

            Address = AppModel.Response
                .Select(r => r != null ? r.data.fullAddress : null)
                .ToReadOnlyReactiveProperty();
        }
    }
}
