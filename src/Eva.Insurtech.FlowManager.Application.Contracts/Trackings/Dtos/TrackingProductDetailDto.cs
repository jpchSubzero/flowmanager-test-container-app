using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class TrackingProductDetailDto
    {
        public Guid TrackingId { get; set; }
        public Guid FlowId { get; set; }
        public string FlowCode { get; set; }
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ExternalCode { get; set; }
        public string ProductName { get; set; }

    }
}
