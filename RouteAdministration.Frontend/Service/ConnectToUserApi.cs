using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
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

        public async Task<User> GetUserById(string id)
        {
            User user = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiUser/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        user = JsonConvert.DeserializeObject<User>(responseBody);
                    }
                    else
                        user = null;
                }

                return user;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    user.Error = exception.InnerException.Message;
                else
                    user.Error = exception.StatusCode.ToString();

                return user;
            }
        }

        public async Task<User> GetUserByUsername(string username)
        {
            User user = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiUser/login/" + username);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        user = JsonConvert.DeserializeObject<User>(responseBody);
                    }
                    else
                        user = null;
                }

                return user;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    user.Error = exception.InnerException.Message;
                else
                    user.Error = exception.StatusCode.ToString();

                return user;
            }
        }

        public async Task<User> CreateNewUser(User user)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiUser", content);

                    if (result.IsSuccessStatusCode)
                        return user;
                    else
                        user = null;

                    return user;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    user.Error = exception.InnerException.Message;
                else
                    user.Error = exception.StatusCode.ToString();

                return user;
            }
        }

        public async Task<User> EditUser(User user)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(user);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PutAsync($"ApiUser/{user.Id}", content);

                    if (result.IsSuccessStatusCode)
                        return user;
                    else
                        user = null;

                    return user;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    user.Error = exception.InnerException.Message;
                else
                    user.Error = exception.StatusCode.ToString();

                return user;
            }
        }

        public async Task<User> RemoveUser(string id)
        {
            User user = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiUser/{id}");

                    if (result.IsSuccessStatusCode)
                        user.Error = "ok";
                    else
                        user.Error = "notSave";

                    return user;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    user.Error = exception.InnerException.Message;
                else
                    user.Error = exception.StatusCode.ToString();

                return user;
            }
        }
    }
}
