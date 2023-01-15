using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    public interface IRequestLogRepository : IRepository<RequestLog, Guid>
    {
        Task<RequestLog> GetByIdAsync(Guid requestLogId);
        Task<RequestLog> GetByService(string service);
        Task<RequestLog> GetByServiceToday(string service);
        Task<ICollection<RequestLog>> GetNotGroupedRequests();
    }
}

