using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.AppApiServices
{
    public interface IAppApiServiceManager:IApplicationService
    {
        Task<JObject> SubmitAsync(RequestDataDto requestData, bool returnWithError = false);
        Task<JObject> QueryAsync(RequestDataDto requestData, bool returnWithError = false);
    }
}
