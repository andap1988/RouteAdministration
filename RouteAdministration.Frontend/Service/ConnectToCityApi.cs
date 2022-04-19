using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class ConnectToCityApi
    {
        private readonly static string _baseUri = "https://localhost:44334/api/";

        public async Task<List<City>> GetCities()
        {
            List<City> cities = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiCity");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        cities = JsonConvert.DeserializeObject<List<City>>(responseBody);
                    }
                    else
                        cities = null;
                }

                return cities;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    cities[0].Error = exception.InnerException.Message;
                else
                    cities[0].Error = exception.StatusCode.ToString();

                return cities;
            }
        }
    }
}
