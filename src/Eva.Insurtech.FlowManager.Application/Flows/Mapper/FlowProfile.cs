using AutoMapper;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Flows.Inputs;
using Volo.Abp.AutoMapper;

namespace Eva.Insurtech.FlowManagers.Mappers
{
    public class FlowProfile : Profile
    {
        public FlowProfile()
        {
            CreateMap<FlowDto, Flow>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp)
                .ReverseMap();
            CreateMap<CreateFlowStepDto, FlowStep>()
                .ReverseMap();
            CreateMap<CreateFlowStepDto, FlowStepInput>()
                .Ignore(x => x.IsActive);

            CreateMap<FlowStepDto, FlowStep>()
                .ReverseMap();

            CreateMap<CreateFlowDto, Flow>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.Id)
                .Ignore(x => x.FlowSteps)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp);

            CreateMap<UpdateFlowDto, Flow>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.FlowSteps)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp);
        }
    }
}