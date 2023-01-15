using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    public interface IPreTrackingRepository : IRepository<PreTracking, Guid>
    {
        Task<PreTracking> GetByIdAsync(Guid preTrackingId);
        Task<PreTracking> GetByTransactionReference(string transactionReference);
    }
}

