using System;

namespace Eva.Insurtech.FlowManagers.Trackings.ProcessLogs
{
    public class CreateProcessLogDto
    {
        public Guid TrackingId { get; private set; }
        public Guid StepId { get; private set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Action { get; set; }
        public void SetTrackingId(Guid trackingId)
        {
            TrackingId = trackingId;
        }
        public void SetStepId(Guid stepId)
        {
            StepId = stepId;
        }
    }
}



