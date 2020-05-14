using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace FYPMobileApp.Models
{
    public class RestService
    {
        private HttpClient client;

        private const string BaseUrl = "https://fypazureapp.azurewebsites.net/";

        public RestService()
        {
            //TODO remove type

            client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<string> GetRequest()
        {
            var response = await client.GetAsync(client.BaseAddress + "/users/allUsers");
            //response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }

        public async Task<TResult> PostLoginRequest <TResult>(object data)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(client.BaseAddress + "/users/userLogin", content);
            //response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            TResult result = JsonConvert.DeserializeObject<TResult>(responseString);
            return result;
            //return response.Content.ReadAsStringAsync().Result;
        }

        public void PostSafetyStatus(object data)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(data));
            Console.WriteLine($"content: {content}");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.PostAsync(client.BaseAddress + "/accessTimes/markAsSafe", content);
        }
    }
}
