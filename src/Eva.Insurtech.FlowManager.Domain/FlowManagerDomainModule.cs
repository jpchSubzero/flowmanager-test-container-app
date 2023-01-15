using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(AbpDddDomainModule),
        typeof(FlowManagerDomainSharedModule)
    )]
    public class FlowManagerDomainModule : AbpModule
    {

    }
}
