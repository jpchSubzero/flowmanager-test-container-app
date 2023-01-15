using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.PreTrackings.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    public interface IPreTrackingAppService : IApplicationService
    {
        Task<Response<PreTrackingDto>> InsertAsync(CreatePreTrackingDto createPreTracking);
        Task<Response<PreTrackingDto>> AddPreTrackingStep(CreatePreTrackingStepDto input, Guid preTrackingId);
        Task<Response<PreTrackingDto>> GetById(Guid preTrackingId);
        Task<Response<PreTrackingDto>> GetByTransactionReference(string transactionReference);
        Task<Response<bool>> UpdateTracking(string transactionReference, Guid trackingId);
    }
}
