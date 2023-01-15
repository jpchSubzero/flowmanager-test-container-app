using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.MessengerApiServices;
using Microsoft.Extensions.Logging;
using System;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers
{
    public abstract class FlowManagerAppService : ApplicationService
    {
        private readonly ILogger _logger;

        protected FlowManagerAppService(ILogger logger)
        {
            LocalizationResource = typeof(FlowManagerResource);
            ObjectMapperContext = typeof(FlowManagerApplicationModule);
            _logger = logger;

        }

        protected Response<T> GetErrorResponse<T>(Error error, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            ResponseManager<T> response = new();

            if (ex.GetType().IsSubclassOf(typeof(GeneralException)))
            {
                return response.OnError(new Error(ex as GeneralException));
            }
            else
            {
                return response.OnError(error);
            }
        }
    }
}
