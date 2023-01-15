using Eva.Framework.Utility.Response.Models;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public interface  IAuditLogEvaAppService : IApplicationService
    {
        Task<Response<AuditLogResponseDto>> InsertAuditAsync(AuditLogInputDto input);

    }
}
