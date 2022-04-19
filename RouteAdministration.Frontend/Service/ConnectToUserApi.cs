using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class ConnectToUserApi
    {
        private readonly static string _baseUri = "https://localhost:44309/api/";

        public async Task<List<User>> GetUsers()
        {
            List<User> users = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiUser");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        users = JsonConvert.DeserializeObject<List<User>>(responseBody);
                    }
                    else
                        users = null;
                }

                return users;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    users[0].Error = exception.InnerException.Message;
                else
                    users[0].Error = exception.StatusCode.ToString();

                return users;
            }
        }
    }
}
