using Eva.Insurtech.FlowManagers.Localization;
using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.RequestLogs.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Validators
{
    public class CreateRequestLogDtoValidator : AbstractValidator<CreateRequestLogDto>
    {
        public CreateRequestLogDtoValidator(IStringLocalizer<FlowManagerResource> localizer)
        {
            var entity = localizer.GetString(LabelConsts.REQUEST_LOG);

            RuleFor(x => x.Service)
                .IsRequiredString(localizer.GetString(ErrorConsts.ERROR_SERVICE, entity, RequestLogConsts.ServiceMaxLength), RequestLogConsts.ServiceMaxLength);
        }
    }
}
