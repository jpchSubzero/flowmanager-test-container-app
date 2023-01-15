using AutoMapper;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using Volo.Abp.AutoMapper;

namespace Eva.Insurtech.FlowManagers.Mappers
{
    public class TrackingProfile : Profile
    {
        public TrackingProfile()
        {
            CreateMap<TrackingDto, Tracking>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ConcurrencyStamp)
                .ReverseMap();

            CreateMap<CreateTrackingDto, Tracking>()
                .IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.Id)
                .Ignore(x => x.Start)
                .Ignore(x => x.End)
                .Ignore(x => x.Abandon)
                .Ignore(x => x.ChangeState)
                .Ignore(x => x.FailureLogs)
                .Ignore(x => x.ProcessLogs)
                .Ignore(x => x.SubStepLogs)
                .Ignore(x => x.ExtraProperties)
                .Ignore(x => x.ConcurrencyStamp);

            CreateMap<CreateFailureLogDto, FailureLog>()
                .ReverseMap();

            CreateMap<FailureLogInput, CreateFailureLogDto>()
                .Ignore(x => x.RegisterTime)
                .ReverseMap();

            CreateMap<CreateProcessLogDto, ProcessLog>()
                .Ignore(x => x.RegisterTime)
                .Ignore(x => x.Version)
                .ReverseMap();

            CreateMap<ProcessLogDto, ProcessLog>()
                .ReverseMap();

            CreateMap<ProcessLogInput, CreateProcessLogDto>()
                .ReverseMap();

            CreateMap<CreateSubStepLogDto, SubStepLog>()
                .Ignore(x => x.RegisterTime)
                .Ignore(x => x.Attempts)
                .ReverseMap();

            CreateMap<SubStepLogDto, SubStepLog>()
                .ReverseMap();

            CreateMap<SubStepLogInput, CreateSubStepLogDto>()
                .ReverseMap();

        }
    }
}