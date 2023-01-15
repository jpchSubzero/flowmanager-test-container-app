using Eva.Framework.Utility.Option.Contracts;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.AppApiServices;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.BusCharge;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RequestDataDto = Eva.Insurtech.AppApiServices.RequestDataDto;

namespace Eva.Insurtech.FlowManagers.IntegrationApiServices
{
    public class IntegrationApiServiceManager : FlowManagerAppService
    {
        private readonly IAppConfigurationManager _appConfigurationManager;
        private readonly IAppApiServiceManager _appApiServiceManager;

        public IntegrationApiServiceManager(IAppConfigurationManager appConfigurationManager, IAppApiServiceManager appApiServiceManager, ILogger<IntegrationApiServiceManager> logger) : base(logger)
        {
            _appConfigurationManager = appConfigurationManager;
            _appApiServiceManager = appApiServiceManager;
        }
        public async Task<int> GetBusServicesRetries()
        {
            string UrlBusChargeEntries = _appConfigurationManager.GetServiceUrlBySystemControllerCapacity("Integration", "BusCharge", "GetBusServicesRetries");

            RequestDataDto requestData = new()
            {
                UrlBase = UrlBusChargeEntries
                
            };

            var result = await _appApiServiceManager.QueryAsync(requestData);

            var resultObject = JsonConvert.DeserializeObject<Response<int>>(result.ToString());

            if (!resultObject.Success)
                throw new ExternalServiceException(resultObject.Error);

            return resultObject.Result;

        }
        public async Task<BusChargeDto> GetBusServicesByTracking(Guid TrackingId)
        {
            string UrlBase = _appConfigurationManager.GetServiceUrlBySystemControllerCapacity("Integration", "BusCharge", "GetBusChargeByTracking");
            string Urlcomplete  = UrlBase+"{"+ TrackingId.ToString()+ BusChargeConsts.GetBusServices;

            var parameters = new Dictionary<string, string>
            {
                { "trackingId", TrackingId.ToString() }
            };
            RequestDataDto requestData = new()
            {
                UrlBase = Urlcomplete,
                Parameters = parameters,
            };
            var result = await _appApiServiceManager.QueryAsync(requestData);
            var resultObject = JsonConvert.DeserializeObject<Response<BusChargeDto>>(result.ToString());
            if (!resultObject.Success)
                throw new ExternalServiceException(resultObject.Error);

            return resultObject.Result;
        }
    }
}