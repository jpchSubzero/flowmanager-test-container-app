using Eva.Framework.Utility.Option.Contracts;
using Eva.Framework.Utility.Response;
using Eva.Framework.Utility.Response.Filters;
using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.BusCharge;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.Exceptions;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.IntegrationApiServices;
using Eva.Insurtech.FlowManagers.MessengerApiServices;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using Eva.Insurtech.TrackingManagers.Trackings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;

namespace Eva.Insurtech.Trackings
{
    public class TrackingAppService : FlowManagerAppService, ITrackingAppService
    {
        private readonly TrackingManager _trackingManager;
        private readonly CatalogManager _catalogManager;
        private readonly ProductManager _productManager;
        private readonly FlowManager _flowManager;
        private readonly IAppConfigurationManager _appConfigurationManager;
        private readonly IObjectMapper _objectMapper;
        private readonly IntegrationApiServiceManager _integrationApiServiceManager;
        private readonly MessengerApiServiceManager _messengerApiServiceManager;
        public TrackingAppService(
            TrackingManager trackingManager,
            CatalogManager catalogManager,
            ProductManager productManager,
            FlowManager flowManager,
            IObjectMapper objectMapper,
            IAppConfigurationManager appConfigurationManager,
            ILogger<TrackingAppService> logger,
            IntegrationApiServiceManager integrationApiServiceManager,
            MessengerApiServiceManager messengerApiServiceManager
        ) : base(logger)
        {
            _trackingManager = trackingManager;
            _catalogManager = catalogManager;
            _productManager = productManager;
            _flowManager = flowManager;
            _objectMapper = objectMapper;
            _appConfigurationManager = appConfigurationManager;
            _integrationApiServiceManager = integrationApiServiceManager;
            _messengerApiServiceManager = messengerApiServiceManager;
        }

        public async Task<Response<TrackingDto>> GetAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                TrackingDto tracking = _objectMapper.Map<Tracking, TrackingDto>(await _trackingManager.GetAsync(trackingId));
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                return response.OnSuccess(tracking);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingProductDetailDto>> GetFlowAndProductDetailByTrackingId(Guid trackingId)
        {
            ResponseManager<TrackingProductDetailDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                var flow = await _flowManager.GetAsync(tracking.FlowId);
                var product = await _productManager.GetByExternalIdAsync(flow.ProductId);

                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                GenericValidations.ValidateIfItemExists(product, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

                TrackingProductDetailDto trackingProductDetail = new()
                {
                    ChannelCode = flow.ChannelCode,
                    ChannelName = flow.Name,
                    ExternalCode = product.ExternalCode,
                    FlowCode = flow.Code,
                    FlowId = flow.Id,
                    ProductCode = product.Code,
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    TrackingId = tracking.Id,
                };

                return response.OnSuccess(trackingProductDetail);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingProductDetailDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> AddFailureLog(CreateFailureLogDto createFailureLogDto, Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                await _catalogManager.ValidateIfExistInCatalog(TrackingConsts.FLOW_STATES, createFailureLogDto.StateId);

                createFailureLogDto.SetTrackingId(trackingId);
                var failuteLogInput = _objectMapper.Map<CreateFailureLogDto, FailureLogInput>(createFailureLogDto);
                var tracking = await _trackingManager.AddFailureLogAsync(failuteLogInput);
                await ValidateIfTrackingIsClose(tracking);

                var flow = await _flowManager.GetAsync(tracking.FlowId);
                GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

                var catEmailCode = flow.Code + BusChargeConsts.ComplementEmail;
                var dataProduct = await _flowManager.GetByProductIdAsync(flow.ProductId);
                string description = dataProduct.FirstOrDefault()?.Description;

                var actualStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
                var actualOrder = actualStep.Order;

                var previousStateCode = await GetPreviousStateFromStep(flow, actualOrder);
                var previousStateCatalog = await _catalogManager.GetByCodeAsync(previousStateCode.ToUpper(), TrackingConsts.FLOW_STATES);

                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ERROR, TrackingConsts.FLOW_GENERAL_STATES);

                tracking.SetStateId(previousStateCatalog.CatalogId);
                tracking.SetGeneralStateId(generalState.CatalogId);
                tracking = await _trackingManager.UpdateAsync(tracking, true);

                tracking = await ValidateBusServiceFails(trackingId, failuteLogInput, tracking, catEmailCode, description);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> CreateAsync(CreateTrackingRequestDto createTrackingRequest, Guid flowId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var maxCreateByIp = _appConfigurationManager.GetVariableByTypeName<int>(TrackingConsts.Tracking, TrackingConsts.MaxCreateByIp);

                var trackingsByIp = await GetByIpAsync(createTrackingRequest.IpClient);
                if (maxCreateByIp > 0 && trackingsByIp.Result?.Count >= maxCreateByIp)
                {
                    throw new TrackingNotFoundException(TrackingErrorCodes.GetErrorTrackingExcededCreationByDay());
                }
                createTrackingRequest.SetFlowId(flowId);

                CreateTrackingDto input = await SetCatalogDto(createTrackingRequest.FlowId, TrackingConsts.START_TRACKING, TrackingConsts.TRACKING_CREATED, TrackingConsts.INITIALIZED);

                var flow = await _flowManager.GetAsync(createTrackingRequest.FlowId);
                GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));

                input.ChannelCode = flow.ChannelCode;
                input.WayCode = createTrackingRequest.WayCode;
                input.IpClient = createTrackingRequest.IpClient;

                await ValidateIfExistsCatalogs(input);
                await ValidateIfExistsChannel(input);

                var tracking = _objectMapper.Map<CreateTrackingDto, Tracking>(input);

                tracking = await _trackingManager.InsertAsync(tracking);
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                await ValidateIfTrackingIsClose(tracking);

                var step = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
                var state = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_ENDED, TrackingConsts.FLOW_STATES);
                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);

