using AutoMapper;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps.Dtos;
using Volo.Abp.AutoMapper;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Mappers
{
    public class PreTrackingProfile : Profile
    {
        public PreTrackingProfile()
        {
            CreateMap<PreTrackingDto,PreTracking>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .ReverseMap();
            CreateMap<CreatePreTrackingDto, PreTracking>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.Id)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .Ignore(x => x.PreTrackingSteps)
                .Ignore(x => x.TrackingId)
                .ReverseMap();
            CreateMap<PreTrackingStepDto, PreTrackingStep>()
                .ReverseMap();
            CreateMap<CreatePreTrackingStepDto, PreTrackingStepDto>()
                .Ignore(x => x.RegisterDate)
                .Ignore(x => x.Iterations)
                .ReverseMap();
            CreateMap<PreTrackingStepDto, PreTrackingStepInput>()
                .ReverseMap();
            CreateMap<CreatePreTrackingStepDto, PreTrackingStepInput>()
                .ReverseMap();
        }
    }
}

