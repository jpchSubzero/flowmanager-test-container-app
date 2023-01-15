using Eva.Insurtech.FlowManagers.Flows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Flows
{
    public class FlowRepository : EfCoreRepository<FlowManagerDbContext, Flow, Guid>, IFlowRepository
    {
        public FlowRepository(IDbContextProvider<FlowManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<Flow> GetByIdWithoutInactivesAsync(Guid flowId, bool withFlowSteps = true)
        {
            var dbSet = await GetDbSetAsync();
            var flow = await dbSet.Where(x => x.Id == flowId && x.IsActive && !x.IsDeleted)
                .IncludeIf(withFlowSteps, x => x.FlowSteps)
                .FirstOrDefaultAsync();
            return flow;
        }

        public async Task<Flow> GetByIdWithInactivesAsync(Guid flowId)
        {
            var dbSet = await GetDbSetAsync();
            var flow = await dbSet.Where(x => x.Id == flowId && !x.IsDeleted)
                .Include(x => x.FlowSteps)
                .FirstOrDefaultAsync();
            return flow;
        }

        public async Task<ICollection<Flow>> GetAllAsync(bool withDetails = false)
        {
            var dbSet = await GetDbSetAsync();
            var products = dbSet.Where(x => x.IsActive && !x.IsDeleted)
                .IncludeIf(withDetails, x => x.FlowSteps);
            return await products.ToListAsync();
        }

        public async Task<Flow> FindIfExistsAsync(Flow flow)
        {
            return await FindAsync(x => (x.Id == flow.Id || x.Code == flow.Code) && !x.IsDeleted);
        }

        public async Task<Flow> FindByCodeAsync(Flow flow)
        {
            return await FindAsync(x => x.Code.Equals(flow.Code) && !x.Id.Equals(flow.Id));
        }

        public async Task<ICollection<Flow>> GetByProductIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.ProductId.Equals(id)).ToListAsync();
        }

        public async Task<Flow> GetByProductIdAndChannelCodeAsync(Guid id, string channelCode)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.ProductId.Equals(id) && x.ChannelCode.Equals(channelCode));
        }

        public async Task<Flow> GetByCodeAsync(string code, bool withInactives = false)
        {
            var dbSet = await GetDbSetAsync();
            if (withInactives)
            {
                return await dbSet.Where(x => x.Code == code && !x.IsDeleted)
                    .Include(x => x.FlowSteps)
                    .FirstOrDefaultAsync();

            }
            return await dbSet.Where(x => x.Code == code && x.IsActive && !x.IsDeleted)
                .Include(x => x.FlowSteps)
                .FirstOrDefaultAsync();
        }

        public async Task<Flow> GetFlowWithDetailsAsync(Guid idFlow)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.Id == idFlow )
                    .Include(x => x.FlowSteps)
                    .FirstOrDefaultAsync();            
            
        }
    }
}

