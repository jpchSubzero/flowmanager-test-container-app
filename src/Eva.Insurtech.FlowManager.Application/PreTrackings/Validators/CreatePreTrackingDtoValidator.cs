using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Validators
{
    public class CreatePreTrackingDtoValidator : AbstractValidator<CreatePreTrackingDto>
    {
        public CreatePreTrackingDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.PRE_TRACKING);

            RuleFor(x => x.TransactionReference)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_TRANSACTION_REFERENCE), PreTrackingConsts.TransactionReferenceMaxLength);
            RuleFor(x => x.FullName)
                .IsName(localizer.GetString(ErrorConsts.ERROR_NAME, entity, PreTrackingConsts.FullNameMaxLength), PreTrackingConsts.FullNameMaxLength);
            RuleFor(x => x.Identification)
                .IsIdentification(localizer.GetString(ErrorConsts.ERROR_IDENTIFICATION, entity));
            RuleFor(x => x.CellPhone)
                .IsPhone(localizer.GetString(ErrorConsts.ERROR_CELL_PHONE, entity));
            RuleFor(x => x.Email)
                .IsEmail(localizer.GetString(ErrorConsts.ERROR_EMAIL, entity));
        }
    }
}

