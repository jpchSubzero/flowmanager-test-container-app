using Eva.Framework.Utility.Response.Models;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers.IntegrationApiServices
{
    public interface IIntegrationApiServiceManager : IApplicationService
    {
        Task<object> GetBusServicesRetries();
        Task<object> GetBusServicesByTracking(Guid TrackingId);
    }
}
