using System;

namespace Eva.Insurtech.FlowManagers.Flows.Inputs
{
    public class FlowStepInput
    {
        public Guid FlowId { get; set; }
        public Guid StepId { get; set; }
        public int Order { get; set; }
        public string EndPointService { get; set; }
        public string QueueService { get; set; }
        public bool IsActive { get; set; } = true;
        public int MaxLifeTime { get; set; }

        public FlowStepInput()
        {

        }

        public FlowStepInput(Guid flowId, Guid stepId, int order, string endPointService, string queueService, bool isActive = true, int maxLifeTime = 0)
        {
            FlowId = flowId;
            StepId = stepId;
            Order = order;
            EndPointService = endPointService;
            QueueService = queueService;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
        }

        public FlowStepInput(Guid stepId, int order, string endPointService, string queueService, bool isActive = true, int maxLifeTime = 0)
        {
            StepId = stepId;
            Order = order;
            EndPointService = endPointService;
            QueueService = queueService;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
        }
    }
}
