using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerDomainSharedModule),
        typeof(AbpDddApplicationContractsModule),
        typeof(AbpAuthorizationModule)
        )]
    public class FlowManagerApplicationContractsModule : AbpModule
    {

    }
}
