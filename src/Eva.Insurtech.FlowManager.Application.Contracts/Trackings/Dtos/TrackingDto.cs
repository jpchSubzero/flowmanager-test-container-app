using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class TrackingDto : EntityDto<Guid>
    {
        public Guid FlowId { get; set; }
        public Guid StepId { get; set; }
        public Guid StateId { get; set; }
        public Guid GeneralStateId { get; set; }
        public string ChannelCode { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? Abandon { get; set; }
        public DateTime ChangeState { get; set; }
        public ICollection<CreateFailureLogDto> FailureLogs { get; set; }
        public ICollection<ProcessLogDto> ProcessLogs { get; set; }
        public ICollection<SubStepLogDto> SubStepLogs { get; set; }
        public ExtraPropertyDictionary ExtraProperties { get; set; }
        public string WayCode { get; set; }
        public string IpClient { get; set; }
    }
}
