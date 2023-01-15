using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerApplicationContractsModule),
        typeof(AbpHttpClientModule))]
    public class FlowManagerHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "FlowManager";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(FlowManagerApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
