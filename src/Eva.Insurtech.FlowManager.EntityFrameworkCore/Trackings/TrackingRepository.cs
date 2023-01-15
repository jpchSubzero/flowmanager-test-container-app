using Eva.Insurtech.FlowManagers.Trackings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.ObjectExtending;
using static System.Net.Mime.MediaTypeNames;

namespace Eva.Insurtech.FlowManagers.EntityFrameworkCore.Trackings
{
    public class TrackingRepository : EfCoreRepository<FlowManagerDbContext, Tracking, Guid>, ITrackingRepository
    {
        private const int _registerToTake = 1000;

        public TrackingRepository(IDbContextProvider<FlowManagerDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<Tracking> FindIfExistsAsync(Tracking tracking)
        {
            return await FindAsync(x => (x.Id == tracking.Id) && !x.IsDeleted);
        }

        public async Task<ICollection<Tracking>> GetAllAsync(bool withDetails = false)
        {
            var dbSet = await GetDbSetAsync();
            var products = dbSet.Where(x => !x.IsDeleted);
            return await products.ToListAsync();
        }

        public async Task<Tracking> GetByIdAsync(Guid trackingId)
        {
            var dbSet = await GetDbSetAsync();
            var flow = await dbSet.Where(x => x.Id == trackingId && !x.IsDeleted)
                .FirstOrDefaultAsync();
            return flow;
        }

        public async Task<Tracking> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties)
        {
            Tracking tracking = null;
            int tries = 0;

            var query = await GetQueryableAsync();

            while (tracking == null && (tries * _registerToTake < query.Count()))
            {
                tracking = query.OrderByDescending(x => x.CreationTime).Skip(tries * _registerToTake).Take(_registerToTake).AsEnumerable().FirstOrDefault(x => !x.IsDeleted && x.ExtraProperties.Any(y => extraProperties.Contains(y)));
                tries++;
            }
            return tracking;
        }

        public async Task<Tracking> GetByChannelExtraPropertiesAsync(string channelCode, ExtraPropertyDictionary extraProperties)
        {
            Tracking tracking = null;
            int tries = 0;

            var query = (await GetQueryableAsync()).Where(x => x.ChannelCode.Equals(channelCode));
            while (tracking == null && (tries * _registerToTake < query.Count()))
            {
                tracking = query.OrderByDescending(x => x.CreationTime).Skip(tries * _registerToTake).Take(_registerToTake).AsEnumerable().FirstOrDefault(x => !x.IsDeleted && x.ExtraProperties.Any(y => extraProperties.Contains(y)));
                tries++;
            }
            return tracking;
        }

        public async Task<Tracking> GetByWayChannelExtraPropertiesAsync(string wayCode, string channelCode, ExtraPropertyDictionary extraProperties)
        {
            Tracking tracking = null;
            int tries = 0;

            var query = (await GetQueryableAsync()).Where(x => x.ChannelCode.Equals(channelCode) && x.WayCode.Equals(wayCode));

            while (tracking == null && (tries * _registerToTake < query.Count()))
            {
                tracking = query.OrderByDescending(x => x.CreationTime).Skip(tries * _registerToTake).Take(_registerToTake).AsEnumerable().FirstOrDefault(x => !x.IsDeleted && x.ExtraProperties.Any(y => extraProperties.Contains(y)));
                tries++;
            }
            return tracking;
        }

        public async Task<ICollection<Tracking>> GetByIpClientAsync(string ip)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.Where(x => x.IpClient.Equals(ip) && x.Start.Date.Equals(DateTime.Now.Date) && !x.IsDeleted).ToListAsync();
        }
    }
}

