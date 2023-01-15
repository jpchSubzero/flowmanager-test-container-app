using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateProcessLogDtoValidator : AbstractValidator<CreateProcessLogDto>
    {
        public CreateProcessLogDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.PROCESS_LOG);

            RuleFor(x => x.TrackingId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entity));
            RuleFor(x => x.StepId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entity));
            RuleFor(x => x.Request)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_METHOD, entity, ProcessLogConsts.RequestMaxLength), ProcessLogConsts.RequestMaxLength);
            RuleFor(x => x.Response)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_ERROR, entity, ProcessLogConsts.ResponseMaxLength), ProcessLogConsts.ResponseMaxLength);
            RuleFor(x => x.Action)
                .IsNotRequiredString(localizer.GetString(ErrorConsts.ERROR_DETAIL, entity, ProcessLogConsts.ActionMaxLength), ProcessLogConsts.ActionMaxLength);
        }
    }
}

