using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore
{
    public class FlowManagerModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public FlowManagerModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}