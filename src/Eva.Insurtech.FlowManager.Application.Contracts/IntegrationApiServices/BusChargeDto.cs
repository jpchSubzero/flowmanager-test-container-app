using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Eva.Insurtech.FlowManagers.IntegrationApiServices
{
    public class BusChargeDto : EntityDto<Guid>
    {
        public Guid TrackingId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
        public int Attempts { get; set; }
    }
}
