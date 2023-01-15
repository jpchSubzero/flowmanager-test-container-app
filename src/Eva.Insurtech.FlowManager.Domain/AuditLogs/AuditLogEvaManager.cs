using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogEvaManager : DomainService
    {
        private readonly IAuditLogEvaRepository _auditLogRepository;

        public AuditLogEvaManager(IAuditLogEvaRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<AuditLogEva> InsertAsync(AuditLogEva auditLog, bool autoSave = true)
        {
            return await _auditLogRepository.InsertAsync(auditLog, autoSave);
        }

        public async Task<ICollection<AuditLogEva>> GetAsync(Guid trackingId)
        {
            var audit = await _auditLogRepository.GetListByIdAsync(trackingId);
            return audit;
        }
    }
}
