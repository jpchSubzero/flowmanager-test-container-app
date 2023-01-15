using Eva.Insurtech.FlowManagers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    [ExcludeFromCodeCoverage]
    public class AuditoLogEvaManager_Tests : FlowManagerDomainTestBase
    {
        private readonly IAuditLogEvaRepository _auditRepository;
        private readonly AuditLogEvaManager _auditLogManager;

        public AuditoLogEvaManager_Tests()
        {
            _auditRepository = GetRequiredService<IAuditLogEvaRepository>();
            _auditLogManager = GetRequiredService<AuditLogEvaManager>();
        }

        [Fact]
        public async Task CreateAuditLog_WithNullData_ReturnNullException()
        {
            AuditLogEva audit = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _auditLogManager.InsertAsync(audit);
            });
        }

        [Fact]
        public async Task CreateAuditLog_WithData_ReturnAudit()
        {
            var audit = await GetCreate();
            var result = await _auditLogManager.InsertAsync(audit);
            Assert.NotNull(result);
            Assert.Equal(audit.TrackingId, result.TrackingId);
        }

        #region Private Methods

        private async Task<AuditLogEva> GetCreate()
        {

            var audit = new AuditLogEva(
                 Guid.Parse("21E7061E-3A84-719F-C163-3A0287A16144"),
                 "Enviar cotización por correo"
            ) ;
            return audit;
        }
        #endregion    
    }
}
