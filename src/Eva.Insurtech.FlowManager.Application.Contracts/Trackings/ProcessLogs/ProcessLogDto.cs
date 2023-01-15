using System;

namespace Eva.Insurtech.FlowManagers.Trackings.ProcessLogs
{
    public class ProcessLogDto
    {
        public Guid TrackingId { get; set; }
        public Guid StepId { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Action { get; set; }
        public DateTime RegisterTime { get; set; }
        public int Version { get; set; }
    }
}



