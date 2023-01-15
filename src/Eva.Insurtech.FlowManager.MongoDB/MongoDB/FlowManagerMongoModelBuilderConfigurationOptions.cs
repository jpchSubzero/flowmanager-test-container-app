using JetBrains.Annotations;
using Volo.Abp.MongoDB;

namespace Eva.Insurtech.FlowManager.MongoDB
{
    public class FlowManagerMongoModelBuilderConfigurationOptions : AbpMongoModelBuilderConfigurationOptions
    {
        public FlowManagerMongoModelBuilderConfigurationOptions(
            [NotNull] string collectionPrefix = "")
            : base(collectionPrefix)
        {
        }
    }
}