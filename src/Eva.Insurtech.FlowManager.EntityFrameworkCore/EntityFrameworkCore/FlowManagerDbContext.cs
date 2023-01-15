using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    [ConnectionStringName(FlowManagerDbProperties.ConnectionStringName)]
    public class FlowManagerDbContext : AbpDbContext<FlowManagerDbContext>, IFlowManagerDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public FlowManagerDbContext(DbContextOptions<FlowManagerDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureFlowManager();
        }
    }
}