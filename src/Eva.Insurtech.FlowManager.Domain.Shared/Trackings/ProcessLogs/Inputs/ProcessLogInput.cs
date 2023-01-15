using System;

namespace Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs
{
    public class ProcessLogInput
    {
        public Guid TrackingId { get; set; }
        public Guid StepId { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Action { get; set; }
        public int Version { get; set; }
    }
}
