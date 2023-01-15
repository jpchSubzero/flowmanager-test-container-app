using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers.MessengerApiServices
{
    public interface IMessengerAppService: IApplicationService
    {
        Task<bool> SendReportAsync(MessengerEvaDto dataMenssenger);
    }
}
