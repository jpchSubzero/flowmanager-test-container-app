using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.Flows;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    public class RequestLogAppService : FlowManagerAppService, IRequestLogAppService
    {
        private readonly RequestLogManager _requestLogManager;
        private readonly IObjectMapper _objectMapper;

        public RequestLogAppService(
            ILogger<FlowAppService> logger,
            IObjectMapper objectMapper,
            RequestLogManager requestLogManager
        ) : base(logger)
        {
            _objectMapper = objectMapper;
            _requestLogManager = requestLogManager;
        }

        public async Task<Response<RequestLogDto>> InsertAsync(CreateRequestLogDto createRequestLog)
        {
            ResponseManager<RequestLogDto> response = new();
            try
            {
                var requestLogResult = await GetByServiceToday(createRequestLog.Service);
                if (requestLogResult.Result != null)
                    return response.OnSuccess(requestLogResult.Result);

                var requestLog = _objectMapper.Map<CreateRequestLogDto, RequestLog>(createRequestLog);

                requestLog.SetRegisterDate(DateTime.Now);
                var result = await _requestLogManager.InsertAsync(requestLog, true);
                return response.OnSuccess(_objectMapper.Map<RequestLog, RequestLogDto>(result));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<RequestLogDto>(TrackingErrorCodes.GetErrorRequestLogCreate(), ex);
            }
        }

        public async Task<Response<RequestLogDto>> AddRequestLogRequest(CreateRequestDto input, Guid requestLogId)
        {
            ResponseManager<RequestLogDto> response = new();
            try
            {
                input.SetRequestLogId(requestLogId);
                var createRequestInput = _objectMapper.Map<CreateRequestDto, RequestInput>(input);
                var RequestLog = await _requestLogManager.AddRequestLogRequest(createRequestInput);
                return response.OnSuccess(_objectMapper.Map<RequestLog, RequestLogDto>(RequestLog));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<RequestLogDto>(TrackingErrorCodes.GetErrorRequestLogAddStep(), ex);
            }
        }

        public async Task<Response<RequestLogDto>> GetById(Guid requestLogId)
        {
            ResponseManager<RequestLogDto> response = new();
            try
            {
                var requestLog = await _requestLogManager.GetById(requestLogId);
                return response.OnSuccess(_objectMapper.Map<RequestLog, RequestLogDto>(requestLog));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<RequestLogDto>(TrackingErrorCodes.GetErrorGetRequestLog(TrackingConsts.ID), ex);
            }
        }

        public async Task<Response<RequestLogDto>> GetByService(string service)
        {
            ResponseManager<RequestLogDto> response = new();
            try
            {
                var requestLog = await _requestLogManager.GetByService(service);
                return response.OnSuccess(_objectMapper.Map<RequestLog, RequestLogDto>(requestLog));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<RequestLogDto>(TrackingErrorCodes.GetErrorGetRequestLog(TrackingConsts.SERVICE), ex);
            }
        }

        private async Task<Response<RequestLogDto>> GetByServiceToday(string service)
        {
            ResponseManager<RequestLogDto> response = new();
            try
            {
                var requestLog = await _requestLogManager.GetByServiceToday(service);
                return response.OnSuccess(_objectMapper.Map<RequestLog, RequestLogDto>(requestLog));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<RequestLogDto>(TrackingErrorCodes.GetErrorGetRequestLog(TrackingConsts.SERVICE_TODAY), ex);
            }
        }

        public async Task<Response<bool>> GroupRequests()
        {
            ResponseManager<bool> response = new();
            try
            {
                await _requestLogManager.GroupRequests();
                return response.OnSuccess(true);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }
    }
}
