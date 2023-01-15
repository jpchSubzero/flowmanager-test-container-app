using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    public class FlowManagerHttpApiHostMigrationsDbContext : AbpDbContext<FlowManagerHttpApiHostMigrationsDbContext>
    {
        public FlowManagerHttpApiHostMigrationsDbContext(DbContextOptions<FlowManagerHttpApiHostMigrationsDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureFlowManager();
        }
    }
}
