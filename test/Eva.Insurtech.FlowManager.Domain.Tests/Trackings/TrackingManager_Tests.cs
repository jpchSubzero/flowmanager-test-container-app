using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Catalogs.Exceptions;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.Exceptions;
using Eva.Insurtech.FlowManagers.Products.Exceptions;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Xunit;

namespace Eva.Insurtech.TrackingManagers.Trackings
{
    [ExcludeFromCodeCoverage]
    public class TrackingManager_Tests : FlowManagerDomainTestBase
    {
        private readonly ITrackingRepository _trackingRepository;
        private readonly IFlowRepository _flowRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly TrackingManager _trackingManager;

        public TrackingManager_Tests()
        {
            _trackingRepository = GetRequiredService<ITrackingRepository>();
            _catalogRepository = GetRequiredService<ICatalogRepository>();
            _catalogRepository = GetRequiredService<ICatalogRepository>();
            _flowRepository = GetRequiredService<IFlowRepository>();

            _trackingManager = GetRequiredService<TrackingManager>();
        }

        [Fact]
        public async Task CreateTracking_WithNullData_ReturnNullException()
        {
            Tracking tracking = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task CreateTracking_WithData_ReturnTracking()
        {
            var tracking = await GetCreate();

            var result = await _trackingManager.InsertAsync(tracking);

            Assert.NotNull(result);
            Assert.Equal(result.StepId, tracking.StepId);
            Assert.Equal(result.FlowId, tracking.FlowId);
            Assert.Equal(result.StateId, tracking.StateId);
            Assert.Equal(result.GeneralStateId, tracking.GeneralStateId);

            Assert.NotEqual(result.Start, DateTime.MinValue);
            Assert.NotEqual(result.ChangeState, DateTime.MinValue);
            Assert.Null(result.End);
            Assert.Null(result.Abandon);
        }

        [Fact]
        public async Task CreateTracking_WithNonExistentFlow_ReturnFlowNotFoundException()
        {
            var tracking = await GetCreate();
            tracking.SetFlowId(Guid.NewGuid());

            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task CreateTracking_WithNonExistentState_ReturnCatalogNotFoundException()
        {
            var tracking = await GetCreate();
            tracking.SetStateId(Guid.NewGuid());

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task CreateTracking_WithNonExistentStep_ReturnCatalogNotFoundException()
        {
            var tracking = await GetCreate();
            tracking.SetStepId(Guid.NewGuid());

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task CreateTracking_WithNonExistentGeneralState_ReturnCatalogNotFoundException()
        {
            var tracking = await GetCreate();
            tracking.SetGeneralStateId(Guid.NewGuid());

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task CreateTracking_WithNonExistentChannel_ReturnCatalogNotFoundException()
        {
            var tracking = await GetCreateWithoutChannel();

            await Assert.ThrowsAsync<ChannelNotFoundException>(async () =>
            {
                await _trackingManager.InsertAsync(tracking);
            });
        }

        [Fact]
        public async Task GetTracking_WithExistsTracking_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var result = await _trackingManager.GetAsync(tracking.Id);

            Assert.NotNull(result);
            Assert.Equal(result.StepId, tracking.StepId);
            Assert.Equal(result.FlowId, tracking.FlowId);
            Assert.Equal(result.StateId, tracking.StateId);
            Assert.Equal(result.GeneralStateId, tracking.GeneralStateId);

            Assert.NotEqual(result.Start, DateTime.MinValue);
            Assert.NotEqual(result.ChangeState, DateTime.MinValue);
            Assert.Null(result.End);
            Assert.Null(result.Abandon);
        }

        [Fact]
        public async Task GetTracking_WithNonExistsTracking_ReturnNullException()
        {
            var trackingResponse = await _trackingManager.GetAsync(Guid.NewGuid());
            Assert.Null(trackingResponse);
        }

        [Fact]
        public async Task UpdateTrackingStep_WithExistsStep_ReturnTracking()
        {
            var catalog = await _catalogRepository.GetByCodeAsync("CATALOG06");

            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var trackingToUpdate = await _trackingManager.UpdateStepAsync(tracking.Id, catalog.CatalogId);

            Assert.NotNull(trackingToUpdate);
            Assert.NotEqual(trackingToUpdate.StepId, tracking.StepId);
            Assert.Equal(trackingToUpdate.FlowId, tracking.FlowId);
            Assert.Equal(trackingToUpdate.StateId, tracking.StateId);
            Assert.Equal(trackingToUpdate.GeneralStateId, tracking.GeneralStateId);

            Assert.NotEqual(trackingToUpdate.Start, DateTime.MinValue);
            Assert.NotEqual(trackingToUpdate.ChangeState, DateTime.MinValue);
            Assert.Null(trackingToUpdate.End);
            Assert.Null(trackingToUpdate.Abandon);
        }

        [Fact]
        public async Task UpdateTrackingStep_WithNonExistsStep_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.UpdateStepAsync(tracking.Id, Guid.NewGuid());
            });
        }

        [Fact]
        public async Task UpdateTrackingState_WithExistsState_ReturnTracking()
        {
            var catalog = await _catalogRepository.GetByCodeAsync("CATALOG06");

            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var trackingToUpdate = await _trackingManager.UpdateStateAsync(tracking.Id, catalog.CatalogId);

            Assert.NotNull(trackingToUpdate);
            Assert.Equal(trackingToUpdate.StepId, tracking.StepId);
            Assert.Equal(trackingToUpdate.FlowId, tracking.FlowId);
            Assert.NotEqual(trackingToUpdate.StateId, tracking.StateId);
            Assert.Equal(trackingToUpdate.GeneralStateId, tracking.GeneralStateId);

            Assert.NotEqual(trackingToUpdate.Start, DateTime.MinValue);
            Assert.NotEqual(trackingToUpdate.ChangeState, DateTime.MinValue);
            Assert.Null(trackingToUpdate.End);
            Assert.Null(trackingToUpdate.Abandon);
        }

        [Fact]
        public async Task UpdateTrackingState_WithNonExistsState_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.UpdateStateAsync(tracking.Id, Guid.NewGuid());
            });
        }

        [Fact]
        public async Task UpdateTrackingGeneralState_WithExistsGeneralState_ReturnTracking()
        {
            var catalog = await _catalogRepository.GetByCodeAsync("CATALOG06");

            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var trackingToUpdate = await _trackingManager.UpdateGeneralStateAsync(tracking.Id, catalog.CatalogId);

            Assert.NotNull(trackingToUpdate);
            Assert.Equal(trackingToUpdate.StepId, tracking.StepId);
            Assert.Equal(trackingToUpdate.FlowId, tracking.FlowId);
            Assert.Equal(trackingToUpdate.StateId, tracking.StateId);
            Assert.NotEqual(trackingToUpdate.GeneralStateId, tracking.GeneralStateId);

            Assert.NotEqual(trackingToUpdate.Start, DateTime.MinValue);
            Assert.NotEqual(trackingToUpdate.ChangeState, DateTime.MinValue);
            Assert.Null(trackingToUpdate.End);
            Assert.Null(trackingToUpdate.Abandon);
        }

        [Fact]
        public async Task UpdateTrackingGeneralState_WithNonExistsGeneralState_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _trackingManager.UpdateGeneralStateAsync(tracking.Id, Guid.NewGuid());
            });
        }

        [Fact]
        public async Task CreateFailureLog_WithData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = GetOneFailureLogInput(tracking.Id);

            var result = await _trackingManager.AddFailureLogAsync(input);

            Assert.NotNull(result.FailureLogs);
            Assert.Equal(result.FailureLogs.FirstOrDefault().Method, input.Method);
            Assert.Equal(result.FailureLogs.FirstOrDefault().Error, input.Error);
            Assert.Equal(result.FailureLogs.FirstOrDefault().Detail, input.Detail);
            Assert.Equal(result.FailureLogs.FirstOrDefault().TrackingId, input.TrackingId);
            Assert.Equal(result.FailureLogs.FirstOrDefault().StepId, tracking.StepId);
            Assert.True(result.FailureLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.FailureLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateFailureLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = GetOneFailureLogInput(Guid.NewGuid());

            await Assert.ThrowsAsync<TrackingNotFoundException>(async () =>
            {
                await _trackingManager.AddFailureLogAsync(input);
            });
        }

        [Fact]
        public async Task SetExtraProperties_WithNoData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            await Assert.ThrowsAsync<TrackingExtraPropertiesNullException>(async () =>
            {
            await _trackingManager.SetExtraPropertiesAsync(tracking.Id, null);
            });
        }

        [Fact]
        public async Task SetExtraProperties_WithUncompleteData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary
            {
                { "", "" }
            };

            await Assert.ThrowsAsync<TrackingExtraPropertiesNullException>(async () =>
            {
                await _trackingManager.SetExtraPropertiesAsync(tracking.Id, input);
            });
        }

        [Fact]
        public async Task SetExtraProperties_WithData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary
            {
                { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" }
            };

            var result = await _trackingManager.SetExtraPropertiesAsync(tracking.Id, input);

            Assert.NotNull(result);
            Assert.Equal(input, result.ExtraProperties);
        }

        [Fact]
        public async Task GetExtraProperties_WithData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = new ExtraPropertyDictionary
            {
                { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" }
            };

            await _trackingManager.SetExtraPropertiesAsync(tracking.Id, input);

            var result = await _trackingManager.GetByExtraPropertiesAsync(input);

            Assert.NotNull(result);
            Assert.Equal(input, result.ExtraProperties);
        }

        [Fact]
        public async Task GetExtraProperties_WithoutData_ReturnEmpty()
        {
            var input = new ExtraPropertyDictionary
            {
                { "TransactionReference", "82592564-40d6-e6bf-2d42-3a017bffe69c" }
            };

            var result = await _trackingManager.GetByExtraPropertiesAsync(input);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProcessLog_WithData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = GetCreateProcessLog(tracking.Id);

            var result = await _trackingManager.AddProcessLogAsync(input);

            Assert.NotNull(result.ProcessLogs);
            Assert.Equal(result.ProcessLogs.FirstOrDefault().TrackingId, input.TrackingId);
            Assert.Equal(result.ProcessLogs.FirstOrDefault().StepId, tracking.StepId);
            Assert.True(result.ProcessLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.ProcessLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateProcessLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = GetCreateProcessLog(Guid.NewGuid());

            await Assert.ThrowsAsync<TrackingNotFoundException>(async () =>
            {
                await _trackingManager.AddProcessLogAsync(input);
            });
        }

        [Fact]
        public async Task CreateSubStepLog_WithData_ReturnTracking()
        {
            var trackings = await _trackingRepository.GetListAsync();
            var tracking = trackings.FirstOrDefault();

            var input = GetCreateSubStepLog(tracking.Id);

            var result = await _trackingManager.AddSubStepLogAsync(input);

            Assert.NotNull(result.SubStepLogs);
            Assert.Equal(result.SubStepLogs.FirstOrDefault().TrackingId, input.TrackingId);
            Assert.Equal(result.SubStepLogs.FirstOrDefault().StepId, tracking.StepId);
            Assert.True(result.SubStepLogs.FirstOrDefault().RegisterTime < DateTime.Now && result.SubStepLogs.FirstOrDefault().RegisterTime > DateTime.Now.AddMinutes(-2));
        }

        [Fact]
        public async Task CreateSubStepLog_WithNonExistsTracking_ReturnTracking()
        {
            var input = GetCreateSubStepLog(Guid.NewGuid());

            await Assert.ThrowsAsync<TrackingNotFoundException>(async () =>
            {
                await _trackingManager.AddSubStepLogAsync(input);
            });
        }


        #region Private Methods


        private async Task<Tracking> GetCreate()
        {
            var flows = await _flowRepository.GetAllAsync();
            var flow = flows.FirstOrDefault();

            var catalog1 = await _catalogRepository.GetByCodeAsync("CATALOG01");
            var catalog2 = await _catalogRepository.GetByCodeAsync("CATALOG02");
            var catalog3 = await _catalogRepository.GetByCodeAsync("CATALOG03");

            var tracking = new Tracking(
                flow.Id,
                catalog1.CatalogId,
                catalog2.CatalogId,
                catalog3.CatalogId,
                "BANCO01",
                "BANCAMOVIL",
                "192.168.0.1"
            );
            return tracking;
        }

        private async Task<Tracking> GetCreateWithoutChannel()
        {
            var flows = await _flowRepository.GetAllAsync();
            var flow = flows.FirstOrDefault();

            var catalog1 = await _catalogRepository.GetByCodeAsync("CATALOG01");
            var catalog2 = await _catalogRepository.GetByCodeAsync("CATALOG02");
            var catalog3 = await _catalogRepository.GetByCodeAsync("CATALOG03");

            var tracking = new Tracking(
                flow.Id,
                catalog1.CatalogId,
                catalog2.CatalogId,
                catalog3.CatalogId,
                "NON_EXISTENT",
                "BANCAMOVIL",
                "192.168.0.1"
            );
            return tracking;
        }

        private static FailureLogInput GetOneFailureLogInput(Guid trackingId)
        {
            var failureLogInput = new FailureLogInput()
            {
                TrackingId = trackingId,
                Method = "GetSubscription",
                Error = "Error 403 - This web app is stopped",
                Detail = "The web app you have attempted to reach is currently stopped and does not accept any requests. Please try to reload the page or visit it again soon. If you are the web app administrator, please find the common 403 error scenarios and resolution here. For further troubleshooting tools and recommendations, please visit Azure Portal."
            };

            return failureLogInput;
        }

        private static ProcessLogInput GetCreateProcessLog(Guid trackingId)
        {
            var input = new ProcessLogInput()
            {
                TrackingId = trackingId,
                Action = "Action",
                Request = "Request",
                Response = "Response"
            };

            return input;
        }

        private static SubStepLogInput GetCreateSubStepLog(Guid trackingId)
        {
            var input = new SubStepLogInput()
            {
                TrackingId = trackingId,
                Attempts = 1,
                SubStepCode = "TEST_STEP"
            };

            return input;
        }

        #endregion    
    }
}
