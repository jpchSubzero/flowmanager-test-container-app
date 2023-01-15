using Eva.Insurtech.FlowManagers.RequestLogs.Exceptions;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using Eva.Insurtech.FlowManagers.Trackings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    public class RequestLogManager : DomainService
    {
        private readonly IRequestLogRepository _requestLogRepository;

        public RequestLogManager(IRequestLogRepository requestLogRepository)
        {
            _requestLogRepository = requestLogRepository;
        }

        public async Task<RequestLog> InsertAsync(RequestLog requestLog, bool autoSave = true)
        {
            return await _requestLogRepository.InsertAsync(requestLog, autoSave);
        }

        public async Task<RequestLog> AddRequestLogRequest(RequestInput input)
        {
            var requestLog = await _requestLogRepository.GetByIdAsync(input.RequestLogId);
            GenericValidations.ValidateIfItemExists(requestLog, new RequestLogNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            requestLog.Requests.RemoveAll(x => x.TransactionReference.Equals(input.TransactionReference));
            requestLog.AddRequest(input);
            return await _requestLogRepository.UpdateAsync(requestLog, true);
        }

        public async Task<RequestLog> GetById(Guid requestLogId)
        {
            return await _requestLogRepository.GetByIdAsync(requestLogId);
        }

        public async Task<RequestLog> GetByService(string service)
        {
            return await _requestLogRepository.GetByService(service);
        }

        public async Task<RequestLog> GetByServiceToday(string service)
        {
            return await _requestLogRepository.GetByServiceToday(service);
        }

        public async Task GroupRequests()
        {
            var notGroupedRequests = await _requestLogRepository.GetNotGroupedRequests();
            foreach (var notGroupedRequest in notGroupedRequests)
            {
                notGroupedRequest.GroupRequestsByService();
            }
        }
    }
}
