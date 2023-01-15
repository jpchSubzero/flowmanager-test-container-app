using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eva.Insurtech.AuditLogEva.AuditLog
          
{
    public class AuditLogEva : FullAuditedAggregateRoot<Guid>
    {
        public Guid TrackingId { get; set; }
        public string Action { get; set; }
        public DateTime DateTimeExecution { get; set; }

        public AuditLogEva(Guid trackingId,string action)
        {
            this.TrackingId = trackingId;
            this.Action = action;
            this.DateTimeExecution = DateTime.Now;
        }

    }
}
