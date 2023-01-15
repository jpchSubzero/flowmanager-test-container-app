using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    [RemoteService]
    [Route("api/request-log")]
    public class RequestLogController : FlowManagerController, IRequestLogAppService
    {
        private readonly IRequestLogAppService _requestLogAppService;

        public RequestLogController(IRequestLogAppService requestLogAppService)
        {
            _requestLogAppService = requestLogAppService;
        }

        [HttpPost]
        public async Task<Response<RequestLogDto>> InsertAsync(CreateRequestLogDto createRequestLog)
        {
            return await _requestLogAppService.InsertAsync(createRequestLog);
        }

        [HttpPost]
        [Route("{requestLogId}/request")]
        public async Task<Response<RequestLogDto>> AddRequestLogRequest(CreateRequestDto input, Guid requestLogId)
        {
            return await _requestLogAppService.AddRequestLogRequest(input, requestLogId);
        }

        [HttpGet]
        [Route("{requestLogId}/by-id")]
        public async Task<Response<RequestLogDto>> GetById(Guid requestLogId)
        {
            return await _requestLogAppService.GetById(requestLogId);
        }

        [HttpGet]
        [Route("{transactionReference}/by-service")]
        public async Task<Response<RequestLogDto>> GetByService(string service)
        {
            return await _requestLogAppService.GetByService(service);
        }

        [HttpPost]
        [Route("group-requests")]
        public async Task<Response<bool>> GroupRequests()
        {
            return await _requestLogAppService.GroupRequests();
        }
    }
}
