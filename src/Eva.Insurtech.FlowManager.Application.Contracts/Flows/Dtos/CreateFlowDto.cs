using System;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class CreateFlowDto
    {
        public string Code { get; set; }
        public string ChannelCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid ProductId { get; set; }
        public int MaxLifeTime { get; set; }
        public int OtpMaxTime { get; set; }
        public int OtpMaxAttempts { get; set; }
        public int OtpMaxResends { get; set; }
    }
}
