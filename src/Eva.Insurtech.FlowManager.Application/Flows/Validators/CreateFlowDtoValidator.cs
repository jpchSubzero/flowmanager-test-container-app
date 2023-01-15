using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Eva.Insurtech.FlowManagers.Validators
{
    public class CreateFlowDtoValidator : AbstractValidator<CreateFlowDto>
    {
        public CreateFlowDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entityFlow = localizer.GetString(LabelConsts.FLOW);
            var entityProduct = localizer.GetString(LabelConsts.PRODUCT);

            RuleFor(x => x.Code)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_CODE, entityFlow, FlowConsts.CodeMaxLength), FlowConsts.CodeMaxLength);
            RuleFor(x => x.Name)
                .IsName(localizer.GetString(ErrorConsts.ERROR_NAME, entityFlow, FlowConsts.NameMaxLength), FlowConsts.NameMaxLength);
            RuleFor(x => x.Description)
                .IsRequiredDescription(localizer.GetString(ErrorConsts.ERROR_DESCRIPTION, entityFlow, FlowConsts.DescriptionMaxLength), FlowConsts.DescriptionMaxLength);
            RuleFor(x => x.ProductId)
                .IsGuid(localizer.GetString(ErrorConsts.ERROR_ID, entityProduct));
        }
    }
}