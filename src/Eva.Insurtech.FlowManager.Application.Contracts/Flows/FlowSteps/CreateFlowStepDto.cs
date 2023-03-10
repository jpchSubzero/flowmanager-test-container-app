using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.FlowSteps
{
    public class CreateFlowStepDto
    {
        public Guid FlowId { get; private set; }
        public Guid StepId { get; set; }
        public int Order { get; set; }
        public string EndPointService { get; set; }
        public string QueueService { get; set; }
        public bool IsActive { get; set; } = true;
        public int MaxLifeTime { get; set; }
        public void SetFlowId(Guid id)
        {
            FlowId = id;
        }
    }
}
