using System;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Dtos
{
    public class CreatePreTrackingStepDto
    {
        public Guid PreTrackingId { get; private set; }
        public string Container { get; set; }
        public string Component { get; set; }
        public string Method { get; set; }
        public string Body { get; set; }
        public string Observations { get; set; }

        public void SetPreTrackingId(Guid id)
        {
            PreTrackingId = id;
        }
    }
}




