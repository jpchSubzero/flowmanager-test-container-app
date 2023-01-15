using Eva.Framework.Utility.Option;
using Eva.Framework.Utility.Option.Contracts;
using Eva.Framework.Utility.Option.Models;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Net.Http;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace Eva.Insurtech.FlowManagers
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAuthorizationModule),
        typeof(FlowManagerDomainModule)
        )]
    public class FlowManagerTestBaseModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAlwaysAllowAuthorization();
            context.Services.AddSingleton<HttpClient>();

            var configuration = context.Services.GetConfiguration();
            context.Services.AddSingleton<IAppConfigurationManager, AppConfigurationManager>();
            context.Services.Configure<AppConfigurationObject>(
                configuration.GetSection("AppConfiguration")
            );
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            SeedTestData(context);
        }

        private static void SeedTestData(ApplicationInitializationContext context)
        {
            AsyncHelper.RunSync(async () =>
            {
                using (var scope = context.ServiceProvider.CreateScope())
                {
                    await scope.ServiceProvider
                        .GetRequiredService<IDataSeeder>()
                        .SeedAsync();
                }
            });
        }
    }
}
