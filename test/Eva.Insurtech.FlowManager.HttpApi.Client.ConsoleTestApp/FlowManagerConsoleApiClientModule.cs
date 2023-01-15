using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class FlowManagerConsoleApiClientModule : AbpModule
    {
        
    }
}
