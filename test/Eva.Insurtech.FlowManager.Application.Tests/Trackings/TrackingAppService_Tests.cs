using Eva.Framework.Utility.Option.Contracts;
using Eva.Insurtech.AppApiServices;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.IntegrationApiServices;
using Eva.Insurtech.FlowManagers.MessengerApiServices;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs;
using Eva.Insurtech.Flows.MockHttpMessageHandler;
using Eva.Insurtech.TrackingManagers.Trackings;
using Eva.Insurtech.Trackings;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using Xunit;

namespace Eva.Insurtech.Product.Trackings
{
    [ExcludeFromCodeCoverage]
    public class TrackingAppService_Tests : FlowManagerApplicationTestBase
    {
        private readonly ITrackingAppService _trackingAppService;
        private readonly IFlowAppService _flowAppService;
        private readonly CatalogManager _catalogManager;
        private readonly ProductManager _productManager;
        private readonly Eva.Insurtech.FlowManagers.Flows.FlowManager _flowManager;
        private readonly IFlowRepository _flowRepository;
        private readonly ITrackingRepository _trackingtRepository;
        private readonly TrackingManager _trackingManager;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IAppConfigurationManager _appConfigurationManager;
        private readonly IObjectMapper _objectMapper;
        private readonly ILogger<TrackingAppService> _logger;
        private readonly IntegrationApiServiceManager _integrationApiServiceManager;
        private readonly MessengerApiServiceManager _messengerApiServiceManager;

        public TrackingAppService_Tests()
        {
            _trackingAppService = GetRequiredService<ITrackingAppService>();
            _flowAppService = GetRequiredService<IFlowAppService>();
            _catalogManager = GetRequiredService<CatalogManager>();
            _productManager = GetRequiredService<ProductManager>();
            _flowManager = GetRequiredService<Eva.Insurtech.FlowManagers.Flows.FlowManager>();
            _flowRepository = GetRequiredService<IFlowRepository>();
            _trackingtRepository = GetRequiredService<ITrackingRepository>();
            _trackingManager = GetRequiredService<TrackingManager>();
            _catalogRepository = GetRequiredService<ICatalogRepository>();
            _appConfigurationManager = GetRequiredService<IAppConfigurationManager>();
            _objectMapper = GetRequiredService<IObjectMapper>();
            _logger = GetRequiredService<ILogger<TrackingAppService>>();

            var _loggerA = GetRequiredService<ILogger<AppApiServiceManager>>();
            var _loggerI = GetRequiredService<ILogger<IntegrationApiServiceManager>>();
            string integrationResponse = "{'success':true,'result':{'trackingId':'710392e7-f3df-e351-082f-3a05b481620f','message':'','type':'SALE','isActive':true,'attempts':3,'id':'00000000-0000-0000-0000-000000000000'},'error':null,'targetUrl':'','unAuthorizedRequest':true}";
            var handlerConfigurationMock = FakeHttpClient.GetMockHttpMessageHandler(integrationResponse);
            var httpClientConfiguration = new HttpClient(handlerConfigurationMock.Object);
            var mockHttpClientConfiguration = new AppApiServiceManager(httpClientConfiguration, _loggerA);
            _integrationApiServiceManager = new IntegrationApiServiceManager(_appConfigurationManager, mockHttpClientConfiguration, _loggerI);
            _messengerApiServiceManager = GetRequiredService<MessengerApiServiceManager>();
        }

        [Fact]
        public async Task GetTracking_ById_ReturnResponseSuccess()
        {
            var trackings = await _trackingtRepository.GetListAsync();
            var tracking = await _trackingAppService.GetAsync(trackings.LastOrDefault().Id);

            Assert.Equal(tracking.Result.Id, trackings.LastOrDefault().Id);
            Assert.Equal(tracking.Result.FlowId, trackings.LastOrDefault().FlowId);
            Assert.Equal(tracking.Result.StepId, trackings.LastOrDefault().StepId);
            Assert.Equal(tracking.Result.StateId, trackings.LastOrDefault().StateId);
            Assert.Equal(tracking.Result.GeneralStateId, trackings.LastOrDefault().GeneralStateId);
        }

        [Fact]
        public async Task GetTracking_ByNonExistentId_ReturnResponseSuccess()
        {
            var result = await _trackingAppService.GetAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task CreateTracking_WithData_ReturnResponseSuccess()
        {
            await LoadCatalogs();

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_TRACKING, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_CREATED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.INITIALIZED, TrackingConsts.FLOW_GENERAL_STATES);

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            var flows = await _flowRepository.GetAllAsync();
            var flow = flows.FirstOrDefault();

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var result = await trackingAppService.CreateAsync(newTracking, flow.Id);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, flow.Id);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
            Assert.Equal(newTracking.WayCode, result.Result.WayCode);
            Assert.Equal(newTracking.IpClient, result.Result.IpClient);
        }

