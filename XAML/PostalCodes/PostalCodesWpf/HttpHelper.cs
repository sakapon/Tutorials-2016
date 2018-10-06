using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PostalCodesWpf
{
    public static class HttpHelper
    {
        async public static Task<T> GetAsync<T>(string uri)
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<T>();
            }
        }

        async public static Task<T> GetJsonAsync<T>(string uri)
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        async public static Task<T> GetOrDefaultAsync<T>(string uri)
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(uri);
                if (!response.IsSuccessStatusCode) return default(T);
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public static T Get<T>(string uri) =>
            GetOrDefaultAsync<T>(uri).GetAwaiter().GetResult();
    }
}
