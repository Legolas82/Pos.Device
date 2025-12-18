using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace POS.Core.Client
{
    public static class WebAPIHelper
    {

        public static async Task<IEnumerable<T>> Get<T>(string baseUrl, string path, string id = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                if (id == "")
                {
                    var response = await client.GetAsync(path);
                    var data = CreateObject<IEnumerable<T>>(response.Content.ToString());
                    return data;
                }
                else
                {
                    var response = await client.GetAsync($"{path}/{id}");
                    var obj = CreateObject<T>(response.Content.ToString());
                    var data = new T[1];
                    data[0] = obj;
                    return data;
                }
            }
        }

        public static async Task<T> Post<T>(string baseUrl, string path, T obj)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.PostAsJsonAsync(path, obj);
                //return CreateObject<T>(response.Content.ToString());
                return obj;
            }
        }

        public static async Task<T> Put<T>(string baseUrl, string path, string id, T obj)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.PutAsJsonAsync($"{path}/{id}", obj);
                return CreateObject<T>(response.Content.ToString());
            }
        }

        public static async Task<T> Delete<T>(string baseUrl, string path, string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                var response = await client.DeleteAsync($"{path}/{id}");
                return CreateObject<T>(response.Content.ToString());
            }
        }

        private static T CreateObject<T>(string obj)
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}
