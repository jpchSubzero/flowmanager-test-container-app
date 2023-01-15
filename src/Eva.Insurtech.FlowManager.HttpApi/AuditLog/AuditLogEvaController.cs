using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    [RemoteService]
    [Route("api/auditLog")]
    public class AuditLogEvaController : FlowManagerController, IAuditLogEvaAppService
    {
        private readonly IAuditLogEvaAppService _auditLogAppService;

        public AuditLogEvaController(IAuditLogEvaAppService auditLogAppService)
        {
            _auditLogAppService = auditLogAppService;
        }

        [HttpPost]
        [Route("audit")]
        public Task<Response<AuditLogResponseDto>> InsertAuditAsync(AuditLogInputDto input)
        {
            return _auditLogAppService.InsertAuditAsync(input);
        }
    }
}
