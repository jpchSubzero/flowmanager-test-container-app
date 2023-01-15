using Eva.Framework.Utility.Option.Contracts;
using Eva.Insurtech.AppApiServices;
using Eva.Insurtech.FlowManagers.BusCharge;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.IntegrationApiServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Eva.Insurtech.FlowManagers.MessengerApiServices
{
    public class MessengerApiServiceManager : FlowManagerAppService, IMessengerAppService
    {
        private readonly IAppConfigurationManager _appConfigurationManager;
        private readonly IAppApiServiceManager _appApiServiceManager;
        public MessengerApiServiceManager(IAppConfigurationManager appConfigurationManager, IAppApiServiceManager appApiServiceManager, ILogger<MessengerApiServiceManager> logger) : base(logger)
        {
            _appConfigurationManager = appConfigurationManager;
            _appApiServiceManager = appApiServiceManager;
        }
        private string GetUrlContext()
        {
            var url = _appConfigurationManager.GetServiceUrlBySystemControllerCapacity("Eva", "Apis", "MessengerBaseUrl");
            return url;
        }
        public async Task<bool> SendReportAsync(MessengerEvaDto dataMenssenger)
        {
            try
            {
                string urlBase = $"{GetUrlContext()}{BusChargeConsts.MessengerReportApi}";
                RequestDataDto requestData = new()
                {
                    UrlBase = urlBase,
                    Body = JsonConvert.SerializeObject(dataMenssenger, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }),
                };

                var result = await _appApiServiceManager.SubmitAsync(requestData);
                return result["result"].ToString() == BusChargeConsts.MessengerSuccess;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
