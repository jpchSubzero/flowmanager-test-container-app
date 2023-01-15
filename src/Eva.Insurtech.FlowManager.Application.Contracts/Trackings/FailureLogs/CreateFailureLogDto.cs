using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.FlowSteps
{
    public class CreateFailureLogDto
    {
        public Guid TrackingId { get; private set; }
        public string Method { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
        public Guid StateId { get; set; }
        public Guid StepId { get; set; }
        public DateTime RegisterTime { get; private set; }
        public void SetTrackingId(Guid trackingId)
        {
            TrackingId = trackingId;
        }
    }
}