        [Fact]
        public async Task CreateTracking_WithNonExistsFlow_ReturnNotFoundById()
        {
            await LoadCatalogs();

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var result = await trackingAppService.CreateAsync(newTracking, Guid.NewGuid());

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task AbandonTracking_WithData_ReturnResponseSuccess()
        {
            await LoadCatalogs();

            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ABANDONED, TrackingConsts.FLOW_GENERAL_STATES);

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            var trackings = await _trackingtRepository.GetAllAsync();
            var tracking = trackings.FirstOrDefault();

            var result = await trackingAppService.AbandonAsync(tracking.Id);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.FlowId);
            Assert.Equal(result.Result.StepId, tracking.StepId);
            Assert.Equal(result.Result.StateId, tracking.StateId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task AbandonTracking_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.AbandonAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task TimedOutTracking_WithData_ReturnResponseSuccess()
        {
            await LoadCatalogs();

            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.TIMED_OUT, TrackingConsts.FLOW_GENERAL_STATES);

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            var trackings = await _trackingtRepository.GetAllAsync();
            var tracking = trackings.FirstOrDefault();

            var result = await trackingAppService.TimedOutAsync(tracking.Id);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.FlowId);
            Assert.Equal(result.Result.StepId, tracking.StepId);
            Assert.Equal(result.Result.StateId, tracking.StateId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task TimedOutTracking_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.TimedOutAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndTracking_WithData_ReturnResponseSuccess()
        {
            await LoadCatalogs();

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_ENDED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            var trackings = await _trackingtRepository.GetAllAsync();
            var tracking = trackings.FirstOrDefault();

            var result = await trackingAppService.EndAsync(tracking.Id);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndTracking_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StartQuotation_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW001");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_QUOTATION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.QUOTATION_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_CREATED, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartQuotation_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartQuotationAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndQuotation_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SUBSCRIPTION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.QUOTATION_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_QUOTATION, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.QUOTATION_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);

            var result = (TrackingDto)result1.Result;

            Assert.NotNull(result1.Result);
            Assert.True(result1.Success);
            Assert.Equal(result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.StepId, step.CatalogId);
            Assert.Equal(result.StateId, state.CatalogId);
            Assert.Equal(result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndQuotation_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndQuotationAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StartSubscription_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SUBSCRIPTION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SUBSCRIPTION_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_CREATED, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartSubscription_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartSubscriptionAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndSubscription_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_PAYMENT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SUBSCRIPTION_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SUBSCRIPTION, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SUBSCRIPTION_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);

            var result = (TrackingDto)result1.Result;

            Assert.NotNull(result1.Result);
            Assert.True(result1.Success);
            Assert.Equal(result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.StepId, step.CatalogId);
            Assert.Equal(result.StateId, state.CatalogId);
            Assert.Equal(result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndSubscription_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndSubscriptionAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StartSale_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.PAYMENT_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartSaleAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartSale_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartSaleAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndSale_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_REGISTERED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndSaleAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndSale_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndSaleAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StarPayment_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW001");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_PAYMENT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.PAYMENT_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartPayment_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartPaymentAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndPayment_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.PAYMENT_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_PAYMENT, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.PAYMENT_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndPayment_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndPaymentAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StarContract_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.CONTRACT_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartContractAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartContract_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartContractAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndContract_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_NOTIFICATION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.CONTRACT_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.CONTRACT_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndContractAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndContract_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndContractAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StarNotification_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_NOTIFICATION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.NOTIFICATION_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = step;
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.CONTRACT_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = generalState;

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartNotification_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartNotificationAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndNotification_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.CLOSE_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.NOTIFICATION_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_NOTIFICATION, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.NOTIFICATION_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndNotification_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndNotificationAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task StarInspection_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW001");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_INSPECTION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.INSPECTION_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_INSPECTION, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.PAYMENT_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task StartInspection_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.StartInspectionAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndInspection_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW001");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.INSPECTION_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_INSPECTION, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.INSPECTION_STARTED, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndInspection_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndInspectionAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task CloseSale_WithData_ReturnResponseSuccess()
        {
            await LoadCatalogs();

            var flow = await PopulateFlowSteps();

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.CLOSE_SALE, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.NOTIFICATION_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);
            var expectedUri = new Uri("https://des-api-eva.novaecuador.com/configuration/api/configuration/catalog/getCatalogByCode?code=FLOW_STEPS&includeItems=true");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await trackingAppService.CloseSaleAsync(tracking.Result.Id);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(0),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task CloseSale_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.CloseSaleAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task EndProcess_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.TRACKING_ENDED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.ENDED, TrackingConsts.FLOW_GENERAL_STATES);

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.EndProcessAsync(tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowId, tracking.Result.FlowId);
            Assert.Equal(result.Result.StepId, step.CatalogId);
            Assert.Equal(result.Result.StateId, state.CatalogId);
            Assert.Equal(result.Result.GeneralStateId, generalState.CatalogId);
        }

        [Fact]
        public async Task EndProcess_WithNonExistsTracking_ReturnNotFoundById()
        {
            await LoadCatalogs();

            var result = await _trackingAppService.EndProcessAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task AddFailureLog_WithData_ReturnSuccess()
        {
            string busServices = "{'success':true,'result':5,'error':null,'targetUrl':' numero de reintentos','unAuthorizedRequest':true}";
            string integrationResponse = "{'success':true,'result':{'trackingId':'710392e7-f3df-e351-082f-3a05b481620f','message':'','type':'SALE','isActive':true,'attempts':15,'id':'00000000-0000-0000-0000-000000000000'},'error':null,'targetUrl':'','unAuthorizedRequest':true}";
            List<string> productResponses = new()
            {
                busServices,
                integrationResponse
            };
            var loggerA = GetRequiredService<ILogger<AppApiServiceManager>>();
            var loggerI = GetRequiredService<ILogger<IntegrationApiServiceManager>>();

            var handlerConfigurationMock = FakeHttpClient.GetMockWithHttpMessageHandlerSequence(productResponses);
            var httpClientConfiguration = new HttpClient(handlerConfigurationMock.Object);
            var mockHttpClientConfiguration = new AppApiServiceManager(httpClientConfiguration, loggerA);
            var integrationApiServiceManager = new IntegrationApiServiceManager(_appConfigurationManager, mockHttpClientConfiguration, loggerI);

            var loggerM = GetRequiredService<ILogger<MessengerApiServiceManager>>();
            string messengerResponse = "{'success':true,'result':'Accepted','error':null,'targetUrl':'','unAuthorizedRequest':true}";
            var handlerMessengerMock = FakeHttpClient.GetMockHttpMessageHandler(messengerResponse);
            var httpClientMessenger = new HttpClient(handlerMessengerMock.Object);
            var mockHttpClientMessenger = new AppApiServiceManager(httpClientMessenger, loggerA);
            var messengerApiServiceManager = new MessengerApiServiceManager(_appConfigurationManager, mockHttpClientMessenger, loggerM);

            var trackingAppService = new TrackingAppService(
           GetRequiredService<TrackingManager>(),
            GetRequiredService<CatalogManager>(),
            GetRequiredService<ProductManager>(),
            GetRequiredService<Eva.Insurtech.FlowManagers.Flows.FlowManager>(),
            GetRequiredService<IObjectMapper>(),
            GetRequiredService<IAppConfigurationManager>(),
            GetRequiredService<ILogger<TrackingAppService>>(),
            integrationApiServiceManager,
            messengerApiServiceManager
                );
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, step.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, state.CatalogId);
            var trackingUpdated = await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, generalState.CatalogId);

