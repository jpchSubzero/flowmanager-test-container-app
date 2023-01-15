using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateSubStepLogDtoValidator : AbstractValidator<CreateSubStepLogDto>
    {
        public CreateSubStepLogDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.SUB_STEP_LOG);

            RuleFor(x => x.TrackingId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entity));
            RuleFor(x => x.StepId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entity));
            RuleFor(x => x.SubStepCode)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_CODE, entity, SubStepLogConsts.SubStepCodeMaxLength), SubStepLogConsts.SubStepCodeMaxLength);
        }
    }
}

