using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.PreTrackings
{
    [ExcludeFromCodeCoverage]

    public class PreTrackingRepository : EfCoreRepository<FlowManagerDbContext, PreTracking, Guid>, IPreTrackingRepository
    {
        public PreTrackingRepository(
            IDbContextProvider<FlowManagerDbContext> dbContextProvider
        ) : base(dbContextProvider)
        {
        }

        public async Task<PreTracking> GetByIdAsync(Guid preTrackingId)
        {
            var dbSet = await GetDbSetAsync();
            var prePracking = await dbSet.Where(x => x.Id.Equals(preTrackingId) && !x.IsDeleted).FirstOrDefaultAsync();
            return prePracking;
        }

        public async Task<PreTracking> GetByTransactionReference(string transactionReference)
        {
            var dbSet = await GetDbSetAsync();
            var prePracking = await dbSet.Where(x => x.TransactionReference.Equals(transactionReference) && !x.IsDeleted).FirstOrDefaultAsync();
            return prePracking;
        }
    }
}

