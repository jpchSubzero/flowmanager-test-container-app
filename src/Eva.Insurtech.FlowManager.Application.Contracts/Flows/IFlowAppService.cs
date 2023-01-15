using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public interface IFlowAppService : IApplicationService
    {
        Task<Response<FlowDto>> InsertAsync(CreateFlowDto input);
        Task<Response<FlowDto>> UpdateAsync(UpdateFlowDto input, Guid flowId);
        Task<Response<bool>> DeleteAsync(Guid flowId);
        Task<Response<ICollection<FlowDto>>> GetListAsync(bool withDetails = false);
        Task<Response<FlowDto>> GetAsync(Guid flowId);
        Task<Response<FlowDto>> AddFlowStep(CreateFlowStepDto input, Guid flowId);
        Task<Response<ICollection<FlowDto>>> GetByProductId(Guid productId);
        Task<Response<FlowDto>> GetByCodeAsync(string code);
        Task<Response<FlowDto>> GetByProductAndChannel(Guid productId, string channelCode);
        Task<Response<int>> LoadInitialDataAsync();
        Task<Response<FlowDto>> GetByTrackingAsync(Guid trackingId);
        Task<Response<bool>> DeleteFlowStepAsync(Guid flowId,Guid flowStepId);
    }
}
