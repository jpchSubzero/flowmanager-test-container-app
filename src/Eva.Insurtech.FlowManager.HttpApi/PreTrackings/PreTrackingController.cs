using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    [RemoteService]
    [Route("api/pre-tracking")]
    public class PreTrackingController : FlowManagerController, IPreTrackingAppService
    {
        private readonly IPreTrackingAppService _preTrackingAppService;

        public PreTrackingController(IPreTrackingAppService preTrackingAppService)
        {
            _preTrackingAppService = preTrackingAppService;
        }

        [HttpPost]
        public async Task<Response<PreTrackingDto>> InsertAsync(CreatePreTrackingDto createPreTracking)
        {
            return await _preTrackingAppService.InsertAsync(createPreTracking);
        }

        [HttpPost]
        [Route("{preTrackingId}/pre-tracking-step")]
        public async Task<Response<PreTrackingDto>> AddPreTrackingStep(CreatePreTrackingStepDto input, Guid preTrackingId)
        {
            return await _preTrackingAppService.AddPreTrackingStep(input, preTrackingId);
        }

        [HttpGet]
        [Route("{preTrackingId}/by-id")]
        public async Task<Response<PreTrackingDto>> GetById(Guid preTrackingId)
        {
            return await _preTrackingAppService.GetById(preTrackingId);
        }

        [HttpGet]
        [Route("{transactionReference}/by-transaction-reference")]
        public async Task<Response<PreTrackingDto>> GetByTransactionReference(string transactionReference)
        {
            return await _preTrackingAppService.GetByTransactionReference(transactionReference);
        }

        [HttpPost]
        [Route("{transactionReference}/tracking/{trackingId}/update")]
        public async Task<Response<bool>> UpdateTracking(string transactionReference, Guid trackingId)
        {
            return await _preTrackingAppService.UpdateTracking(transactionReference, trackingId);
        }
    }
}
