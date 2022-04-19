using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
    }
}
