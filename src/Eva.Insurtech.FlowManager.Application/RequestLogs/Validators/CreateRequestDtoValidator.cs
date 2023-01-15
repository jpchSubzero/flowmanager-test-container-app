using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Validators
{
    public class CreateRequestDtoValidator : AbstractValidator<CreateRequestDto>
    {
        public CreateRequestDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.REQUEST_LOG);

            RuleFor(x => x.TransactionReference)
                .IsCode(localizer.GetString(ErrorConsts.ERROR_TRANSACTION_REFERENCE), RequestLogConsts.TransactionReferenceMaxLength);
            RuleFor(x => x.Service)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_SERVICE, entity, RequestLogConsts.ServiceMaxLength), RequestLogConsts.ServiceMaxLength);
            RuleFor(x => x.Body)
                .IsRequiredStringMaxSize(localizer.GetString(ErrorConsts.ERROR_BODY, entity));
        }
    }
}

