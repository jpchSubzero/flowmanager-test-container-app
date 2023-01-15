using System;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Inputs
{
    public class PreTrackingStepInput
    {
        public Guid PreTrackingId { get; set; }
        public string Container { get; set; }
        public string Component { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public string Observations { get; set; }
    }
}
