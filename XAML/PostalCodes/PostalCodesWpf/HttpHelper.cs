﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        public static T Get<T>(string uri) =>
            GetAsync<T>(uri).GetAwaiter().GetResult();
    }
}
