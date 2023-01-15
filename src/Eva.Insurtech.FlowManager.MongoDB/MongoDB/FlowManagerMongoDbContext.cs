using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Eva.Insurtech.FlowManager.MongoDB
{
    [ConnectionStringName(FlowManagerDbProperties.ConnectionStringName)]
    public class FlowManagerMongoDbContext : AbpMongoDbContext, IFlowManagerMongoDbContext
    {
        /* Add mongo collections here. Example:
         * public IMongoCollection<Question> Questions => Collection<Question>();
         */

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.ConfigureFlowManager();
        }
    }
}