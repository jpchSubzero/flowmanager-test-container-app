using System;

namespace Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps.Dtos
{
    public class PreTrackingStepDto
    {
        public Guid PreTrackingId { get; set; }
        public string Container { get; set; }
        public string Component { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public int Iterations { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Observations { get; set; }

    }
}






