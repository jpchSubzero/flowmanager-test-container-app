using System;

namespace Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs
{
    public class CreateSubStepLogDto
    {
        public virtual Guid TrackingId { get; private set; }
        public virtual Guid StepId { get; private set; }
        public virtual string SubStepCode { get; set; }
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



