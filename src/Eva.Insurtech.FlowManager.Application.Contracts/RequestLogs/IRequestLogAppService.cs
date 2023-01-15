using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    public interface IRequestLogAppService : IApplicationService
    {
        Task<Response<RequestLogDto>> InsertAsync(CreateRequestLogDto createRequestLog);
        Task<Response<RequestLogDto>> AddRequestLogRequest(CreateRequestDto input, Guid requestLogId);
        Task<Response<RequestLogDto>> GetById(Guid requestLogId);
        Task<Response<RequestLogDto>> GetByService(string service);
        Task<Response<bool>> GroupRequests();
    }
}
