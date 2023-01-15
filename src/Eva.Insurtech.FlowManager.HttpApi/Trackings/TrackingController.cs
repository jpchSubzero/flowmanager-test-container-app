using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;

namespace Eva.Insurtech.FlowManagers
{
    [RemoteService]
    [Route("api/tracking")]
    public class TrackingController : FlowManagerController, ITrackingAppService
    {
        private readonly ITrackingAppService _trackingAppService;

        public TrackingController(ITrackingAppService trackingAppService)
        {
            _trackingAppService = trackingAppService;
        }

        [HttpGet]
        [Route("{trackingId}")]
        public async Task<Response<TrackingDto>> GetAsync(Guid trackingId)
        {
            Check.NotNull(trackingId, nameof(trackingId));
            return await _trackingAppService.GetAsync(trackingId);
        }

        [HttpGet]
        [Route("{ip}/by-ip")]
        public async Task<Response<ICollection<TrackingDto>>> GetByIpAsync(string ip)
        {
            return await _trackingAppService.GetByIpAsync(ip);
        }

        [HttpPost]
        [Route("{trackingId}/failure-log")]
        public async Task<Response<TrackingDto>> AddFailureLog(CreateFailureLogDto createFailureLogDto, Guid trackingId)
        {
            Check.NotNull(createFailureLogDto, nameof(createFailureLogDto));
            return await _trackingAppService.AddFailureLog(createFailureLogDto, trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/process-log")]
        public async Task<Response<TrackingDto>> AddProcessLog(CreateProcessLogDto createProcessLogDto, Guid trackingId)
        {
            return await _trackingAppService.AddProcessLog(createProcessLogDto, trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/sub-step-log")]
        public async Task<Response<TrackingDto>> AddSubStepLog(CreateSubStepLogDto createSubStepLogDto, Guid trackingId)
        {
            return await _trackingAppService.AddSubStepLog(createSubStepLogDto, trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/process-log")]
        public async Task<Response<ICollection<ProcessLogDto>>> GetProcessLogsByTrackingStepActionAndVersionAsync(Guid trackingId, Guid? stepId = null, string actionName = "", int version = 0)
        {
            return await _trackingAppService.GetProcessLogsByTrackingStepActionAndVersionAsync(trackingId, stepId, actionName, version);
        }

        [HttpPost]
        [Route("{flowId}/create")]
        public async Task<Response<TrackingDto>> CreateAsync(CreateTrackingRequestDto createTrackingRequest, Guid flowId)
        {
            return await _trackingAppService.CreateAsync(createTrackingRequest, flowId);
        }

        [HttpPost]
        [Route("{trackingId}/abandon")]
        public async Task<Response<TrackingDto>> AbandonAsync(Guid trackingId)
        {
            return await _trackingAppService.AbandonAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/timed-out")]
        public async Task<Response<TrackingDto>> TimedOutAsync(Guid trackingId)
        {
            return await _trackingAppService.TimedOutAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end")]
        public async Task<Response<TrackingDto>> EndAsync(Guid trackingId)
        {
            return await _trackingAppService.EndAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/start-quotation")]
        public async Task<Response<TrackingDto>> StartQuotationAsync(Guid trackingId)
        {
            return await _trackingAppService.StartQuotationAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-quotation")]
        public async Task<Response<object>> EndQuotationAsync(Guid trackingId, bool filtered = true)
        {
            return await _trackingAppService.EndQuotationAsync(trackingId, filtered);
        }

        [HttpPost]
        [Route("{trackingId}/start-subscription")]
        public async Task<Response<TrackingDto>> StartSubscriptionAsync(Guid trackingId)
        {
            return await _trackingAppService.StartSubscriptionAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-subscription")]
        public async Task<Response<object>> EndSubscriptionAsync(Guid trackingId, bool filtered = true)
        {
            return await _trackingAppService.EndSubscriptionAsync(trackingId, filtered);
        }

        [HttpPost]
        [Route("{trackingId}/start-sale")]
        public async Task<Response<TrackingDto>> StartSaleAsync(Guid trackingId)
        {
            return await _trackingAppService.StartSaleAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-sale")]
        public async Task<Response<TrackingDto>> EndSaleAsync(Guid trackingId)
        {
            return await _trackingAppService.EndSaleAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/start-payment")]
        public async Task<Response<TrackingDto>> StartPaymentAsync(Guid trackingId)
        {
            return await _trackingAppService.StartPaymentAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-payment")]
        public async Task<Response<TrackingDto>> EndPaymentAsync(Guid trackingId)
        {
            return await _trackingAppService.EndPaymentAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/start-contract")]
        public async Task<Response<TrackingDto>> StartContractAsync(Guid trackingId)
        {
            return await _trackingAppService.StartContractAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-contract")]
        public async Task<Response<TrackingDto>> EndContractAsync(Guid trackingId)
        {
            return await _trackingAppService.EndContractAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/start-notification")]
        public async Task<Response<TrackingDto>> StartNotificationAsync(Guid trackingId)
        {
            return await _trackingAppService.StartNotificationAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-notification")]
        public async Task<Response<TrackingDto>> EndNotificationAsync(Guid trackingId)
        {
            return await _trackingAppService.EndNotificationAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/start-inspection")]
        public async Task<Response<TrackingDto>> StartInspectionAsync(Guid trackingId)
        {
            return await _trackingAppService.StartInspectionAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-inspection")]
        public async Task<Response<TrackingDto>> EndInspectionAsync(Guid trackingId)
        {
            return await _trackingAppService.EndInspectionAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/close-sale")]
        public async Task<Response<TrackingDto>> CloseSaleAsync(Guid trackingId)
        {
            return await _trackingAppService.CloseSaleAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/end-process")]
        public async Task<Response<TrackingDto>> EndProcessAsync(Guid trackingId)
        {
            return await _trackingAppService.EndProcessAsync(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/extra-properties")]
        public async Task<Response<TrackingDto>> SetExtraPropertiesAsync(Guid trackingId, ExtraPropertyDictionary extraProperties)
        {
            return await _trackingAppService.SetExtraPropertiesAsync(trackingId, extraProperties);
        }

        [HttpPost]
        [Route("by-extra-properties")]
        public async Task<Response<TrackingDto>> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties)
        {
            return await _trackingAppService.GetByExtraPropertiesAsync(extraProperties);
        }

        [HttpPost]
        [Route("channel/{channelCode}/by-extra-properties")]
        public async Task<Response<TrackingDto>> GetByChannelExtraPropertiesAsync([Required] string channelCode, ExtraPropertyDictionary extraProperties)
        {
            return await _trackingAppService.GetByChannelExtraPropertiesAsync(channelCode, extraProperties);
        }

        [HttpPost]
        [Route("channel/{channelCode}/way/{wayCode}/by-extra-properties")]
        public async Task<Response<TrackingDto>> GetByWayChannelExtraPropertiesAsync([Required] string wayCode, [Required] string channelCode, ExtraPropertyDictionary extraProperties)
        {
            return await _trackingAppService.GetByWayChannelExtraPropertiesAsync(wayCode, channelCode, extraProperties);
        }

        [HttpGet]
        [Route("{trackingId}/details")]
        public async Task<Response<TrackingProductDetailDto>> GetFlowAndProductDetailByTrackingId(Guid trackingId)
        {
            return await _trackingAppService.GetFlowAndProductDetailByTrackingId(trackingId);
        }

        [HttpPost]
        [Route("{trackingId}/backward/{steps}")]
        public async Task<Response<TrackingDto>> BackwardStepsAsync(Guid trackingId, int steps)
        {
            return await _trackingAppService.BackwardStepsAsync(trackingId, steps);
        }

        [HttpPost]
        [Route("{trackingId}/foreward/{steps}")]
        public async Task<Response<TrackingDto>> ForewardStepsAsync(Guid trackingId, int steps)
        {
            return await _trackingAppService.ForewardStepsAsync(trackingId, steps);
        }

        [HttpPost]
        [Route("{trackingId}/move-to/{step}")]
        public async Task<Response<TrackingDto>> MoveToEspecificStepsAsync(Guid trackingId, int step)
        {
            return await _trackingAppService.MoveToEspecificStepsAsync(trackingId, step);
        }

        [HttpGet]
        [Route("{trackingId}/is-close")]
        public async Task<Response<bool?>> IsClose(Guid trackingId)
        {
            return await _trackingAppService.IsClose(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/is-ended")]
        public async Task<Response<bool?>> IsEnded(Guid trackingId)
        {
            return await _trackingAppService.IsEnded(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/is-timeout")]
        public async Task<Response<bool?>> IsTimeout(Guid trackingId)
        {
            return await _trackingAppService.IsTimeout(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/is-abandoned")]
        public async Task<Response<bool?>> IsAbandoned(Guid trackingId)
        {
            return await _trackingAppService.IsAbandoned(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/is-inprogress")]
        public async Task<Response<bool?>> IsInProgress(Guid trackingId)
        {
            return await _trackingAppService.IsInProgress(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/is-initialized")]
        public async Task<Response<bool?>> IsInitialized(Guid trackingId)
        {
            return await _trackingAppService.IsInitialized(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/has-error")]
        public async Task<Response<bool?>> HasError(Guid trackingId)
        {
            return await _trackingAppService.HasError(trackingId);
        }

        [HttpGet]
        [Route("{trackingId}/current-step")]
        public async Task<Response<string>> CurrentStep(Guid trackingId)
        {
            return await _trackingAppService.CurrentStep(trackingId);
        }
    }
}


