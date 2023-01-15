using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public interface IAuditLogEvaRepository : IRepository<AuditLogEva, Guid>
    {
        Task<ICollection<AuditLogEva>> GetListByIdAsync(Guid trackingId);
    }
}
