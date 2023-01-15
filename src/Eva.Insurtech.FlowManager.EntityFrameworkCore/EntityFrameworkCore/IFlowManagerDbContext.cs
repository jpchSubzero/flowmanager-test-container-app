using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    [ConnectionStringName(FlowManagerDbProperties.ConnectionStringName)]
    public interface IFlowManagerDbContext : IEfCoreDbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * DbSet<Question> Questions { get; }
         */
    }
}