                tracking.SetStepId(step.CatalogId);
                tracking.SetStateId(state.CatalogId);
                tracking.SetGeneralStateId(generalState.CatalogId);
                tracking.UpdateEndDate();

                tracking = await _trackingManager.UpdateAsync(tracking);
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> AbandonAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                await ValidateIfTrackingIsClose(tracking);

                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ABANDONED, TrackingConsts.FLOW_GENERAL_STATES);

                tracking.SetGeneralStateId(generalState.CatalogId);
                tracking.UpdateAbandonDate();

                tracking = await _trackingManager.UpdateAsync(tracking);
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> TimedOutAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                await ValidateIfTrackingIsClose(tracking);

                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.TIMED_OUT, TrackingConsts.FLOW_GENERAL_STATES);

                tracking.SetGeneralStateId(generalState.CatalogId);
                tracking.UpdateAbandonDate();

                tracking = await _trackingManager.UpdateAsync(tracking);
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartQuotationAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_QUOTATION, TrackingConsts.QUOTATION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<object>> EndQuotationAsync(Guid trackingId, bool filtered = true)
        {
            ResponseManager<object> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.QUOTATION_DONE, TrackingConsts.QUOTATION_STARTED);

                var resultDto = _objectMapper.Map<Tracking, TrackingDto>(tracking);

                return await FilterResponse(filtered, response, tracking, resultDto, TrackingConsts.EndQuotationFilterSuffixCode);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<object>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartSubscriptionAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_SUBSCRIPTION, TrackingConsts.SUBSCRIPTION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<object>> EndSubscriptionAsync(Guid trackingId, bool filtered = true)
        {
            ResponseManager<object> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.SUBSCRIPTION_DONE, TrackingConsts.SUBSCRIPTION_STARTED);

                var resultDto = _objectMapper.Map<Tracking, TrackingDto>(tracking);

                return await FilterResponse(filtered, response, tracking, resultDto, TrackingConsts.EndSubscriptionFilterSuffixCode);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<object>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartSaleAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_SALE, TrackingConsts.SALE_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndSaleAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.SALE_REGISTERED, TrackingConsts.SALE_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartPaymentAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_PAYMENT, TrackingConsts.PAYMENT_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndPaymentAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.PAYMENT_DONE, TrackingConsts.PAYMENT_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartContractAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_CONTRACT, TrackingConsts.CONTRACT_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndContractAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.CONTRACT_DONE, TrackingConsts.CONTRACT_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartNotificationAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_NOTIFICATION, TrackingConsts.NOTIFICATION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndNotificationAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.NOTIFICATION_DONE, TrackingConsts.NOTIFICATION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> StartInspectionAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetStartContext(trackingId, TrackingConsts.START_INSPECTION, TrackingConsts.INSPECTION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndInspectionAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.INSPECTION_DONE, TrackingConsts.INSPECTION_STARTED);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> CloseSaleAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await SetEndContext(trackingId, TrackingConsts.SALE_DONE, string.Empty);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> EndProcessAsync(Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                await ValidateIfTrackingIsClose(tracking);

                var lastStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
                var state = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_ENDED, TrackingConsts.FLOW_STATES);
                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);
                var flow = await _flowManager.GetAsync(tracking.FlowId);

                if (!tracking.StepId.Equals(lastStep.CatalogId))
                {
                    var step = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
                    var nextStep = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(step.Order + 1));
                    var nextStepCatalog = await _catalogManager.GetByExternalIdAsync(nextStep.StepId);
                    var stateCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StateId);
                    var stepCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StepId);

                    if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_CREATED_SUFFIX))
                    {
                        throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(nextStepCatalog), TrackingConsts.END_LABEL, TrackingConsts.START_LABEL));
                    }
                    if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_STARTED_SUFFIX))
                    {
                        throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromState(stateCatalog), TrackingConsts.END_LABEL, TrackingConsts.END_LABEL));
                    }
                    if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_DONE_SUFFIX))
                    {
                        throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.END_LABEL, stepCatalog.Code.Equals(TrackingConsts.CLOSE_SALE) ? TrackingConsts.CLOSE_LABEL : TrackingConsts.START_LABEL));
                    }
                    if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_REGISTERED_SUFFIX))
                    {
                        throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.END_LABEL, TrackingConsts.START_LABEL));
                    }
                }


                tracking.SetStateId(state.CatalogId);
                tracking.SetGeneralStateId(generalState.CatalogId);
                tracking.UpdateEndDate();

                await _trackingManager.UpdateAsync(tracking);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> SetExtraPropertiesAsync(Guid trackingId, ExtraPropertyDictionary extraProperties)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

                tracking = await _trackingManager.SetExtraPropertiesAsync(trackingId, extraProperties);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetByExtraPropertiesAsync(extraProperties);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> BackwardStepsAsync(Guid trackingId, int steps)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                Tracking tracking = await ValidateTracking(trackingId);
                var flow = await _flowManager.GetAsync(tracking.FlowId);
                var step = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
                var order = step?.Order;
                var newOrder = order - steps;
                if (order != null && newOrder > 1)
                {
                    var newStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_TRACKING, TrackingConsts.FLOW_STEPS);
                    var requiredState = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_CREATED, TrackingConsts.FLOW_STATES);
                    var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.INITIALIZED, TrackingConsts.FLOW_GENERAL_STATES);

                    if ((newOrder - 1) > 1)
                    {
                        var previousState = await GetPreviousStateFromStep(flow, newOrder);
                        var newStepId = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(newOrder))?.StepId;
                        newStep = await _catalogManager.GetByExternalIdAsync(newStepId.Value);
                        requiredState = await _catalogManager.GetByCodeAsync(previousState.ToUpper(), TrackingConsts.FLOW_STATES);
                        generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);
                    }
                    tracking = await UpdateTrackingStateStepGeneralState(tracking, newStep, requiredState, generalState);
                }
                else
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorNotEnoughToBackward());
                }

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> ForewardStepsAsync(Guid trackingId, int steps)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                Tracking tracking = await ValidateTracking(trackingId);
                var flow = await _flowManager.GetAsync(tracking.FlowId);
                var maxStep = flow.FlowSteps.Max(x => x.Order);
                var step = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
                var order = step?.Order;
                var newOrder = order + steps;

                if (order != null && newOrder < maxStep)
                {
                    var newStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
                    var requiredState = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_ENDED, TrackingConsts.FLOW_STATES);
                    var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);

                    if (newOrder < maxStep)
                    {
                        var previousState = await GetPreviousStateFromStep(flow, newOrder);
                        var newStepId = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(newOrder))?.StepId;
                        newStep = await _catalogManager.GetByExternalIdAsync(newStepId.Value);
                        requiredState = await _catalogManager.GetByCodeAsync(previousState.ToUpper(), TrackingConsts.FLOW_STATES);
                        generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);
                    }
                    tracking = await UpdateTrackingStateStepGeneralState(tracking, newStep, requiredState, generalState);
                }
                else
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorNotEnoughToForeward());
                }


                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> MoveToEspecificStepsAsync(Guid trackingId, int step)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                Tracking tracking = await ValidateTracking(trackingId);
                var flow = await _flowManager.GetAsync(tracking.FlowId);
                var maxStep = flow.FlowSteps.Max(x => x.Order);

                if (step <= 1 || step >= maxStep)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorStepOutOfRange(2, maxStep - 1));
                }

                var stepToGo = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(step));

                var previousState = await GetPreviousStateFromStep(flow, step);
                var newStep = await _catalogManager.GetByExternalIdAsync(stepToGo.StepId);
                var requiredState = await _catalogManager.GetByCodeAsync(previousState.ToUpper(), TrackingConsts.FLOW_STATES);
                var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);
                tracking = await UpdateTrackingStateStepGeneralState(tracking, newStep, requiredState, generalState);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsClose(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var ended = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);
                var timedOut = await _catalogManager.GetByCodeAsync(TrackingConsts.TIMED_OUT, TrackingConsts.FLOW_GENERAL_STATES);
                var abandoned = await _catalogManager.GetByCodeAsync(TrackingConsts.ABANDONED, TrackingConsts.FLOW_GENERAL_STATES);
                List<Guid> closedStates = new() { ended.CatalogId, timedOut.CatalogId, abandoned.CatalogId };

                return response.OnSuccess(closedStates.Contains(tracking.GeneralStateId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsEnded(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var ended = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(ended.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsTimeout(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var timedOut = await _catalogManager.GetByCodeAsync(TrackingConsts.TIMED_OUT, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(timedOut.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsAbandoned(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var abandoned = await _catalogManager.GetByCodeAsync(TrackingConsts.ABANDONED, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(abandoned.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsInProgress(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var inProgress = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(inProgress.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> IsInitialized(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var initialized = await _catalogManager.GetByCodeAsync(TrackingConsts.INITIALIZED, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(initialized.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<bool?>> HasError(Guid trackingId)
        {
            ResponseManager<bool?> response = new();
            try
            {
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var error = await _catalogManager.GetByCodeAsync(TrackingConsts.ERROR, TrackingConsts.FLOW_GENERAL_STATES);

                return response.OnSuccess(tracking.GeneralStateId.Equals(error.CatalogId));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<bool?>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<string>> CurrentStep(Guid trackingId)
        {
            // Pendiente: Pruebas unitarias.
            ResponseManager<string> response = new();
            try
            {
                var currentStep = "UNDEFINED";
                Tracking tracking = await _trackingManager.GetAsync(trackingId);
                if (tracking != null)
                {
                    var step = await _catalogManager.GetByExternalIdAsync(tracking.StateId);
                    if (step != null)
                    {
                        currentStep = step.Code;
                    }
                }
                return response.OnSuccess(currentStep);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<string>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> AddProcessLog(CreateProcessLogDto createProcessLogDto, Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                await ValidateIfTrackingIsClose(tracking);

                createProcessLogDto.SetTrackingId(trackingId);
                createProcessLogDto.SetStepId(tracking.StepId);

                var processLogInput = _objectMapper.Map<CreateProcessLogDto, ProcessLogInput>(createProcessLogDto);
                tracking = await _trackingManager.AddProcessLogAsync(processLogInput);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<ICollection<ProcessLogDto>>> GetProcessLogsByTrackingStepActionAndVersionAsync(Guid trackingId, Guid? stepId = null, string actionName = "", int version = 0)
        {
            actionName = actionName.IsNullOrEmpty() ? "" : actionName;
            ResponseManager<ICollection<ProcessLogDto>> response = new();
            try
            {
                TrackingDto tracking = _objectMapper.Map<Tracking, TrackingDto>(await _trackingManager.GetAsync(trackingId));
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                var processLogs = stepId != null ?
                    tracking.ProcessLogs.Where(y => y.StepId.Equals(stepId) && y.Action.ToLower().Contains(actionName.ToLower())).OrderBy(x => x.Version) :
                    tracking.ProcessLogs.Where(y => y.Action.ToLower().Contains(actionName.ToLower())).OrderBy(x => x.Version);
                if (version == 0)
                    return response.OnSuccess(processLogs.OrderByDescending(x => x.Version).ToList());
                return response.OnSuccess(processLogs.Where(x => x.Version == version).ToList());
            }
            catch (Exception ex)
            {
                return GetErrorResponse<ICollection<ProcessLogDto>>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<ICollection<TrackingDto>>> GetByIpAsync(string ip)
        {
            ResponseManager<ICollection<TrackingDto>> response = new();
            try
            {
                var trackings = _objectMapper.Map<ICollection<Tracking>, ICollection<TrackingDto>>(await _trackingManager.GetByIpClientAsync(ip));
                if (trackings == null || !trackings.Any())
                {
                    throw new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundByIp());
                }
                return response.OnSuccess(trackings);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<ICollection<TrackingDto>>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> AddSubStepLog(CreateSubStepLogDto createSubStepLogDto, Guid trackingId)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetAsync(trackingId);
                await ValidateIfTrackingIsClose(tracking);

                createSubStepLogDto.SetTrackingId(trackingId);
                createSubStepLogDto.SetStepId(tracking.StepId);

                var subStepLogInput = _objectMapper.Map<CreateSubStepLogDto, SubStepLogInput>(createSubStepLogDto);
                tracking = await _trackingManager.AddSubStepLogAsync(subStepLogInput);

                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> GetByChannelExtraPropertiesAsync(string channelCode, ExtraPropertyDictionary extraProperties)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetByChannelExtraPropertiesAsync(channelCode, extraProperties);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<TrackingDto>> GetByWayChannelExtraPropertiesAsync(string wayCode, string channelCode, ExtraPropertyDictionary extraProperties)
        {
            ResponseManager<TrackingDto> response = new();
            try
            {
                var tracking = await _trackingManager.GetByWayChannelExtraPropertiesAsync(wayCode, channelCode, extraProperties);
                GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
                return response.OnSuccess(_objectMapper.Map<Tracking, TrackingDto>(tracking));
            }
            catch (Exception ex)
            {
                return GetErrorResponse<TrackingDto>(TrackingErrorCodes.GetErrorGeneral(), ex);
            }
        }




        #region Private Methods


        private async Task<Tracking> SetStartContext(Guid trackingId, string stepCode, string stateCode)
        {
            Tracking tracking = await ValidateTracking(trackingId);

            var step = await _catalogManager.GetByCodeAsync(stepCode, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(stateCode, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);
            var flow = await _flowManager.GetAsync(tracking.FlowId);

            await ValidateStateForStartContext(tracking, step, state, flow);

            await ValidateTrackingTimeOut(tracking, step, flow);

            return await UpdateTrackingStateStepGeneralState(tracking, step, state, generalState);
        }

        private async Task<Tracking> SetEndContext(Guid trackingId, string stateCode, string requiredStateCode)
        {
            Tracking tracking = await ValidateTracking(trackingId);

            var flow = await _flowManager.GetAsync(tracking.FlowId);
            var state = await _catalogManager.GetByCodeAsync(stateCode, TrackingConsts.FLOW_STATES);

            await ValidateStateForEndContext(requiredStateCode, tracking, flow, state);

            if (flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId)) == null)
            {
                await _catalogManager.GetByCodeAsync(TrackingConsts.START_TRACKING, TrackingConsts.FLOW_STEPS);
            }

            var flowStepOrder = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId))?.Order;
            var nextFlowStep = flow.FlowSteps.FirstOrDefault(x => x.Order == flowStepOrder + 1);

            var step = await _catalogManager.GetByExternalIdAsync(nextFlowStep.StepId);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            await ValidateTrackingTimeOut(tracking, step, flow);

            return await UpdateTrackingStateStepGeneralState(tracking, step, state, generalState);
        }

        private async Task<Tracking> ValidateTracking(Guid trackingId)
        {
            var tracking = await _trackingManager.GetAsync(trackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            await ValidateIfTrackingIsClose(tracking);
            return tracking;
        }

        private async Task ValidateStateForStartContext(Tracking tracking, Catalog step, Catalog state, Flow flow)
        {
            var actualFlowStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
            var newFlowStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(step.CatalogId));

            var actualFlowStepOrder = actualFlowStep?.Order;
            var newFlowStepOrder = newFlowStep?.Order;

            actualFlowStepOrder = actualFlowStepOrder == TrackingConsts.FIRST_STEP ? actualFlowStepOrder + 1 : actualFlowStepOrder;

            var expectedStep = await _catalogManager.GetByExternalIdAsync(flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(actualFlowStepOrder)).StepId);
            var actualStep = actualFlowStep != null ? await _catalogManager.GetByExternalIdAsync(actualFlowStep.StepId) : null;

            if (newFlowStep == null || newFlowStepOrder != actualFlowStepOrder || state.CatalogId == tracking.StateId)
            {
                if (newFlowStep == null)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorStepNotFoundOnFlow(GetContextNameForErrorFromStep(step), TrackingConsts.START_LABEL, flow.Name));
                }
                if (actualFlowStep?.Order == TrackingConsts.FIRST_STEP)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromStep(step), GetContextNameForErrorFromStep(expectedStep), TrackingConsts.START_LABEL, TrackingConsts.START_LABEL));
                }
                var stateCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StateId);

                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_STARTED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromStep(step), GetContextNameForErrorFromStep(expectedStep), TrackingConsts.START_LABEL, TrackingConsts.END_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_DONE_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromStep(step), GetContextNameForErrorFromStep(actualStep), TrackingConsts.START_LABEL, TrackingConsts.START_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_REGISTERED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromStep(step), GetContextNameForErrorFromStep(actualStep), TrackingConsts.START_LABEL, TrackingConsts.START_LABEL));
                }
            }
        }

        private async Task ValidateStateForEndContext(string requiredStateCode, Tracking tracking, Flow flow, Catalog state)
        {
            var step = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
            var nextStep = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(step.Order + 1));
            var closeSale = await _catalogManager.GetByCodeAsync(TrackingConsts.CLOSE_SALE, TrackingConsts.FLOW_STEPS);
            var closeSaleStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(closeSale.CatalogId));
            var trackingStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
            var nextStepCatalog = nextStep == null ? null : await _catalogManager.GetByExternalIdAsync(nextStep.StepId);

            if (requiredStateCode.IsNullOrEmpty())
            {
                if (closeSaleStep.Equals(trackingStep))
                {
                    return;
                }
                var stateCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StateId);
                await ProcessExceptionOnCloseSale(tracking, flow, state, nextStepCatalog, stateCatalog);
            }
            else
            {
                var expectedState = await _catalogManager.GetByCodeAsync(requiredStateCode, TrackingConsts.FLOW_STATES);
                if (tracking.StateId.Equals(expectedState.CatalogId))
                {
                    return;
                }
                await ProcessExceptionNonCloseSale(tracking, flow, state, nextStepCatalog);
            }

            async Task ProcessExceptionOnCloseSale(Tracking tracking, Flow flow, Catalog state, Catalog nextStepCatalog, Catalog stateCatalog)
            {
                var actualFlowStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));
                var stepCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StepId);
                if (actualFlowStep?.Order == TrackingConsts.FIRST_STEP)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(nextStepCatalog), TrackingConsts.CLOSE_LABEL, TrackingConsts.START_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_STARTED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromState(stateCatalog), TrackingConsts.CLOSE_LABEL, TrackingConsts.END_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_DONE_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.CLOSE_LABEL, TrackingConsts.START_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_REGISTERED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.CLOSE_LABEL, TrackingConsts.START_LABEL));
                }
            }

            async Task ProcessExceptionNonCloseSale(Tracking tracking, Flow flow, Catalog state, Catalog nextStepCatalog)
            {
                var actualFlowStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(tracking.StepId));

                var stepCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StepId);
                var stateCatalog = await _catalogManager.GetByExternalIdAsync(tracking.StateId);
                var endState = await _catalogManager.GetByCodeAsync(GetStepCodeFromState(state), TrackingConsts.FLOW_STATES);

                var newFlowStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(endState.CatalogId));

                if (newFlowStep == null)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorStepNotFoundOnFlow(GetContextNameForErrorFromStep(endState), TrackingConsts.END_LABEL, flow.Name));
                }
                if (actualFlowStep?.Order == TrackingConsts.FIRST_STEP)
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(nextStepCatalog), TrackingConsts.END_LABEL, TrackingConsts.START_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_STARTED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromState(stateCatalog), TrackingConsts.END_LABEL, TrackingConsts.END_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_DONE_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectEndStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.END_LABEL, TrackingConsts.START_LABEL));
                }
                if (stateCatalog.Code.EndsWith(TrackingConsts.TRACKING_REGISTERED_SUFFIX))
                {
                    throw new TrackingStepException(TrackingErrorCodes.GetErrorIncorrectStartStep(GetContextNameForErrorFromState(state), GetContextNameForErrorFromStep(stepCatalog), TrackingConsts.END_LABEL, TrackingConsts.START_LABEL));
                }
            }
        }

        private async Task ValidateIfTrackingIsClose(Tracking tracking)
        {
            if (tracking == null)
                throw new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById());

            var ended = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);
            var timedOut = await _catalogManager.GetByCodeAsync(TrackingConsts.TIMED_OUT, TrackingConsts.FLOW_GENERAL_STATES);
            var abandoned = await _catalogManager.GetByCodeAsync(TrackingConsts.ABANDONED, TrackingConsts.FLOW_GENERAL_STATES);
            List<Guid> closedStates = new() { ended.CatalogId, timedOut.CatalogId, abandoned.CatalogId };

            if (closedStates.Contains(tracking.GeneralStateId))
            {
                var trackingGeneralState = await _catalogManager.GetByExternalIdAsync(tracking.GeneralStateId);
                throw new TrackingStepException(TrackingErrorCodes.GetErrorLastStep(trackingGeneralState.Name.ToLower()));
            }
        }

        private async Task ValidateIfExistsCatalogs(CreateTrackingDto input)
        {
            await _catalogManager.ValidateIfExistInCatalog(TrackingConsts.FLOW_STATES, input.StateId);
            await _catalogManager.ValidateIfExistInCatalog(TrackingConsts.FLOW_GENERAL_STATES, input.GeneralStateId);
            await _catalogManager.ValidateIfExistInCatalog(TrackingConsts.FLOW_STEPS, input.StepId);
        }

        private async Task ValidateIfExistsChannel(CreateTrackingDto input)
        {
            await _productManager.ValidateIfChannelExistInProduct(input.ChannelCode);
        }

        private async Task<Tracking> UpdateTrackingStateStepGeneralState(Tracking tracking, Catalog step, Catalog state, Catalog generalState)
        {
            tracking.SetStepId(step.CatalogId);
            tracking.SetStateId(state.CatalogId);
            tracking.SetGeneralStateId(generalState.CatalogId);

            return await _trackingManager.UpdateAsync(tracking);
        }

        private async Task<CreateTrackingDto> SetCatalogDto(Guid flowId, string stepCode, string stateCode, string generalStateCode)
        {
            var step = await _catalogManager.GetByCodeAsync(stepCode, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(stateCode, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(generalStateCode, TrackingConsts.FLOW_GENERAL_STATES);

            var input = new CreateTrackingDto()
            {
                FlowId = flowId,
                StepId = step.CatalogId,
                StateId = state.CatalogId,
                GeneralStateId = generalState.CatalogId
            };
            return input;
        }

        private async Task<string> GetPreviousStateFromStep(Flow flow, int? newOrder)
        {
            if (newOrder == TrackingConsts.FIRST_FLOW_STEP || newOrder == TrackingConsts.FIRST_STEP)
            {
                return TrackingConsts.TRACKING_CREATED;
            }
            var previousStepOnFlow = flow.FlowSteps.FirstOrDefault(x => x.Order.Equals(newOrder - 1));
            var previousStepId = previousStepOnFlow == null ? Guid.NewGuid() : previousStepOnFlow.StepId;
            var previousStep = await _catalogManager.GetByExternalIdAsync(previousStepId);
            var previousStepContext = GetContextNameFromStep(previousStep);
            var previousStateCode = previousStep.Code.Equals(TrackingConsts.START_SALE) ? $"{previousStepContext}{TrackingConsts.TRACKING_REGISTERED_SUFFIX}" : $"{previousStepContext}{TrackingConsts.TRACKING_DONE_SUFFIX}";
            return previousStateCode;
        }

        private static string GetContextNameForErrorFromStep(Catalog nextStepCatalog)
        {
            return nextStepCatalog.Name.ToLower().Split(" ").LastOrDefault();
        }

        private static string GetContextNameForErrorFromState(Catalog state)
        {
            return state.Name.ToLower().Split(" ").FirstOrDefault();
        }

        private static string GetContextNameFromStep(Catalog step)
        {
            return step.Code.ToLower().Split("_").LastOrDefault();
        }

        private static string GetContextNameFromState(Catalog state)
        {
            return state.Code.ToLower().Split("_").FirstOrDefault();
        }

        private static string GetStepCodeFromState(Catalog state)
        {
            return $"{TrackingConsts.TRACKING_START_PREFIX}{GetContextNameFromState(state)}".ToUpper();
        }

        private async Task<Response<object>> FilterResponse(bool filtered, ResponseManager<object> response, Tracking tracking, TrackingDto resultDto, string filterSuffix)
        {
            if (!filtered)
            {
                return response.OnSuccess(resultDto);
            }

            var flow = await _flowManager.GetAsync(tracking.FlowId);

            try
            {
                var filterCatalog = await _catalogManager.GetByCodeAndItemCode(TrackingConsts.RESPONSE_FILTERS, $"{flow.Code}{filterSuffix}");

                if (filterCatalog == null || filterCatalog.AssociatedValue.IsNullOrEmpty())
                    return response.OnSuccess(resultDto);

                var filter = JsonConvert.DeserializeObject<List<FilteredField>>(filterCatalog.AssociatedValue);

                var result = CustomAnswerFields.ExtractCustomFields(filter, resultDto);

                if (CustomAnswerFields.IsDictionary(result))
                {
                    var resultFiltered = (IDictionary<string, object>)result;
                    return response.OnSuccess(resultFiltered);
                }
                return response.OnSuccess(resultDto);
            }
            catch (Exception)
            {
                return response.OnSuccess(resultDto);
            }
        }

        private async Task ValidateTrackingTimeOut(Tracking tracking, Catalog step, Flow flow)
        {
            var newStep = flow.FlowSteps.FirstOrDefault(x => x.StepId.Equals(step.CatalogId));
            if (newStep.MaxLifeTime > 0 && DateTime.Compare(tracking.ChangeState.AddMinutes(newStep.MaxLifeTime), DateTime.Now) <= 0)
            {
                await TimedOutAsync(tracking.Id);
                throw new TrackingTimeoutException(TrackingErrorCodes.GetErrorTimeoutTracking());
            }
            if (flow.MaxLifeTime > 0 && DateTime.Compare(tracking.Start.AddMinutes(flow.MaxLifeTime), DateTime.Now) <= 0)
            {
                await TimedOutAsync(tracking.Id);
                throw new TrackingTimeoutException(TrackingErrorCodes.GetErrorTimeoutTracking());
            }
        }

        private async Task<Tracking> ValidateBusServiceFails(Guid trackingId, FailureLogInput failuteLogInput, Tracking tracking, string catEmailCode, string descriptionInput)
        {
            int numRetries = await _integrationApiServiceManager.GetBusServicesRetries();
            var regBusCharge = await _integrationApiServiceManager.GetBusServicesByTracking(trackingId);

            if (regBusCharge != null && regBusCharge.Attempts > numRetries)
            {
                var emailadd = await _catalogManager.GetByCodeAsync(catEmailCode, catEmailCode);
                var errorNotification = emailadd.ExtraProperties["errorNotification"];
                var mailData = JsonConvert.DeserializeObject<MessengerEvaDto>(errorNotification.ToString());
                var description = mailData.TextData.FirstOrDefault()?.Text;
                mailData.TextData.FirstOrDefault().Text = $"{BusChargeConsts.MessengerTextBus}{descriptionInput} {description}";
                var resultMail = await _messengerApiServiceManager.SendReportAsync(mailData);
                if (!resultMail)
                {
                    failuteLogInput.Error += BusChargeConsts.ErrorEmail;
                    tracking = await _trackingManager.AddFailureLogAsync(failuteLogInput);
                }
            }

            return tracking;
        }


        #endregion
    }
}