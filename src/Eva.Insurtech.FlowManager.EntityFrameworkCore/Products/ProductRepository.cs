using Eva.Insurtech.FlowManagers.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Products
{
    public class ProductRepository : EfCoreRepository<FlowManagerDbContext, Product, Guid>, IProductRepository
    {
        public ProductRepository(IDbContextProvider<FlowManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<Product> GetByExternalIdAsync(Guid id)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.ProductId.Equals(id));
        }

        public async Task<Product> GetByCodeAsync(string code)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.FirstOrDefaultAsync(x => x.Code.Equals(code));
        }

        public async Task<ICollection<Product>> GetAllAsync()
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.ToListAsync();
        }
    }
}
