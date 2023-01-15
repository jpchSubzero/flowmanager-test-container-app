using System;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class CreateTrackingRequestDto
    {
        public Guid FlowId { get; private set; }
        public string WayCode { get; set; }
        public string IpClient { get; set; }

        public void SetFlowId(Guid id)
        {
            FlowId = id;
        }
    }
}
