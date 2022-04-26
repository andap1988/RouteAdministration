using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RouteAdministration.Frontend.Service
{
    public class ConnectToRadarApi
    {
        public readonly string _baseUri = "https://api.radar.io/v1/";
        public readonly string _apiKeyTestPublish = "prj_test_pk_9815826d01be5e546051f94bbe6e3f4f7d49e588";
        public readonly string _apiKeyLivePublish = "prj_live_pk_4366636ca1c2e5a9666445f2b2f91712357fd87d";

        public async Task<string> GetLocation()
        {
            var result = "";

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(_apiKeyLivePublish);
                    client.BaseAddress = new Uri(_baseUri);
                    
                    HttpResponseMessage response = await client.GetAsync("geocode/forward?query=Rua+Nove+de+Julho+500+Centro+Araraquara&country=BR");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;

                        result = responseBody;
                    }
                    else
                        result = null;
                }

                return result;
            }
            catch (HttpRequestException exception)
            {
                result = "erro";
                return result;
            }
        }
    }
}
