using Eva.Insurtech.FlowManagers.Catalogs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Catalogs
{
    public class CatalogRepository : EfCoreRepository<FlowManagerDbContext, Catalog, Guid>, ICatalogRepository
    {
        public CatalogRepository(IDbContextProvider<FlowManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<Catalog> GetByExternalIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.CatalogId.Equals(id));
        }

        public async Task<Catalog> GetByCodeAsync(string code)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.Code.Equals(code));
        }

        public async Task<ICollection<Catalog>> GetAllAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.ToListAsync();
        }
    }
}
