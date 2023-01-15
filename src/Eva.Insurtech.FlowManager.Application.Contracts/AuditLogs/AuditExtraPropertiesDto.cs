using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditExtraPropertiesDto
    {
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string FlowCode { get; set; }
        public Guid FlowId { get; set; }
        public string CurrentStep { get; set; }

    }
}
