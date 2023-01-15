using System;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class CreateTrackingDto
    {
        public Guid FlowId { get; set; }
        public Guid StepId { get; set; }
        public Guid StateId { get; set; }
        public Guid GeneralStateId { get; set; }
        public string ChannelCode { get; set; }
        public string WayCode { get; set; }
        public string IpClient { get; set; }
    }
}
