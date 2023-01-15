using Eva.Insurtech.FlowManagers.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers
{
    /* Domain tests are configured to use the EF Core provider.
     * You can switch to MongoDB, however your domain tests should be
     * database independent anyway.
     */
    [DependsOn(
        typeof(FlowManagerEntityFrameworkCoreTestModule)
        )]
    public class FlowManagerDomainTestModule : AbpModule
    {
        
    }
}
