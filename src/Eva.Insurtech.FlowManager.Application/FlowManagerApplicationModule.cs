using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using Volo.Abp.FluentValidation;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(FlowManagerDomainModule),
        typeof(FlowManagerApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpFluentValidationModule)
        )]
    public class FlowManagerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<FlowManagerApplicationModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<FlowManagerApplicationModule>(validate: true);
            });
        }
    }
}
