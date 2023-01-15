using Eva.Insurtech.AuditLogEva.AuditLog;
using Eva.Insurtech.FlowManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{

    [ExcludeFromCodeCoverage]
    public class AuditLogAppService_Tests : FlowManagerApplicationTestBase
    {
        private readonly IAuditLogEvaAppService _auditAppService;
        private readonly IAuditLogEvaRepository _auditRepository;
        public AuditLogAppService_Tests()
        {
            _auditAppService = GetRequiredService<IAuditLogEvaAppService>();
            _auditRepository = GetRequiredService<IAuditLogEvaRepository>();
        }

        [Fact]
        public async void Should_Insert_RecordAudit_With_SucessReturn()
        {
            var input = new AuditLogInputDto()
            {
                Action = "Empezar cotización",
                TrackingId = Guid.Parse("298C5E86-9DB7-535F-66A0-3A0484964E4E")
            };
            var result= await _auditAppService.InsertAuditAsync(input);
            Assert.True(result.Success);
        }
    }
}
