using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        public async Task<Person> GetPersonById(string id)
        {
            Person person = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiPerson/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        person = JsonConvert.DeserializeObject<Person>(responseBody);
                    }
                    else
                        person = null;
                }

                return person;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    person.Error = exception.InnerException.Message;
                else
                    person.Error = exception.StatusCode.ToString();

                return person;
            }
        }

        public async Task<Person> CreateNewPerson(Person person)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(person);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiPerson", content);

                    if (result.IsSuccessStatusCode)
                        return person;
                    else
                        person = null;

                    return person;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    person.Error = exception.InnerException.Message;
                else
                    person.Error = exception.StatusCode.ToString();

                return person;
            }
        }
    }
}
