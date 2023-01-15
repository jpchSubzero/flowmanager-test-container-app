using Eva.Framework.Utility.Response.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.ApiServices
{
    public class ApiServiceManager : DomainService
    {
        private readonly HttpClient _httpClient;

        public ApiServiceManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JObject> QueryAsync(RequestDataDto requestData)
        {
            StringContent requestContent = null;

            try
            {
                if (requestData.Credentials != null)
                {
                    byte[] authentication = Encoding.ASCII.GetBytes($"{requestData.Credentials.User}:{requestData.Credentials.Password}");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));
                }

                if (requestData.Headers != null)
                    foreach (var header in requestData.Headers)
                        _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

                if (!string.IsNullOrEmpty(requestData.Body))
                    requestContent = new StringContent(requestData.Body, Encoding.UTF8, "application/json");

                if (requestData.Parameters != null)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var parameter in requestData.Parameters)
                        if (!string.IsNullOrEmpty(parameter.Value))
                        {
                            if (String.IsNullOrEmpty(stringBuilder.ToString()))
                                stringBuilder.Append("?" + parameter.Key + "=" + parameter.Value);
                            else
                                stringBuilder.Append("&" + parameter.Key + "=" + parameter.Value);
                        }
                    requestData.UrlBase += stringBuilder.ToString();
                }

                var result = await _httpClient.GetAsync(requestData.UrlBase);

                result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                JObject response = JsonConvert.DeserializeObject<JObject>(content);

                return response;
            }
            catch (GeneralException ex)
            {
                throw new ExternalServiceException(new Framework.Utility.Response.Models.Error(ex));
            }

        }
    }
}
