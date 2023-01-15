using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using Eva.Insurtech.FlowManagers.PreTrackings.Exceptions;
using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using Eva.Insurtech.Flows;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    public class PreTrackingAppService : FlowManagerAppService, IPreTrackingAppService
    {
        private readonly PreTrackingManager _preTrackingManager;
        private readonly IObjectMapper _objectMapper;

        public PreTrackingAppService(
            ILogger<FlowAppService> logger,
            IObjectMapper objectMapper,
            PreTrackingManager preTrackingManager
        ) : base(logger)
        {
            _objectMapper = objectMapper;
            _preTrackingManager = preTrackingManager;
        }

        public async Task<Response<PreTrackingDto>> InsertAsync(CreatePreTrackingDto createPreTracking)
        {
            ResponseManager<PreTrackingDto> response = new();

            var previousPreTracking = await _preTrackingManager.GetByTransactionReference(createPreTracking.TransactionReference);
            if (previousPreTracking != null)
                return response.OnSuccess(_objectMapper.Map<PreTracking, PreTrackingDto>(previousPreTracking));

            try
            {
                PreTracking preTracking = _objectMapper.Map<CreatePreTrackingDto, PreTracking>(createPreTracking);

                var result = await _preTrackingManager.InsertAsync(preTracking, true);
                return response.OnSuccess(_objectMapper.Map<PreTracking, PreTrackingDto>(result));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<PreTrackingDto>(TrackingErrorCodes.GetErrorPreTrackingCreate(), ex);
            }
        }

        public async Task<Response<PreTrackingDto>> AddPreTrackingStep(CreatePreTrackingStepDto input, Guid preTrackingId)
        {
            ResponseManager<PreTrackingDto> response = new();
            try
            {
                input.SetPreTrackingId(preTrackingId);
                var preTrackingStepInput = _objectMapper.Map<CreatePreTrackingStepDto, PreTrackingStepInput>(input);
                var preTracking = await _preTrackingManager.AddPreTrackingStep(preTrackingStepInput);
                return response.OnSuccess(_objectMapper.Map<PreTracking, PreTrackingDto>(preTracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<PreTrackingDto>(TrackingErrorCodes.GetErrorPreTrackingAddStep(), ex);
            }
        }

        public async Task<Response<PreTrackingDto>> GetById(Guid preTrackingId)
        {
            ResponseManager<PreTrackingDto> response = new();
            try
            {
                var preTracking = await _preTrackingManager.GetById(preTrackingId);
                return response.OnSuccess(_objectMapper.Map<PreTracking, PreTrackingDto>(preTracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<PreTrackingDto>(TrackingErrorCodes.GetErrorGetPreTracking(TrackingConsts.ID), ex);
            }
        }

        public async Task<Response<PreTrackingDto>> GetByTransactionReference(string transactionReference)
        {
            ResponseManager<PreTrackingDto> response = new();
            try
            {
                var preTracking = await _preTrackingManager.GetByTransactionReference(transactionReference);
                return response.OnSuccess(_objectMapper.Map<PreTracking, PreTrackingDto>(preTracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<PreTrackingDto>(TrackingErrorCodes.GetErrorGetPreTracking(TrackingConsts.TRANSACTION_REFERENCE), ex);
            }
        }

        public async Task<Response<bool>> UpdateTracking(string transactionReference, Guid trackingId)
        {
            ResponseManager<bool> response = new();
            try
            {
                var preTracking = await _preTrackingManager.UpdateTracking(transactionReference, trackingId);
                return preTracking == null ? response.OnSuccess(true) : response.OnSuccess(false);
            }
            catch (PreTrackingNotFoundException ex)
            {
                return GetErrorResponse<bool>(TrackingErrorCodes.GetErrorNotFoundById(), ex);
            }
            catch (TrackingNotFoundException ex)
            {
                return GetErrorResponse<bool>(TrackingErrorCodes.GetErrorNotFoundById(), ex);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool>(TrackingErrorCodes.GetErrorGetPreTracking(TrackingConsts.TRANSACTION_REFERENCE), ex);
            }
        }
    }
}
