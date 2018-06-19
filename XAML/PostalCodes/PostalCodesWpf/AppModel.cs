using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace PostalCodesWpf
{
    public class AppModel
    {
        public ReactiveProperty<string> PostalCode { get; } = new ReactiveProperty<string>();
        public ReadOnlyReactiveProperty<ZipAddressResponse> Response { get; }

        public AppModel()
        {
            Response = PostalCode
                .Select(c => c != null ? HttpHelper.Get<ZipAddressResponse>($"http://api.zipaddress.net/?zipcode={c}") : null)
                .Select(r => r?.code == 200 ? r : null)
                .ToReadOnlyReactiveProperty();
        }
    }

    // http://zipaddress.net/
    public class ZipAddressResponse
    {
        public int code { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string pref { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string town { get; set; }
        public string fullAddress { get; set; }
    }
}
