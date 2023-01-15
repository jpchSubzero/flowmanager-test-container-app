using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public interface ITrackingRepository : IRepository<Tracking, Guid>
    {
        Task<Tracking> GetByIdAsync(Guid trackingId);
        Task<ICollection<Tracking>> GetAllAsync(bool withDetails = false);
        Task<Tracking> FindIfExistsAsync(Tracking tracking);
        Task<Tracking> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties);
        Task<Tracking> GetByChannelExtraPropertiesAsync(string channelCode, ExtraPropertyDictionary extraProperties);
        Task<Tracking> GetByWayChannelExtraPropertiesAsync(string wayCode, string channelCode, ExtraPropertyDictionary extraProperties);
        Task<ICollection<Tracking>> GetByIpClientAsync(string ip);
    }
}
