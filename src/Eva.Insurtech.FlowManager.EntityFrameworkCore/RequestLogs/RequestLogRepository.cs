using Eva.Framework.Utility.Option;
using Eva.Framework.Utility.Option.Contracts;
using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.RequestLogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.RequestLogs
{
    public class RequestLogRepository : EfCoreRepository<FlowManagerDbContext, RequestLog, Guid>, IRequestLogRepository
    {
        private readonly IAppConfigurationManager _appConfigurationManager;

        public RequestLogRepository(
            IDbContextProvider<FlowManagerDbContext> dbContextProvider,
            IAppConfigurationManager appConfigurationManager
        ) : base(dbContextProvider)
        {
            _appConfigurationManager = appConfigurationManager;
        }

        public async Task<RequestLog> GetByIdAsync(Guid requestLogId)
        {
            var dbSet = await GetDbSetAsync();
            var prePracking = await dbSet.Where(x => x.Id.Equals(requestLogId) && !x.IsDeleted).FirstOrDefaultAsync();
            return prePracking;
        }

        public async Task<RequestLog> GetByService(string service)
        {
            var dbSet = await GetDbSetAsync();
            var prePracking = await dbSet.Where(x => x.Service.Equals(service) && !x.IsDeleted).FirstOrDefaultAsync();
            return prePracking;
        }

        public async Task<RequestLog> GetByServiceToday(string service)
        {
            var today = DateTime.Now.Date;
            var dbSet = await GetDbSetAsync();
            var prePracking = await dbSet.Where(x => x.Service.Equals(service) && x.RegisterDate.Date.Equals(today) && !x.IsDeleted).FirstOrDefaultAsync();
            return prePracking;
        }

        public async Task<ICollection<RequestLog>> GetNotGroupedRequests()
        {
            var now = DateTime.Now;
            var groupedTime = _appConfigurationManager.GetVariableByTypeName<int>(RequestLogConsts.RequestLog, RequestLogConsts.MaxLifeTime);

            var dbSet = await GetDbSetAsync();

            var prePrackings = await dbSet.Where(x => x.Iterations.Equals(0) && !x.IsDeleted).ToListAsync();
            return prePrackings.Where(x => (now - x.RegisterDate).Days > groupedTime).ToList();
        }
    }
}

