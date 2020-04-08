using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FYPMobileApp.Models
{
    public class RestService
    {
        private HttpClient client;

        private const string BaseUrl = "http://10.0.2.2:3000/users/";

        public RestService()
        {
            //TODO remove type

            client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> GetRequest()
        {
            var response = await client.GetAsync(client.BaseAddress + "allUsers");
            //response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        //TODO
        public async Task<string> PostRequest(object data)
        {
            StringContent content = new StringContent(JsonSerializer.Serialize(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(client.BaseAddress + "userLogin", content);
            //response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
