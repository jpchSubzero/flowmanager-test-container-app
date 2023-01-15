using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public interface ITrackingAppService : IApplicationService
    {
        Task<Response<TrackingDto>> GetAsync(Guid trackingId);
        Task<Response<TrackingDto>> AddFailureLog(CreateFailureLogDto createFailureLogDto, Guid trackingId);
        Task<Response<TrackingDto>> CreateAsync(CreateTrackingRequestDto createTrackingRequest, Guid flowId);
        Task<Response<TrackingDto>> AbandonAsync(Guid trackingId);
        Task<Response<TrackingDto>> TimedOutAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndAsync(Guid trackingId);
        Task<Response<TrackingDto>> StartQuotationAsync(Guid trackingId);
        Task<Response<object>> EndQuotationAsync(Guid trackingId, bool filtered = true);
        Task<Response<TrackingDto>> StartSubscriptionAsync(Guid trackingId);
        Task<Response<object>> EndSubscriptionAsync(Guid trackingId, bool filtered = true);
        Task<Response<TrackingDto>> StartSaleAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndSaleAsync(Guid trackingId);
        Task<Response<TrackingDto>> StartPaymentAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndPaymentAsync(Guid trackingId);
        Task<Response<TrackingDto>> StartContractAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndContractAsync(Guid trackingId);
        Task<Response<TrackingDto>> StartNotificationAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndNotificationAsync(Guid trackingId);
        Task<Response<TrackingDto>> StartInspectionAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndInspectionAsync(Guid trackingId);
        Task<Response<TrackingDto>> CloseSaleAsync(Guid trackingId);
        Task<Response<TrackingDto>> EndProcessAsync(Guid trackingId);
        Task<Response<TrackingDto>> SetExtraPropertiesAsync(Guid trackingId, ExtraPropertyDictionary extraProperties);
        Task<Response<TrackingDto>> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties);
        Task<Response<TrackingProductDetailDto>> GetFlowAndProductDetailByTrackingId(Guid trackingId);
        Task<Response<TrackingDto>> BackwardStepsAsync(Guid trackingId, int steps);
        Task<Response<TrackingDto>> ForewardStepsAsync(Guid trackingId, int steps);
        Task<Response<TrackingDto>> MoveToEspecificStepsAsync(Guid trackingId, int step);
        Task<Response<bool?>> IsClose(Guid trackingId);
        Task<Response<bool?>> IsEnded(Guid trackingId);
        Task<Response<bool?>> IsTimeout(Guid trackingId);
        Task<Response<bool?>> IsAbandoned(Guid trackingId);
        Task<Response<bool?>> IsInProgress(Guid trackingId);
        Task<Response<bool?>> IsInitialized(Guid trackingId);
        Task<Response<bool?>> HasError(Guid trackingId);
        Task<Response<string>> CurrentStep(Guid trackingId);
        Task<Response<TrackingDto>> AddProcessLog(CreateProcessLogDto createProcessLogDto, Guid trackingId);
        Task<Response<ICollection<TrackingDto>>> GetByIpAsync(string ip);
        Task<Response<TrackingDto>> AddSubStepLog(CreateSubStepLogDto createSubStepLogDto, Guid trackingId);
        Task<Response<ICollection<ProcessLogDto>>> GetProcessLogsByTrackingStepActionAndVersionAsync(Guid trackingId, Guid? stepId = null, string actionName = "", int version = 0);
        Task<Response<TrackingDto>> GetByChannelExtraPropertiesAsync(string channelCode, ExtraPropertyDictionary extraProperties);
        Task<Response<TrackingDto>> GetByWayChannelExtraPropertiesAsync(string wayCode, string channelCode, ExtraPropertyDictionary extraProperties);

    }
}
