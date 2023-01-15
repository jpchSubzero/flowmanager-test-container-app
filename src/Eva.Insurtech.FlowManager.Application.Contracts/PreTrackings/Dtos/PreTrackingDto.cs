using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps.Dtos;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Dtos
{
    public class PreTrackingDto : EntityDto<Guid>
    {
        public Guid? TrackingId { get; set; }
        public string TransactionReference { get; set; }
        public string Identification { get; set; }
        public string FullName { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public ICollection<PreTrackingStepDto> PreTrackingSteps { get; set; }

    }
}




