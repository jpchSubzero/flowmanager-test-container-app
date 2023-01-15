using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.ApiServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Eva.Framework.Utility.Response.Models;
using Serilog;

namespace Eva.Insurtech.AppApiServices
{
    public class AppApiServiceManager : FlowManagerAppService, IAppApiServiceManager
    {
        private readonly HttpClient _httpClient;
        
        public AppApiServiceManager (HttpClient httpClient, ILogger<AppApiServiceManager> logger) : base(logger)
        { 
            _httpClient = httpClient;
        }
        public async Task<JObject> SubmitAsync(RequestDataDto requestData, bool returnWithError = false)
        {
            StringContent requestContent = null;
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

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

                requestData.UrlBase = CompleteUrlParameters(requestData);

                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var result = await _httpClient.PostAsync(requestData.UrlBase, requestContent);

                if (!returnWithError)
                    result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                content = CompleteJsonNodeData(content);

                JObject response = JsonConvert.DeserializeObject<JObject>(content);

                return response;
            }
            catch (GeneralException ex)
            {
                Log.Logger.Error(string.Format("Excepcion: {0} Trama: {1}", ex.Message, JsonConvert.SerializeObject(requestData)));
                throw new ExternalServiceException(new Framework.Utility.Response.Models.Error(ex));
            }
        }
        public async Task<JObject> QueryAsync(RequestDataDto requestData, bool returnWithError = false)
        {

            try
            {
                _httpClient.DefaultRequestHeaders.Clear();

                if (requestData.Credentials != null)
                {
                    byte[] authentication = Encoding.ASCII.GetBytes($"{requestData.Credentials.User}:{requestData.Credentials.Password}");
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authentication));
                }

                if (requestData.Headers != null)
                    foreach (var header in requestData.Headers)
                        if (!header.Key.IsNullOrEmpty() && !header.Value.IsNullOrEmpty())
                            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);

                requestData.UrlBase = CompleteUrlParameters(requestData);

                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var result = await _httpClient.GetAsync(requestData.UrlBase);

                if (!returnWithError)
                    result.EnsureSuccessStatusCode();

                var content = await result.Content.ReadAsStringAsync();

                content = CompleteJsonNodeData(content);

                JObject response = JsonConvert.DeserializeObject<JObject>(content);

                return response;
            }
            catch (GeneralException ex)
            {
                Log.Logger.Error(string.Format("Excepcion: {0} Trama: {1}", ex.Message, JsonConvert.SerializeObject(requestData)));
                throw new ExternalServiceException(new Framework.Utility.Response.Models.Error(ex));
            }

        }

        #region Private Methods

        private static string CompleteUrlParameters(RequestDataDto requestData)
        {
            if (requestData.InLineParameters != null)
                requestData.UrlBase += string.Join(string.Empty, requestData.InLineParameters.Where(s => !string.IsNullOrEmpty(s.ToString().Trim())));

            if (requestData.Parameters != null)
                requestData.UrlBase += string.Format($"?{string.Join("&", requestData.Parameters.Where(v => !string.IsNullOrEmpty(v.Value?.Trim())).Select(k => string.Format($"{k.Key}={k.Value}")))}");

            return requestData.UrlBase;
        }

        private static string CompleteJsonNodeData(string data)
        {
            data = data.Trim();

            if (data.Substring(0, 1).Contains("["))
                return $"{{\"data\": {data}}}";
            if (!data.StartsWith("{") && !data.Equals("null"))
                return $"{{\"data\": {data}}}";
            return data;
        }

        #endregion
    }
}
