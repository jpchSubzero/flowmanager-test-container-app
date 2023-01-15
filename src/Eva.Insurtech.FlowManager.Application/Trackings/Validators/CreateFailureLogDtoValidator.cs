using Eva.Insurtech.FlowManagers.FailureLogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.Trackings;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateFailureLogDtoValidator : AbstractValidator<CreateFailureLogDto>
    {
        public CreateFailureLogDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.FAILURE_LOG);

            RuleFor(x => x.TrackingId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entity));
            RuleFor(x => x.Method)
                .IsName(localizer.GetString(ErrorConsts.ERROR_METHOD, entity, FailureLogConsts.NameMaxLength), FailureLogConsts.NameMaxLength);
            RuleFor(x => x.Error)
                .IsRequiredDescription(localizer.GetString(ErrorConsts.ERROR_ERROR, entity, FailureLogConsts.DescriptionMaxLength), FailureLogConsts.DescriptionMaxLength);
            RuleFor(x => x.Detail)
                .IsRequiredDescription(localizer.GetString(ErrorConsts.ERROR_DETAIL, entity, FailureLogConsts.ExceptionMaxLength), FailureLogConsts.ExceptionMaxLength);
        }
    }
}