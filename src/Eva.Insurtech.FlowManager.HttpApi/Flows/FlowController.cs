using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;

namespace Eva.Insurtech.FlowManagers
{
    [RemoteService]
    [Route("api/flow-manager")]
    public class FlowController : FlowManagerController, IFlowAppService
    {
        private readonly IFlowAppService _flowAppService;

        public FlowController(IFlowAppService flowAppService)
        {
            _flowAppService = flowAppService;
        }

        [HttpPost]
        [Route("load-initial-data")]
        public async Task<Response<int>> LoadInitialDataAsync()
        {
            return await _flowAppService.LoadInitialDataAsync();
        }

        #region Basic operations


        [HttpGet]
        public async Task<Response<ICollection<FlowDto>>> GetListAsync(bool withDetails = false)
        {
            return await _flowAppService.GetListAsync(withDetails);
        }

        [HttpGet]
        [Route("{flowId}")]
        public async Task<Response<FlowDto>> GetAsync(Guid flowId)
        {
            Check.NotNull(flowId, nameof(flowId));
            return await _flowAppService.GetAsync(flowId);
        }

        [HttpPost]
        public async Task<Response<FlowDto>> InsertAsync(CreateFlowDto input)
        {
            return await _flowAppService.InsertAsync(input);
        }

        [HttpPut]
        [Route("{flowId}")]
        public async Task<Response<FlowDto>> UpdateAsync(UpdateFlowDto input, Guid flowId)
        {
            Check.NotNull(flowId, nameof(flowId));
            return await _flowAppService.UpdateAsync(input, flowId);
        }

        [HttpDelete]
        [Route("{flowId}")]
        public async Task<Response<bool>> DeleteAsync(Guid flowId)
        {
            return await _flowAppService.DeleteAsync(flowId);
        }

        #endregion

        [HttpPatch]
        [Route("{flowId}/flow-step")]
        public async Task<Response<FlowDto>> AddFlowStep(CreateFlowStepDto input, Guid flowId)
        {
            return await _flowAppService.AddFlowStep(input, flowId);
        }

        [HttpGet]
        [Route("{productId}/product")]
        public async Task<Response<ICollection<FlowDto>>> GetByProductId(Guid productId)
        {
            return await _flowAppService.GetByProductId(productId);
        }

        [HttpGet]
        [Route("{code}/by-code")]
        public async Task<Response<FlowDto>> GetByCodeAsync(string code)
        {
            return await _flowAppService.GetByCodeAsync(code);
        }

        [HttpGet]
        [Route("{productId}/by-product-channel")]
        public async Task<Response<FlowDto>> GetByProductAndChannel(Guid productId, [Required] string channelCode)
        {
            return await _flowAppService.GetByProductAndChannel(productId, channelCode);
        }

        [HttpGet]
        [Route("{trackingId}/by-tracking")]
        public async Task<Response<FlowDto>> GetByTrackingAsync(Guid trackingId)

        {
            return await _flowAppService.GetByTrackingAsync(trackingId);
        }

        [HttpDelete]
        [Route("front/flow-step")]
        public async Task<Response<bool>> DeleteFlowStepAsync(Guid flowId, Guid stepId)
        {
            return await _flowAppService.DeleteFlowStepAsync(flowId, stepId);
        }
    }
}
