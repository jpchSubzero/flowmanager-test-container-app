using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Validators
{
    public class CreatePreTrackingStepDtoValidator : AbstractValidator<CreatePreTrackingStepDto>
    {
        public CreatePreTrackingStepDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.PRE_TRACKING);

            RuleFor(x => x.Container)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_CONTAINER, entity, PreTrackingConsts.ContainerMaxLength), PreTrackingConsts.ContainerMaxLength);
            RuleFor(x => x.Component)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_COMPONENT, entity, PreTrackingConsts.ComponentMaxLength), PreTrackingConsts.ComponentMaxLength);
            RuleFor(x => x.Method)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_METHOD, entity, PreTrackingConsts.MethodMaxLength), PreTrackingConsts.MethodMaxLength);
            RuleFor(x => x.Body)
                .IsRequiredStringMaxSize(localizer.GetString(ErrorConsts.ERROR_BODY, entity));
        }
    }
}
