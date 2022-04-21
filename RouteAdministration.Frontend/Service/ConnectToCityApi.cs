using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        public async Task<City> GetCityById(string id)
        {
            City city = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiCity/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        city = JsonConvert.DeserializeObject<City>(responseBody);
                    }
                    else
                        city = null;
                }

                return city;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    city.Error = exception.InnerException.Message;
                else
                    city.Error = exception.StatusCode.ToString();

                return city;
            }
        }

        public async Task<City> CreateNewCity(City city)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(city);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiCity", content);

                    if (result.IsSuccessStatusCode)
                        return city;
                    else
                        city = null;

                    return city;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    city.Error = exception.InnerException.Message;
                else
                    city.Error = exception.StatusCode.ToString();

                return city;
            }
        }

        public async Task<City> RemoveCity(string id)
        {
            City city = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiCity/{id}");

                    if (result.IsSuccessStatusCode)
                        city.Error = "ok";
                    else
                        city.Error = "notSave";

                    return city;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    city.Error = exception.InnerException.Message;
                else
                    city.Error = exception.StatusCode.ToString();

                return city;
            }
        }

    }
}
