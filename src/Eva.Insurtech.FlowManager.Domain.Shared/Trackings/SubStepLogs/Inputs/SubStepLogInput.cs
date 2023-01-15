using System;

namespace Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs
{
    public class SubStepLogInput
    {
        public virtual Guid TrackingId { get; set; }
        public virtual Guid StepId { get; set; }
        public virtual string SubStepCode { get; set; }
        public virtual int Attempts { get; set; }
    }
}
