using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Flows.Inputs;
using Eva.Insurtech.FlowManagers.Flows.Seeders;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.TrackingManagers.Trackings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Eva.Insurtech.Flows
{
    public class FlowAppService : FlowManagerAppService, IFlowAppService
    {
        private readonly FlowManager _flowManager;
        private readonly ProductManager _productManager;
        private readonly CatalogManager _catalogManager;
        private readonly TrackingManager _trackingManager;
        private readonly IObjectMapper _objectMapper;

        public FlowAppService(
            FlowManager flowManager,
            ProductManager productManager,
            CatalogManager catalogManager,
            TrackingManager trackingManager,
            IObjectMapper objectMapper,
            ILogger<FlowAppService> logger
        ) : base(logger)
        {
            _flowManager = flowManager;
            _productManager = productManager;
            _catalogManager = catalogManager;
            _objectMapper = objectMapper;
            _trackingManager = trackingManager;

        }

        public async Task<Response<FlowDto>> InsertAsync(CreateFlowDto input)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                var flow = _objectMapper.Map<CreateFlowDto, Flow>(input);

                await ValidateRequiredIds(input);

                flow = await _flowManager.InsertAsync(flow);
                return response.OnSuccess(_objectMapper.Map<Flow, FlowDto>(flow));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<FlowDto>> UpdateAsync(UpdateFlowDto input, Guid flowId)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                input.SetId(flowId);
                var flow = _objectMapper.Map<UpdateFlowDto, Flow>(input);

                await _productManager.ValidateIfExistInProduct(input.ProductId);

                flow = await _flowManager.UpdateAsync(flow);
                return response.OnSuccess(_objectMapper.Map<Flow, FlowDto>(flow));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool>> DeleteAsync(Guid flowId)
        {
            ResponseManager<bool> response = new();
            try
            {
                return response.OnSuccess(await _flowManager.DeleteAsync(flowId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<ICollection<FlowDto>>> GetListAsync(bool withDetails = false)
        {
            ResponseManager<ICollection<FlowDto>> response = new();
            try
            {
                ICollection<FlowDto> flows = _objectMapper.Map<ICollection<Flow>, ICollection<FlowDto>>(await _flowManager.GetListAsync(withDetails));

                return response.OnSuccess(flows);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<ICollection<FlowDto>>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<FlowDto>> GetAsync(Guid flowId)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                FlowDto flow = _objectMapper.Map<Flow, FlowDto>(await _flowManager.GetAsync(flowId));
                return response.OnSuccess(flow);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<FlowDto>> AddFlowStep(CreateFlowStepDto input, Guid flowId)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                await _catalogManager.ValidateIfExistInCatalog(FlowConsts.FLOW_STEPS, input.StepId);

                input.SetFlowId(flowId);
                var flowStepInput = _objectMapper.Map<CreateFlowStepDto, FlowStepInput>(input);
                var flow = await _flowManager.AddFlowStep(flowStepInput);
                return response.OnSuccess(_objectMapper.Map<Flow, FlowDto>(flow));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<ICollection<FlowDto>>> GetByProductId(Guid productId)
        {
            ResponseManager<ICollection<FlowDto>> response = new();
            try
            {
                var flows = _objectMapper.Map<ICollection<Flow>, ICollection<FlowDto>>(await _flowManager.GetByProductIdAsync(productId));
                return response.OnSuccess(flows);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<ICollection<FlowDto>>(FlowErrorCodes.GetErrorGeneral(), ex);
            }

        }

        public async Task<Response<FlowDto>> GetByCodeAsync(string code)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                FlowDto flow = _objectMapper.Map<Flow, FlowDto>(await _flowManager.GetByCodeAsync(code));
                return response.OnSuccess(flow);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<FlowDto>> GetByProductAndChannel(Guid productId, string channelCode)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                await _productManager.ValidateIfExistInProduct(productId);
                await _productManager.ValidateIfChannelExistInProduct(channelCode);

                var flow = _objectMapper.Map<Flow, FlowDto>(await _flowManager.GetByProductIdAndChannelCodeAsync(productId, channelCode));
                return response.OnSuccess(flow);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<int>> LoadInitialDataAsync()
        {
            ResponseManager<int> response = new();
            try
            {
                return response.OnSuccess(await FlowSeeder.LoadInitialData(_productManager, _catalogManager, _flowManager));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<int>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<FlowDto>> GetByTrackingAsync(Guid trackingId)
        {
            ResponseManager<FlowDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                if (tracking == null)
                    return response.OnError(TrackingErrorCodes.GetErrorNotFoundById());
                FlowDto flow = _objectMapper.Map<Flow, FlowDto>(await _flowManager.GetAsync(tracking.FlowId));
                return response.OnSuccess(flow);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<FlowDto>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool>> DeleteFlowStepAsync(Guid flowId, Guid flowStepId)
        {
            ResponseManager<bool> response = new();
            try
            {
                return response.OnSuccess(await _flowManager.DeleteFlowStepAsync(flowId,flowStepId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool>(FlowErrorCodes.GetErrorGeneral(), ex);
            }
        }

        #region Private Methods

        private async Task ValidateRequiredIds(CreateFlowDto input)
        {
            await _productManager.ValidateIfExistInProduct(input.ProductId);
            await _productManager.ValidateIfChannelExistInProduct(input.ChannelCode);
        }

        

        #endregion

    }
}