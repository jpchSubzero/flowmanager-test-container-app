using Eva.Insurtech.FlowManagers.Flows.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class FlowStep : ValueObject
    {
        public virtual Guid FlowId { get; private set; }
        public virtual Guid StepId { get; private set; }
        public virtual int Order { get; private set; }
        public virtual string EndPointService { get; private set; }
        public virtual string QueueService { get; private set; }
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Tiempo máximo para completar el flujo. 0 es sin límite
        /// </summary>
        public virtual int MaxLifeTime { get; private set; }

        protected FlowStep()
        {
        }

        public FlowStep(
            [NotNull] Guid flowId,
            [NotNull] Guid stepId,
            [NotNull] int order,
            [NotNull][MaxLength(FlowStepConsts.EndPointMaxLength)] string endPointService,
            [NotNull][MaxLength(FlowStepConsts.QueueMaxLength)] string queueService,
            [NotNull] bool isActive = true,
            int maxLifeTime = 0
        )
        {
            FlowId = flowId;
            StepId = stepId;
            Order = order;
            EndPointService = endPointService;
            QueueService = queueService;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
        }

        public FlowStep(
            [NotNull] Guid stepId,
            [NotNull] int order,
            [NotNull][MaxLength(FlowStepConsts.EndPointMaxLength)] string endPointService,
            [NotNull][MaxLength(FlowStepConsts.QueueMaxLength)] string queueService,
            [NotNull] bool isActive = true,
            int maxLifeTime = 0
        )
        {
            StepId = stepId;
            Order = order;
            EndPointService = endPointService;
            QueueService = queueService;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
        }

        public FlowStep(FlowStepInput input)
        {
            FlowId = input.FlowId;
            StepId = input.StepId;
            Order = input.Order;
            EndPointService = input.EndPointService;
            QueueService = input.QueueService;
            IsActive = input.IsActive;
            MaxLifeTime = input.MaxLifeTime;
        }

        public void SetFlowId(Guid input)
        {
            FlowId = input;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return FlowId;
            yield return StepId;
            yield return Order;
            yield return EndPointService;
            yield return QueueService;
            yield return IsActive;
            yield return MaxLifeTime;
        }
    }
}