using Eva.Insurtech.FlowManagers.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogEvaRepository : EfCoreRepository<FlowManagerDbContext, AuditLogEva, Guid>, IAuditLogEvaRepository
    {
        public AuditLogEvaRepository(IDbContextProvider<FlowManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<ICollection<AuditLogEva>> GetListByIdAsync(Guid trackingId)
        {
            var dbSet = await GetDbSetAsync();
            var audit = dbSet.Where(x => x.TrackingId == trackingId);
            return await audit.ToListAsync();
        }
    }
}

