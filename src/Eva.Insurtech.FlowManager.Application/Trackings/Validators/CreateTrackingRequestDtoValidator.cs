using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.Trackings;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateTrackingRequestDtoValidator : AbstractValidator<CreateTrackingRequestDto>
    {
        public CreateTrackingRequestDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.TRACKING);

            RuleFor(x => x.FlowId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength));
            RuleFor(x => x.WayCode)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_CODE, entity, TrackingConsts.CodeMaxLength), TrackingConsts.CodeMaxLength);
            RuleFor(x => x.IpClient)
                .IsIpV4(localizer.GetString(ErrorConsts.ERROR_IP_V4_ADDRESS, entity));
        }
    }
}