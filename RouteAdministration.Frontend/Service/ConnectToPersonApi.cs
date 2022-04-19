using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class ConnectToPersonApi
    {
        private readonly static string _baseUri = "https://localhost:44378/api/";

        public async Task<List<Person>> GetPeople()
        {
            List<Person> people = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiPerson");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        people = JsonConvert.DeserializeObject<List<Person>>(responseBody);
                    }
                    else
                        people = null;
                }

                return people;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    people[0].Error = exception.InnerException.Message;
                else
                    people[0].Error = exception.StatusCode.ToString();

                return people;
            }
        }
    }
}
