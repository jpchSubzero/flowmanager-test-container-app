using AutoMapper;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using Volo.Abp.AutoMapper;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Mapper
{
    public class RequestLogProfile : Profile
    {
        public RequestLogProfile()
        {
            CreateMap<RequestLogDto, RequestLog>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .ReverseMap();
            CreateMap<CreateRequestLogDto, RequestLog>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.Id)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.Requests)
                .Ignore(x => x.Iterations)
                .Ignore(x => x.RegisterDate)
                .ReverseMap();
            CreateMap<RequestDto, Request>()
                .ReverseMap();
            CreateMap<CreateRequestDto, RequestDto>()
                .Ignore(x => x.RegisterDate)
                .ReverseMap();
            CreateMap<RequestDto, RequestInput>()
                .ReverseMap();
            CreateMap<CreateRequestDto, RequestInput>()
                .ReverseMap();
        }
    }
}

