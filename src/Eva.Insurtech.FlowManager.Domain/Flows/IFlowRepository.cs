using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public interface IFlowRepository : IRepository<Flow, Guid>
    {
        Task<Flow> GetByIdWithoutInactivesAsync(Guid flowId, bool withFlowSteps = true);
        Task<Flow> GetByIdWithInactivesAsync(Guid flowId);
        Task<ICollection<Flow>> GetAllAsync(bool withDetails = false);
        Task<Flow> GetByCodeAsync(string code, bool withInactives = false);
        Task<Flow> FindIfExistsAsync(Flow flow);
        Task<Flow> FindByCodeAsync(Flow flow);
        Task<ICollection<Flow>> GetByProductIdAsync(Guid id);
        Task<Flow> GetByProductIdAndChannelCodeAsync(Guid id, string channelCode);
        Task<Flow> GetFlowWithDetailsAsync(Guid idFlow);

    }
}


