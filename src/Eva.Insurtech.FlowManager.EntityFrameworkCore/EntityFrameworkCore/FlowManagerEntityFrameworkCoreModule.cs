using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    [DependsOn(
        typeof(FlowManagerDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class FlowManagerEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<FlowManagerDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
            });
        }
    }
}