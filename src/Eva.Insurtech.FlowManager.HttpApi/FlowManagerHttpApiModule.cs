using Localization.Resources.AbpUi;
using Eva.Insurtech.FlowManagers.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule))]
    public class FlowManagerHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(FlowManagerHttpApiModule).Assembly);
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<FlowManagerResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
        }
    }
}