            var expectedState = await _catalogManager.GetByCodeAsync("PAYMENT_DONE", TrackingConsts.FLOW_STATES);
            var expectedGeneralState = await _catalogManager.GetByCodeAsync("ERROR", TrackingConsts.FLOW_GENERAL_STATES);

            var previousStateId = state.CatalogId;

            var input = await GetCreateFailureLog();
            var result = await trackingAppService.AddFailureLog(input, tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Method, input.Method);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Error, input.Error);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Detail, input.Detail);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().StateId, previousStateId);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().StepId, trackingUpdated.StepId);

            Assert.Equal(result.Result.GeneralStateId, expectedGeneralState.CatalogId);
            Assert.Equal(result.Result.StateId, expectedState.CatalogId);

            Assert.True(result.Result.FailureLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.Result.FailureLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task AddFailureLog_WithDataInSecondStep_ReturnSuccess()
        {
            string busServices = "{'success':true,'result':5,'error':null,'targetUrl':' numero de reintentos','unAuthorizedRequest':true}";
            string integrationResponse = "{'success':true,'result':{'trackingId':'7ff1e682-e657-b1c2-95cd-3a08b38c30cf','message':'','type':'SALE','isActive':true,'attempts':15,'id':'00000000-0000-0000-0000-000000000000'},'error':null,'targetUrl':'','unAuthorizedRequest':true}";
            List<string> productResponses = new()
            {
                busServices,
                integrationResponse
            };
            var loggerA = GetRequiredService<ILogger<AppApiServiceManager>>();
            var loggerI = GetRequiredService<ILogger<IntegrationApiServiceManager>>();

            var handlerConfigurationMock = FakeHttpClient.GetMockWithHttpMessageHandlerSequence(productResponses);
            var httpClientConfiguration = new HttpClient(handlerConfigurationMock.Object);
            var mockHttpClientConfiguration = new AppApiServiceManager(httpClientConfiguration, loggerA);
            var integrationApiServiceManager = new IntegrationApiServiceManager(_appConfigurationManager, mockHttpClientConfiguration, loggerI);

            var loggerM = GetRequiredService<ILogger<MessengerApiServiceManager>>();
            string messengerResponse = "{'success':true,'result':'Accepted','error':null,'targetUrl':'','unAuthorizedRequest':true}";
            var handlerMessengerMock = FakeHttpClient.GetMockHttpMessageHandler(messengerResponse);
            var httpClientMessenger = new HttpClient(handlerMessengerMock.Object);
            var mockHttpClientMessenger = new AppApiServiceManager(httpClientMessenger, loggerA);
            var messengerApiServiceManager = new MessengerApiServiceManager(_appConfigurationManager, mockHttpClientMessenger, loggerM);


            var trackingAppService = new TrackingAppService(GetRequiredService<TrackingManager>(),
            GetRequiredService<CatalogManager>(),
            GetRequiredService<ProductManager>(),
            GetRequiredService<Eva.Insurtech.FlowManagers.Flows.FlowManager>(),
            GetRequiredService<IObjectMapper>(),
            GetRequiredService<IAppConfigurationManager>(),
            GetRequiredService<ILogger<TrackingAppService>>(),
            integrationApiServiceManager,messengerApiServiceManager);

            var flow = await _flowManager.GetByCodeAsync("FLOW012");
            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_QUOTATION, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.QUOTATION_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BROK001",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);
            await _trackingManager.UpdateStepAsync(tracking.Result.Id, step.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, state.CatalogId);
            var trackingUpdated = await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, generalState.CatalogId);
            var expectedState = await _catalogManager.GetByCodeAsync("TRACKING_CREATED", TrackingConsts.FLOW_STATES);
            var expectedGeneralState = await _catalogManager.GetByCodeAsync("ERROR", TrackingConsts.FLOW_GENERAL_STATES);
            var previousStateId = state.CatalogId;

            var input = await GetCreateFailureLog();
            var result = await trackingAppService.AddFailureLog(input, tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Method, input.Method);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Error, input.Error);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().Detail, input.Detail);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().StateId, previousStateId);
            Assert.Equal(result.Result.FailureLogs.FirstOrDefault().StepId, trackingUpdated.StepId);

            Assert.Equal(result.Result.GeneralStateId, expectedGeneralState.CatalogId);
            Assert.Equal(result.Result.StateId, expectedState.CatalogId);

            Assert.True(result.Result.FailureLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.Result.FailureLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateFailureLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = await GetCreateFailureLog();

            var result = await _trackingAppService.AddFailureLog(input, Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task SetExtraProperties_WithNoData_ReturnTracking()
        {
            var trackings = await _trackingManager.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            await Assert.ThrowsAsync<AbpValidationException>(async () =>
            {
                await _trackingAppService.SetExtraPropertiesAsync(tracking.Id, null);
            });
        }

        [Fact]
        public async Task SetExtraProperties_WithUncompleteData_ReturnTracking()
        {
            var trackings = await _trackingManager.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary { { "", "" } };

            var result = await _trackingAppService.SetExtraPropertiesAsync(tracking.Id, input);

            Assert.NotNull(result);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NULL_EXTRA_PROPERTIES));
        }

        [Fact]
        public async Task SetExtraProperties_WithData_ReturnTracking()
        {
            var trackings = await _trackingManager.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary { { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" } };

            await _trackingAppService.SetExtraPropertiesAsync(tracking.Id, input);

            var result = await _trackingManager.GetAsync(tracking.Id);

            Assert.NotNull(result);
            Assert.Equal(input, result.ExtraProperties);
        }

        [Fact]
        public async Task GetExtraProperties_WithData_ReturnTracking()
        {
            var trackings = await _trackingManager.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary
            {
                { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" }
            };

            await _trackingManager.SetExtraPropertiesAsync(tracking.Id, input);

            var result = await _trackingAppService.GetByExtraPropertiesAsync(input);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(input, result.Result.ExtraProperties);
        }

        [Fact]
        public async Task GetExtraProperties_WithoutData_ReturnEmpty()
        {
            var input = new ExtraPropertyDictionary
            {
                { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" }
            };

            var result = await _trackingAppService.GetByExtraPropertiesAsync(input);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal(ErrorConsts.ERROR_NOT_FOUND_BY_ID, result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_InOrder_FinishWithEndedProcess()
        {
            var catalogs = await _catalogManager.GetAllAsync();
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var trackingEnded = catalogs.FirstOrDefault(x => x.Code.Equals(TrackingConsts.TRACKING_ENDED));
            var endProcess = catalogs.FirstOrDefault(x => x.Code.Equals(TrackingConsts.END_PROCESS));
            var ended = catalogs.FirstOrDefault(x => x.Code.Equals(TrackingConsts.ENDED));

            await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            await _trackingAppService.StartContractAsync(tracking.Result.Id);
            await _trackingAppService.EndContractAsync(tracking.Result.Id);
            await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            var result = await _trackingAppService.EndProcessAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Equal(trackingEnded.CatalogId, result.Result.StateId);
            Assert.Equal(endProcess.CatalogId, result.Result.StepId);
            Assert.Equal(ended.CatalogId, result.Result.GeneralStateId);
        }

        [Fact]
        public async Task ExecFlow_DisorderStart_FinishWithErrorStart()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            var result = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_INCORRECT_START_STEP", result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_DisorderEnd_FinishWithErrorEnd()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            var result = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_INCORRECT_END_STEP", result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_EndProcessBeforeEnd_FinishWithErrorEndProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            var result = await _trackingAppService.EndProcessAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_INCORRECT_END_STEP", result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_InOrderTryToStartAfterEnd_FinishWithErrorEndedProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            await _trackingAppService.StartContractAsync(tracking.Result.Id);
            await _trackingAppService.EndContractAsync(tracking.Result.Id);
            await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            var result = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_CLOSE_ON_LAST_STEP", result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_InOrderTryToStartAfterAbandon_FinishWithErrorEndedProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            await _trackingAppService.AbandonAsync(tracking.Result.Id);
            var result = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_CLOSE_ON_LAST_STEP", result.Error.Code);
        }

        [Fact]
        public async Task ExecFlow_InOrderTryToStartAfterTimedOut_FinishWithErrorEndedProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            await _trackingAppService.TimedOutAsync(tracking.Result.Id);
            var result = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.Null(result.Result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_CLOSE_ON_LAST_STEP", result.Error.Code);
        }

        [Fact]
        public async Task BackwardFlow_InRange_ReturnProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.CLOSE_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_REGISTERED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            await _trackingAppService.StartContractAsync(tracking.Result.Id);
            await _trackingAppService.EndContractAsync(tracking.Result.Id);
            var result = await _trackingAppService.BackwardStepsAsync(tracking.Result.Id, 2);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Null(result.Error);
            Assert.Equal(step.CatalogId, result.Result.StepId);
            Assert.Equal(state.CatalogId, result.Result.StateId);
            Assert.Equal(generalState.CatalogId, result.Result.GeneralStateId);
        }

        [Fact]
        public async Task BackwardFlow_NotInRange_ErrorProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            var result = await _trackingAppService.BackwardStepsAsync(tracking.Result.Id, 2);

            Assert.NotNull(result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_INCORRECT_BACKWARD_STEPS", result.Error.Code);
        }

        [Fact]
        public async Task ForewardFlow_InRange_ReturnProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            var result = await _trackingAppService.ForewardStepsAsync(tracking.Result.Id, 2);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Null(result.Error);
            Assert.Equal(step.CatalogId, result.Result.StepId);
            Assert.Equal(state.CatalogId, result.Result.StateId);
            Assert.Equal(generalState.CatalogId, result.Result.GeneralStateId);
        }

        [Fact]
        public async Task ForewardFlow_NotInRange_ReturnError()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            var result = await _trackingAppService.ForewardStepsAsync(tracking.Result.Id, 20);

            Assert.NotNull(result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_INCORRECT_FOREWARD_STEPS", result.Error.Code);
        }

        [Fact]
        public async Task MoveToSpecificFlowStep_InRange_ReturnProcess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SUBSCRIPTION_DONE, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            var result = await _trackingAppService.MoveToEspecificStepsAsync(tracking.Result.Id, 3);

            Assert.NotNull(result);
            Assert.NotNull(result.Result);
            Assert.Null(result.Error);
            Assert.Equal(step.CatalogId, result.Result.StepId);
            Assert.Equal(state.CatalogId, result.Result.StateId);
            Assert.Equal(generalState.CatalogId, result.Result.GeneralStateId);
        }

        [Fact]
        public async Task MoveToSpecificFlowStep_NotInRange_ReturnError()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var result = await _trackingAppService.MoveToEspecificStepsAsync(tracking.Result.Id, 30);

            Assert.NotNull(result);
            Assert.NotNull(result.Error);
            Assert.Equal("ERROR_STEP_OUT_OF_RANGE", result.Error.Code);
        }

        [Fact]
        public async Task IsCloseProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            var result = await _trackingAppService.IsClose(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsCloseProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);


            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsClose(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsCloseProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsClose(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task IsEndedProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            var result = await _trackingAppService.IsEnded(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsEndedProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsEnded(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsEndedProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsEnded(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task IsTimeoutProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            await _trackingAppService.TimedOutAsync(tracking.Result.Id);
            var result = await _trackingAppService.IsTimeout(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsTimeoutProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsTimeout(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsTimeoutProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsTimeout(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task IsAbandonedProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            await _trackingAppService.AbandonAsync(tracking.Result.Id);
            var result = await _trackingAppService.IsAbandoned(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsAbandonedProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsAbandoned(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsAbandonedProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsAbandoned(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task IsInProgressProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsInProgress(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsInProgressProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            await _trackingAppService.AbandonAsync(tracking.Result.Id);

            var result = await _trackingAppService.IsInProgress(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsInProgressProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsInProgress(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task IsInitializedProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            var result = await _trackingAppService.IsInitialized(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task IsInitializedProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.IsInitialized(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task IsInitializedProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.IsInitialized(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task HasErrorProcess_WithData_ReturnResponseTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var error = await _catalogManager.GetByCodeAsync(TrackingConsts.ERROR, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, error.CatalogId);

            var result = await _trackingAppService.HasError(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Result);
        }

        [Fact]
        public async Task HasErrorProcess_WithIncorrectGeneralState_ReturnResponseFalse()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var startStep = await _catalogManager.GetByCodeAsync(TrackingConsts.END_PROCESS, TrackingConsts.FLOW_STEPS);
            var startedState = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_DONE, TrackingConsts.FLOW_STATES);
            var inProgressState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, startStep.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, startedState.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, inProgressState.CatalogId);

            var result = await _trackingAppService.HasError(tracking.Result.Id);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.False(result.Result);
        }

        [Fact]
        public async Task HasErrorProcess_WithNonExistsTracking_ReturnError()
        {
            var result = await _trackingAppService.HasError(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task AddProcessLog_WithData_ReturnSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, step.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, state.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, generalState.CatalogId);

            var input = await GetCreateProcessLog();
            var result = await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);

            Assert.True(result.Result.ProcessLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.Result.ProcessLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateProcessLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = await GetCreateProcessLog();

            var result = await _trackingAppService.AddProcessLog(input, Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task GetProcessLogByTracking_WithData_ReturnSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW005");
            var subscriptionStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SUBSCRIPTION, TrackingConsts.FLOW_STEPS);
            var saleStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var closeSaleStep = await _catalogManager.GetByCodeAsync(TrackingConsts.START_CONTRACT, TrackingConsts.FLOW_STEPS);
            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);
            await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            var input = await GetCreateProcessLog(1);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(2);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(2);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id);
            await _trackingAppService.StartSaleAsync(tracking.Result.Id);

            input = await GetCreateProcessLog(3);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(4);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(3);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            await _trackingAppService.CloseSaleAsync(tracking.Result.Id);

            input = await GetCreateProcessLog(5);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(5);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            input = await GetCreateProcessLog(6);
            await _trackingAppService.AddProcessLog(input, tracking.Result.Id);

            var processLogsByTracking = await _trackingAppService.GetProcessLogsByTrackingStepActionAndVersionAsync(tracking.Result.Id);
            var processLogsByTrackingIdStep = await _trackingAppService.GetProcessLogsByTrackingStepActionAndVersionAsync(tracking.Result.Id, subscriptionStep.CatalogId);
            var processLogsByTrackingIdStepAction = await _trackingAppService.GetProcessLogsByTrackingStepActionAndVersionAsync(tracking.Result.Id, saleStep.CatalogId, "action3");
            var processLogsByTrackingIdStepActionVersion = await _trackingAppService.GetProcessLogsByTrackingStepActionAndVersionAsync(tracking.Result.Id, closeSaleStep.CatalogId, "action", 9);

            Assert.NotNull(processLogsByTracking.Result);
            Assert.Equal(9, processLogsByTracking.Result.Count);

            Assert.NotNull(processLogsByTrackingIdStep.Result);
            Assert.Equal(3, processLogsByTrackingIdStep.Result.Count);

            Assert.NotNull(processLogsByTrackingIdStepAction.Result);
            Assert.Equal(2, processLogsByTrackingIdStepAction.Result.Count);

            Assert.NotNull(processLogsByTrackingIdStepActionVersion.Result);
            Assert.Equal(1, processLogsByTrackingIdStepActionVersion.Result.Count);

        }

        [Fact]
        public async Task StartStep_OutOfTime_ReturnError()
        {

            Mock<HttpMessageHandler> handlerMock = GetFlowStatesTypeCatalogMock();
            TrackingAppService trackingAppService = GetCustomTrackingAppService(handlerMock);

            var flow = await _flowRepository.GetByCodeAsync("FLOW002");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await trackingAppService.CreateAsync(newTracking, flow.Id);
            await trackingAppService.StartQuotationAsync(tracking.Result.Id);
            await trackingAppService.EndQuotationAsync(tracking.Result.Id);

            await Task.Delay(72000);

            tracking = await trackingAppService.StartSubscriptionAsync(tracking.Result.Id);

            Assert.False(tracking.Success);
            Assert.Equal("ERROR_TRACKING_TIME_OUT", tracking.Error.Code);
        }

        [Fact]
        public async Task AddSubStepLog_WithData_ReturnSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW003");

            var step = await _catalogManager.GetByCodeAsync(TrackingConsts.START_SALE, TrackingConsts.FLOW_STEPS);
            var state = await _catalogManager.GetByCodeAsync(TrackingConsts.SALE_STARTED, TrackingConsts.FLOW_STATES);
            var generalState = await _catalogManager.GetByCodeAsync(TrackingConsts.IN_PROGRESS, TrackingConsts.FLOW_GENERAL_STATES);

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Id);

            await _trackingManager.UpdateStepAsync(tracking.Result.Id, step.CatalogId);
            await _trackingManager.UpdateStateAsync(tracking.Result.Id, state.CatalogId);
            await _trackingManager.UpdateGeneralStateAsync(tracking.Result.Id, generalState.CatalogId);

            var input = await GetCreateSubStepLog();
            var result = await _trackingAppService.AddSubStepLog(input, tracking.Result.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);

            Assert.True(result.Result.SubStepLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.Result.SubStepLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateSubStepLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = await GetCreateSubStepLog();

            var result = await _trackingAppService.AddSubStepLog(input, Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }




        #region Private Methods

        private async Task<FlowManagers.Flows.Flow> PopulateFlowSteps()
        {
            var flows = await _flowRepository.GetAllAsync();
            var flow = flows.FirstOrDefault();

            await AddFlowStep(flow.Id, Guid.Parse("5A185A91-633A-4E9A-4AE3-08D9D07442D2"), "flowmanager", 1);
            await AddFlowStep(flow.Id, Guid.Parse("ED8A3A4F-6342-4EB9-4AE2-08D9D07442D2"), "quotation", 2);
            await AddFlowStep(flow.Id, Guid.Parse("1D7D3EC0-910B-4D13-4AE4-08D9D07442D2"), "subscription", 3);
            await AddFlowStep(flow.Id, Guid.Parse("B26EF567-981A-42BE-4AE7-08D9D07442D2"), "sale", 4);
            await AddFlowStep(flow.Id, Guid.Parse("1F086F58-7E7A-4F71-4AE6-08D9D07442D2"), "payment", 5);
            await AddFlowStep(flow.Id, Guid.Parse("D2A5BB5A-D802-4B5C-4AE8-08D9D07442D2"), "contract", 6);
            await AddFlowStep(flow.Id, Guid.Parse("CAF69B6B-B61A-451C-4AE9-08D9D07442D2"), "notification", 7);
            await AddFlowStep(flow.Id, Guid.Parse("06748436-0011-498A-4AE5-08D9D07442D2"), "inspection", 8);
            await AddFlowStep(flow.Id, Guid.Parse("27314ADC-6E2B-40F8-4AEA-08D9D07442D2"), "sale", 9);
            await AddFlowStep(flow.Id, Guid.Parse("94D0A94B-8482-4396-4AEB-08D9D07442D2"), "flowmanager", 10);
            return flow;
        }

        private async Task<Mock<HttpMessageHandler>> LoadCatalogs()
        {
            Mock<HttpMessageHandler> handlerMock = GetFlowStepsTypeCatalogMock();
            CatalogManager catalogManager = GetCustomCatalogManager(handlerMock);
            await catalogManager.GetFromConfigurationContextByCatalogCode("FLOW_STEPS");

            handlerMock = GetFlowStatesTypeCatalogMock();
            catalogManager = GetCustomCatalogManager(handlerMock);
            await catalogManager.GetFromConfigurationContextByCatalogCode("FLOW_STATES");

            handlerMock = GetFlowGeneralStatesTypeCatalogMock();
            catalogManager = GetCustomCatalogManager(handlerMock);
            await catalogManager.GetFromConfigurationContextByCatalogCode("FLOW_GENERAL_STATES");
            return handlerMock;
        }

        private static Mock<HttpMessageHandler> GetFlowStepsTypeCatalogMock()
        {
            return FakeHttpClient.GetMockHttpMessageHandler(@"{
    'success': true,
    'result': {
        'code': 'FLOW_STEPS',
        'name': 'Pasos del flujo',
        'description': 'Pasos del flujo',
        'isExtending': false,
        'isActived': true,
        'catalogParentCode': null,
        'countryId': 'f261bd42-3b54-bc46-ff7c-3a013d8c8c0c',
        'catalogItems': [
            {
                'code': 'CLOSE_SALE',
                'name': 'Cerrar venta',
                'description': 'Cerrar venta',
                'order': 13,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '27314adc-6e2b-40f8-4aea-08d9d07442d2'
            },
            {
                'code': 'END_PROCESS',
                'name': 'Finalizar proceso',
                'description': 'Finalizar proceso',
                'order': 14,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '94d0a94b-8482-4396-4aeb-08d9d07442d2'
            },
            {
                'code': 'GET_FLOW',
                'name': 'Obtener flujo',
                'description': 'Obtener flujo',
                'order': 2,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '79d59da4-baea-4048-4adf-08d9d07442d2'
            },
            {
                'code': 'GET_PLANS',
                'name': 'Obtener planes',
                'description': 'Obtener planes',
                'order': 3,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '31da36d9-b74c-4d2c-4ae0-08d9d07442d2'
            },
            {
                'code': 'GET_PRODUCTS',
                'name': 'Obtener productos',
                'description': 'Obtener productos',
                'order': 1,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '91598434-6fad-42cc-4ade-08d9d07442d2'
            },
            {
                'code': 'START_CONTRACT',
                'name': 'Iniciar contrato',
                'description': 'Iniciar contrato',
                'order': 11,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': 'd2a5bb5a-d802-4b5c-4ae8-08d9d07442d2'
            },
            {
                'code': 'START_INSPECTION',
                'name': 'Iniciar inspección',
                'description': 'Iniciar inspección',
                'order': 8,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '06748436-0011-498a-4ae5-08d9d07442d2'
            },
            {
                'code': 'START_NOTIFICATION',
                'name': 'Iniciar notificación',
                'description': 'Iniciar notificación',
                'order': 12,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': 'caf69b6b-b61a-451c-4ae9-08d9d07442d2'
            },
            {
                'code': 'START_PAYMENT',
                'name': 'Iniciar pago',
                'description': 'Iniciar pago',
                'order': 9,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '1f086f58-7e7a-4f71-4ae6-08d9d07442d2'
            },
            {
                'code': 'START_QUOTATION',
                'name': 'Iniciar cotización',
                'description': 'Iniciar cotización',
                'order': 5,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': 'ed8a3a4f-6342-4eb9-4ae2-08d9d07442d2'
            },
            {
                'code': 'START_SALE',
                'name': 'Iniciar venta',
                'description': 'Iniciar venta',
                'order': 10,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': 'b26ef567-981a-42be-4ae7-08d9d07442d2'
            },
            {
                'code': 'START_SIMULATOR',
                'name': 'Iniciar simulador',
                'description': 'Iniciar simulador',
                'order': 4,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '094264e4-19ab-474e-4ae1-08d9d07442d2'
            },
            {
                'code': 'START_SUBSCRIPTION',
                'name': 'Iniciar suscripción',
                'description': 'Iniciar suscripción',
                'order': 7,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '1d7d3ec0-910b-4d13-4ae4-08d9d07442d2'
            },
            {
                'code': 'START_TRACKING',
                'name': 'Iniciar tracking',
                'description': 'Iniciar tracking',
                'order': 6,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02',
                'items': null,
                'id': '5a185a91-633a-4e9a-4ae3-08d9d07442d2'
            }
        ],
        'customCatalogs': [],
        'id': '7535fff0-5ad3-b6ef-9763-3a013d8e3c02'
    },
    'error': null,
    'targetUrl': '',
    'unAuthorizedRequest': true
}");
        }

        private static Mock<HttpMessageHandler> GetFlowStatesTypeCatalogMock()
        {
            return FakeHttpClient.GetMockHttpMessageHandler(@"{
    'success': true,
    'result': {
        'code': 'FLOW_STATES',
        'name': 'Estados del flujo',
        'description': 'Estados del flujo',
        'isExtending': false,
        'isActived': true,
        'catalogParentCode': null,
        'countryId': 'f261bd42-3b54-bc46-ff7c-3a013d8c8c0c',
        'catalogItems': [
            {
                'code': 'CONTRACT_DONE',
                'name': 'Contrato finalizado',
                'description': 'Contrato finalizado',
                'order': 18,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '44f66559-276a-428d-4afd-08d9d07442d2'
            },
            {
                'code': 'CONTRACT_ERROR',
                'name': 'Contrato con error',
                'description': 'Contrato con error',
                'order': 19,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '949e8d60-e459-42dc-4afe-08d9d07442d2'
            },
            {
                'code': 'CONTRACT_STARTED',
                'name': 'Contrato iniciado',
                'description': 'Contrato iniciado',
                'order': 17,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '3b1cd3dc-d18a-4bf7-4afc-08d9d07442d2'
            },
            {
                'code': 'DOCUMENTATION_DONE',
                'name': 'Documento finalizado',
                'description': 'Documento finalizado',
                'order': 21,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '965d6e7a-24a5-495d-4b00-08d9d07442d2'
            },
            {
                'code': 'DOCUMENTATION_ERROR',
                'name': 'Documento con error',
                'description': 'Documento con error',
                'order': 22,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '8053cbed-efa8-4936-4b01-08d9d07442d2'
            },
            {
                'code': 'DOCUMENTATION_STARTED',
                'name': 'Documento iniciado',
                'description': 'Documento iniciado',
                'order': 20,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'd4b0f2d8-bef4-471c-4aff-08d9d07442d2'
            },
            {
                'code': 'INSPECTION_DONE',
                'name': 'Inspección finalizada',
                'description': 'Inspección finalizada',
                'order': 27,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'e00de7a0-ec75-42d5-4b06-08d9d07442d2'
            },
            {
                'code': 'INSPECTION_ERROR',
                'name': 'Inspección con error',
                'description': 'Inspección con error',
                'order': 28,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '1ca4f940-c58c-474e-4b07-08d9d07442d2'
            },
            {
                'code': 'INSPECTION_STARTED',
                'name': 'Inspección iniciada',
                'description': 'Inspección iniciada',
                'order': 26,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '97c75dd9-1c4c-4bc1-4b05-08d9d07442d2'
            },
            {
                'code': 'NOTIFICATION_DONE',
                'name': 'Notificación finalizada',
                'description': 'Notificación finalizada',
                'order': 24,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '34a95a66-1819-4df4-4b03-08d9d07442d2'
            },
            {
                'code': 'NOTIFICATION_ERROR',
                'name': 'Notificación con error',
                'description': 'Notificación con error',
                'order': 25,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '601e6d64-8e4b-4e73-4b04-08d9d07442d2'
            },
            {
                'code': 'NOTIFICATION_STARTED',
                'name': 'Notificación iniciada',
                'description': 'Notificación iniciada',
                'order': 23,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'a1f6a4fe-8623-4287-4b02-08d9d07442d2'
            },
            {
                'code': 'PAYMENT_DONE',
                'name': 'Pago finalizado',
                'description': 'Pago finalizado',
                'order': 15,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '4cefe6d7-6e96-4fef-4afa-08d9d07442d2'
            },
            {
                'code': 'PAYMENT_ERROR',
                'name': 'Pago con error',
                'description': 'Pago con error',
                'order': 16,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'c51d63b2-a342-45ac-4afb-08d9d07442d2'
            },
            {
                'code': 'PAYMENT_STARTED',
                'name': 'Pago iniciado',
                'description': 'Pago iniciado',
                'order': 14,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '072b2d11-3367-4194-4af9-08d9d07442d2'
            },
            {
                'code': 'QUOTATION_DONE',
                'name': 'Cotización finalizada',
                'description': 'Cotización finalizada',
                'order': 3,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '6d2e2716-e258-4362-4aee-08d9d07442d2'
            },
            {
                'code': 'QUOTATION_ERROR',
                'name': 'Cotización con error',
                'description': 'Cotización con error',
                'order': 4,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'b9dee9bf-a612-492c-4aef-08d9d07442d2'
            },
            {
                'code': 'QUOTATION_STARTED',
                'name': 'Cotización iniciada',
                'description': 'Cotización iniciada',
                'order': 2,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'd571916c-8c57-42e4-4aed-08d9d07442d2'
            },
            {
                'code': 'SALE_DONE',
                'name': 'Venta finalizada',
                'description': 'Venta finalizada',
                'order': 13,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '4bbd7357-4f00-4db2-4af8-08d9d07442d2'
            },
            {
                'code': 'SALE_ERROR',
                'name': 'Venta con error',
                'description': 'Venta con error',
                'order': 10,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'e1c0d429-5d9d-4460-4af5-08d9d07442d2'
            },
            {
                'code': 'SALE_PAYED',
                'name': 'Venta pagada',
                'description': 'Venta pagada',
                'order': 12,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '5e20d5d9-e103-4e94-4af7-08d9d07442d2'
            },
            {
                'code': 'SALE_PAYMENT_PENDING',
                'name': 'Venta pago pendiente',
                'description': 'Venta pago pendiente',
                'order': 11,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '98338665-3afb-49b4-4af6-08d9d07442d2'
            },
            {
                'code': 'SALE_REGISTERED',
                'name': 'Venta finalizada',
                'description': 'Venta finalizada',
                'order': 9,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'b9339cdf-097c-4d96-4af4-08d9d07442d2'
            },
            {
                'code': 'SALE_STARTED',
                'name': 'Venta iniciada',
                'description': 'Venta iniciada',
                'order': 8,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '2c89c279-20cc-482e-4af3-08d9d07442d2'
            },
            {
                'code': 'SUBSCRIPTION_DONE',
                'name': 'Suscripción finalizada',
                'description': 'Suscripción finalizada',
                'order': 6,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'ae27d046-02b5-4d05-4af1-08d9d07442d2'
            },
            {
                'code': 'SUBSCRIPTION_ERROR',
                'name': 'Suscripción con error',
                'description': 'Suscripción con error',
                'order': 7,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '778f508d-0a62-4cb1-4af2-08d9d07442d2'
            },
            {
                'code': 'SUBSCRIPTION_STARTED',
                'name': 'Suscripción iniciada',
                'description': 'Suscripción iniciada',
                'order': 5,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': 'db04a437-de11-49b1-4af0-08d9d07442d2'
            },
            {
                'code': 'TRACKING_CREATED',
                'name': 'Tracking creado',
                'description': 'Tracking creado',
                'order': 1,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '75459cf1-1b52-40ca-4aec-08d9d07442d2'
            },
            {
                'code': 'TRACKING_ENDED',
                'name': 'Tracking terminado',
                'description': 'Tracking terminado',
                'order': 29,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4',
                'items': null,
                'id': '2bec830f-9100-4647-8274-e697b188c545'
            }
        ],
        'customCatalogs': [],
        'id': 'dd6f1da3-1b60-1614-54f1-3a013d8e59f4'
    },
    'error': null,
    'targetUrl': '',
    'unAuthorizedRequest': true
}");
        }

        private static Mock<HttpMessageHandler> GetFlowGeneralStatesTypeCatalogMock()
        {
            return FakeHttpClient.GetMockHttpMessageHandler(@"{
    'success': true,
    'result': {
        'code': 'FLOW_GENERAL_STATES',
        'name': 'Estados generales del flujo',
        'description': 'Estados generales del flujo',
        'isExtending': false,
        'isActived': true,
        'catalogParentCode': null,
        'countryId': 'f261bd42-3b54-bc46-ff7c-3a013d8c8c0c',
        'catalogItems': [
            {
                'code': 'ABANDONED',
                'name': 'Abandonado',
                'description': 'Abandonado',
                'order': 5,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'e23b9158-d898-40b4-4b0c-08d9d07442d2'
            },
            {
                'code': 'ENDED',
                'name': 'Terminado',
                'description': 'Terminado',
                'order': 3,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'b02a3893-7a7b-4c30-4b0a-08d9d07442d2'
            },
            {
                'code': 'ERROR',
                'name': 'Error',
                'description': 'Error',
                'order': 6,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'f81a0521-f3a8-4861-4b0d-08d9d07442d2'
            },
            {
                'code': 'IN_PROGRESS',
                'name': 'En progreso',
                'description': 'En progreso',
                'order': 2,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'db54ecc4-39d5-46d1-4b09-08d9d07442d2'
            },
            {
                'code': 'INITIALIZED',
                'name': 'Iniciado',
                'description': 'Iniciado',
                'order': 1,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'd94a148c-4dc4-454f-4b08-08d9d07442d2'
            },
            {
                'code': 'TIMED_OUT',
                'name': 'Caducado',
                'description': 'Caducado',
                'order': 4,
                'isComplexObject': false,
                'associatedValue': null,
                'itemParentId': '00000000-0000-0000-0000-000000000000',
                'isActived': true,
                'catalogId': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d',
                'items': null,
                'id': 'd29cf2b6-1c30-4d65-4b0b-08d9d07442d2'
            }
        ],
        'customCatalogs': [],
        'id': 'c8b15ac4-5e78-359e-f29a-3a013d8e948d'
    },
    'error': null,
    'targetUrl': '',
    'unAuthorizedRequest': true
}");
        }

        private TrackingAppService GetCustomTrackingAppService(Mock<HttpMessageHandler> handlerMock)
        {
            var httpClient = new HttpClient(handlerMock.Object);
            var apiServiceManager = new ApiServiceManager(httpClient);
            var catalogManager = new CatalogManager(_catalogRepository, apiServiceManager, _appConfigurationManager);
            var trackingAppService = new TrackingAppService
                (_trackingManager, catalogManager, _productManager, _flowManager, _objectMapper,
                _appConfigurationManager, _logger, _integrationApiServiceManager, _messengerApiServiceManager
                );
            return trackingAppService;
        }

        private CatalogManager GetCustomCatalogManager(Mock<HttpMessageHandler> handlerMock)
        {
            var httpClient = new HttpClient(handlerMock.Object);
            var apiServiceManager = new ApiServiceManager(httpClient);
            var catalogManager = new CatalogManager(_catalogRepository, apiServiceManager, _appConfigurationManager);
            return catalogManager;
        }

        private async Task<CreateFailureLogDto> GetCreateFailureLog()
        {
            var catalog = await _catalogManager.GetByCodeAsync("SALE_ERROR", TrackingConsts.FLOW_STATES);

            var input = new CreateFailureLogDto()
            {
                Method = "GetSubscription",
                Error = "Error 403 - This web app is stopped",
                Detail = "The web app you have attempted to reach is currently stopped and does not accept any requests. Please try to reload the page or visit it again soon. If you are the web app administrator, please find the common 403 error scenarios and resolution here. For further troubleshooting tools and recommendations, please visit Azure Portal.",
                StateId = catalog.CatalogId
            };

            return await Task.FromResult(input);
        }

        private static async Task<CreateFlowStepDto> GetFlowStepDto(Guid stepId, string endPoint, int order)
        {
            var input = new CreateFlowStepDto()
            {
                EndPointService = endPoint,
                Order = order,
                QueueService = "",
                StepId = stepId
            };
            return await Task.FromResult(input);
        }

        private async Task AddFlowStep(Guid flowId, Guid stepId, string endPoint, int order)
        {
            var input = await GetFlowStepDto(stepId, endPoint, order);
            await _flowAppService.AddFlowStep(input, flowId);
        }

        private static async Task<CreateProcessLogDto> GetCreateProcessLog(int actionOrder = 0)
        {
            var input = new CreateProcessLogDto()
            {
                Action = $"Method.Action{actionOrder}",
                Request = "Request",
                Response = "Response"
            };

            return await Task.FromResult(input);
        }

        private static async Task<CreateSubStepLogDto> GetCreateSubStepLog()
        {
            var input = new CreateSubStepLogDto()
            {
                SubStepCode = "TEST_STEP"
            };

            return await Task.FromResult(input);
        }



        #endregion
    }

}