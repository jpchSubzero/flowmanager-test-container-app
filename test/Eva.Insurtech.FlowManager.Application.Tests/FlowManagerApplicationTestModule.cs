using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerApplicationModule),
        typeof(FlowManagerDomainTestModule)
        )]
    public class FlowManagerApplicationTestModule : AbpModule
    {

    }
}
