using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Trackings.Inputs
{
    public class FailureLogInput
    {
        public Guid TrackingId { get; set; }
        public string Method { get; set; }
        public Guid StateId { get; set; }
        public Guid StepId { get; set; }
        public string Error { get; set; }
        public string Detail { get; set; }
    }
}
