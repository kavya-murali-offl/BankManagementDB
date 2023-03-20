using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Data.Adapter.NetworkAdapter
{
    public class NetAdapter
    {
        private readonly HttpClient _httpClient;

        public NetAdapter()
        {
            _httpClient = new HttpClient();
        }

        public async Task Get(string url)
        {
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string s = await response.Content.ReadAsStringAsync();
            Console.WriteLine(s);
        }

        public async Task Post(string url, string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            Console.WriteLine(response);
            response.EnsureSuccessStatusCode();

            string resp = await response.Content.ReadAsStringAsync();

            Console.WriteLine(resp);
        }
    }
}
