using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class FlowDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string ChannelCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public Guid ProductId { get; set; }
        public ICollection<FlowStepDto> FlowSteps { get; set; }
        public int MaxLifeTime { get; set; }
        public int OtpMaxTime { get; set; }
        public int OtpMaxAttempts { get; set; }
        public int OtpMaxResends { get; set; }
    }
}
