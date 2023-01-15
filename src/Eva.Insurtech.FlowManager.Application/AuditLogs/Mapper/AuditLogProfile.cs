using AutoMapper;
using Eva.Insurtech.AuditLogEva.AuditLog;
using Volo.Abp.AutoMapper;

namespace Eva.Insurtech.AuditLogs.AuditLog.Mapper
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            CreateMap<AuditLogInputDto, AuditLogEva.AuditLog.AuditLogEva>()
                 .IgnoreFullAuditedObjectProperties() 
                 .Ignore(x=>x.Id)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x=>x.DateTimeExecution)
                .ReverseMap();

        }
    }
}
