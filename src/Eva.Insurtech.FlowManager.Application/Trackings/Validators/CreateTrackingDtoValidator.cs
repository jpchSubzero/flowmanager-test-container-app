using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.Trackings;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateTrackingDtoValidator : AbstractValidator<CreateTrackingDto>
    {
        public CreateTrackingDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.TRACKING);

            RuleFor(x => x.FlowId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength));
            RuleFor(x => x.StepId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength));
            RuleFor(x => x.StateId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength));
            RuleFor(x => x.GeneralStateId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength));
            RuleFor(x => x.ChannelCode)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength), TrackingConsts.CodeMaxLength);
        }
    }
}