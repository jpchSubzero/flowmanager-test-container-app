using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Eva.Insurtech.FlowManagers.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;
using Microsoft.Extensions.Localization;
using Volo.Abp;
using Microsoft.Extensions.DependencyInjection;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Flows.FailureLogs;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(AbpValidationModule)
    )]
    public class FlowManagerDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<FlowManagerDomainSharedModule>();
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<FlowManagerResource>("es")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/Error")
                    .AddVirtualJson("/Localization/FlowManager");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("FlowManager", typeof(FlowManagerResource));
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {

            //Inicializa el StringLocalizer en ConfigurationErrorCodes
            var stringLocalizer = context.ServiceProvider.GetRequiredService<IStringLocalizer<FlowManagerResource>>();

            FlowStepErrorCodes.InitStringLocalizer(stringLocalizer);
            FlowErrorCodes.InitStringLocalizer(stringLocalizer);
            TrackingErrorCodes.InitStringLocalizer(stringLocalizer);
            CatalogErrorCodes.InitStringLocalizer(stringLocalizer);
            ProductErrorCodes.InitStringLocalizer(stringLocalizer);
            FailureLogErrorCodes.InitStringLocalizer(stringLocalizer);
        }

    }
}
