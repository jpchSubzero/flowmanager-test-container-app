using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace Eva.Insurtech.FlowManager.MongoDB
{
    [DependsOn(
        typeof(FlowManagerDomainModule),
        typeof(AbpMongoDbModule)
        )]
    public class FlowManagerMongoDbModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddMongoDbContext<FlowManagerMongoDbContext>(options =>
            {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, MongoQuestionRepository>();
                 */
            });
        }
    }
}
