using System;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Eva.Insurtech.FlowManager.MongoDB
{
    public static class FlowManagerMongoDbContextExtensions
    {
        public static void ConfigureFlowManager(
            this IMongoModelBuilder builder,
            Action<AbpMongoModelBuilderConfigurationOptions> optionsAction = null)
        {
            Check.NotNull(builder, nameof(builder));

            var options = new FlowManagerMongoModelBuilderConfigurationOptions(
                FlowManagerDbProperties.DbTablePrefix
            );

            optionsAction?.Invoke(options);
        }
    }
}