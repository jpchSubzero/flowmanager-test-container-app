using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace Eva.Insurtech.FlowManager.MongoDB
{
    [ConnectionStringName(FlowManagerDbProperties.ConnectionStringName)]
    public interface IFlowManagerMongoDbContext : IAbpMongoDbContext
    {
        /* Define mongo collections here. Example:
         * IMongoCollection<Question> Questions { get; }
         */
    }
}
