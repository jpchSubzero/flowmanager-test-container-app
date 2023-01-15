using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.Products
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<Product> GetByIdAsync(Guid id);
        Task<Product> GetByExternalIdAsync(Guid id);
        Task<Product> GetByCodeAsync(string code);
        Task<ICollection<Product>> GetAllAsync();
    }
}
