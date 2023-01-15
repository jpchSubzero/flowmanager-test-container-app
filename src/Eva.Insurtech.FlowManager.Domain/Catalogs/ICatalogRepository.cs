using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.Catalogs
{
    public interface ICatalogRepository : IRepository<Catalog, Guid>
    {
        Task<Catalog> GetByExternalIdAsync(Guid id);
        Task<Catalog> GetByCodeAsync(string code);
        Task<ICollection<Catalog>> GetAllAsync();
    }
}
