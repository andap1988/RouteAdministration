using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class ConnectToEquipApi
    {
        private readonly static string _baseUri = "https://localhost:44338/api/";

        public async Task<List<Equip>> GetEquips()
        {
            List<Equip> equips = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equips = JsonConvert.DeserializeObject<List<Equip>>(responseBody);
                    }
                    else
                        equips = null;
                }

                return equips;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equips[0].Error = exception.InnerException.Message;
                else
                    equips[0].Error = exception.StatusCode.ToString();

                return equips;
            }
        }

        public async Task<Equip> GetEquipById(string id)
        {
            Equip equip = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip/" + id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equip = JsonConvert.DeserializeObject<Equip>(responseBody);
                    }
                    else
                        equip = null;
                }

                return equip;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equip.Error = exception.InnerException.Message;
                else
                    equip.Error = exception.StatusCode.ToString();

                return equip;
            }
        }

        public async Task<List<Equip>> GetEquipByCity(string city)
        {
            List<Equip> equips = new();
            Equip equip = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip/city/" + city);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equips = JsonConvert.DeserializeObject<List<Equip>>(responseBody);
                    }
                    else
                        equips = null;
                }

                return equips;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equips[0].Error = exception.InnerException.Message;
                else
                    equips[0].Error = exception.StatusCode.ToString();

                return equips;
            }
        }

        public async Task<List<Equip>> GetEquipsByEquipsName(List<string> equipsName)
        {
            List<Equip> equips = new();
            Equip equip = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    foreach (var equipName in equipsName)
                    {
                        HttpResponseMessage response = await client.GetAsync("ApiEquip/equip/" + equipName);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = response.Content.ReadAsStringAsync().Result;

                            equip = JsonConvert.DeserializeObject<Equip>(responseBody);
                        }
                        else
                            equip = null;

                        equips.Add(equip);
                    }
                }

                return equips;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equips[0].Error = exception.InnerException.Message;
                else
                    equips[0].Error = exception.StatusCode.ToString();

                return equips;
            }
        }

        public async Task<Equip> GetEquipByEquipName(string equipName)
        {
            Equip equip = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new Uri(_baseUri);

                    HttpResponseMessage response = await client.GetAsync("ApiEquip/equip/" + equipName);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        equip = JsonConvert.DeserializeObject<Equip>(responseBody);
                    }
                    else
                        equip = null;
                }

                return equip;
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equip.Error = exception.InnerException.Message;
                else
                    equip.Error = exception.StatusCode.ToString();

                return equip;
            }
        }

        public async Task<Equip> CreateNewEquip(Equip equip)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(equip);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync("ApiEquip", content);

                    if (result.IsSuccessStatusCode)
                        return equip;
                    else
                        equip = null;

                    return equip;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equip.Error = exception.InnerException.Message;
                else
                    equip.Error = exception.StatusCode.ToString();

                return equip;
            }
        }

        public async Task<Equip> EditEquip(Equip equip)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var json = JsonConvert.SerializeObject(equip);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PutAsync($"ApiEquip/{equip.Id}", content);

                    if (result.IsSuccessStatusCode)
                        return equip;
                    else
                        equip = null;

                    return equip;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equip.Error = exception.InnerException.Message;
                else
                    equip.Error = exception.StatusCode.ToString();

                return equip;
            }
        }

        public async Task<Equip> RemoveEquip(string id)
        {
            Equip equip = new();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseUri);

                    var result = await client.DeleteAsync($"ApiEquip/{id}");

                    if (result.IsSuccessStatusCode)
                        equip.Error = "ok";
                    else
                        equip.Error = "notSave";

                    return equip;
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode == null)
                    equip.Error = exception.InnerException.Message;
                else
                    equip.Error = exception.StatusCode.ToString();

                return equip;
            }
        }
    }
}
