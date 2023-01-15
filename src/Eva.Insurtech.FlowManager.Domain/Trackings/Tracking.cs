using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using JetBrains.Annotations;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class Tracking : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid FlowId { get; private set; }
        public virtual Guid StepId { get; private set; }
        public virtual Guid StateId { get; private set; }
        public virtual Guid GeneralStateId { get; private set; }
        public virtual string ChannelCode { get; private set; }
        public virtual DateTime Start { get; private set; }
        public virtual DateTime? End { get; private set; }
        public virtual DateTime? Abandon { get; private set; }
        public virtual DateTime ChangeState { get; private set; }
        public virtual ICollection<FailureLog> FailureLogs { get; private set; }
        public virtual ICollection<ProcessLog> ProcessLogs { get; private set; }
        public virtual ICollection<SubStepLog> SubStepLogs { get; private set; }
        public virtual string WayCode { get; private set; }
        public virtual string IpClient { get; private set; }


        protected Tracking()
        {
        }

        public Tracking(
            [NotNull] Guid flowId,
            [NotNull] Guid stepId,
            [NotNull] Guid stateId,
            [NotNull] Guid generalStateId,
            [NotNull] string channelCode,
            [NotNull] string wayCode,
            [NotNull] string ipClient
        )
        {
            FlowId = flowId;
            StepId = stepId;
            StateId = stateId;
            GeneralStateId = generalStateId;
            ChannelCode = channelCode;
            WayCode = wayCode;
            IpClient = ipClient;
            Start = DateTime.Now;
            ChangeState = DateTime.Now;
        }

        public Tracking(TrackingInput input)
        {
            FlowId = input.FlowId;
            StepId = input.StepId;
            StateId = input.StateId;
            GeneralStateId = input.GeneralStateId;
            ChannelCode = input.ChannelCode;
            WayCode = input.WayCode;
            IpClient = input.IpClient;
            Start = DateTime.Now;
            ChangeState = DateTime.Now;
        }

        public void SetFlowId(Guid input)
        {
            FlowId = input;
        }

        public void SetStateId(Guid input)
        {
            StateId = input;
        }

        public void SetStepId(Guid input)
        {
            StepId = input;
        }

        public void SetGeneralStateId(Guid input)
        {
            GeneralStateId = input;
        }

        public void SetExtraProperties(ExtraPropertyDictionary extraProperties)
        {
            ExtraProperties = extraProperties;
        }

        public void UpdateChangeStateDate()
        {
            ChangeState = DateTime.Now;
        }
        
        public void UpdateAbandonDate()
        {
            Abandon = DateTime.Now;
        }
        public void UpdateEndDate()
        {
            End = DateTime.Now;
        }

        public void AddFailureLogs(FailureLogInput failureLog)
        {
            FailureLogs ??= new List<FailureLog>();
            FailureLogs.Add(new FailureLog(failureLog));
        }

        public void SetFailureLogs(ICollection<FailureLog> failureLogs)
        {
            FailureLogs = failureLogs;
        }
        public void SetProccessLogs(ICollection<ProcessLog> processLogs)
        { 
            ProcessLogs = processLogs;
        }

        public void SetSubStepLogs(ICollection<SubStepLog> subStepLogs)
        { 
            SubStepLogs = subStepLogs;
        }

        public void AddProcessLogs(ProcessLogInput processLog)
        {
            ProcessLogs ??= new List<ProcessLog>();
            processLog.Version = ProcessLogs.Any() ? ProcessLogs.Max(x => x.Version) + 1 : 1;
            ProcessLogs.Add(new ProcessLog(processLog));
        }

        public void AddSubStepLogs(SubStepLogInput subStepLog)
        {
            SubStepLogs ??= new List<SubStepLog>();
            var subSteps = SubStepLogs.Where(x => x.TrackingId.Equals(subStepLog.TrackingId) && x.StepId.Equals(subStepLog.StepId) && x.SubStepCode.Equals(subStepLog.SubStepCode));
            subStepLog.Attempts = subSteps.Any() ? subSteps.Max(y => y.Attempts) + 1 : 1;
            SubStepLogs.Add(new SubStepLog(subStepLog));
        }
    }
}






