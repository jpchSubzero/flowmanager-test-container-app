using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Catalogs.Exceptions;
using Eva.Insurtech.FlowManagers.Flows.Exceptions;
using Eva.Insurtech.FlowManagers.Flows.Inputs;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Products.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Eva.Insurtech.FlowManagers.Flows
{
    [ExcludeFromCodeCoverage]
    public class FlowManager_Tests : FlowManagerDomainTestBase
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly FlowManager _flowManager;

        public FlowManager_Tests()
        {
            _flowRepository = GetRequiredService<IFlowRepository>();
            _productRepository = GetRequiredService<IProductRepository>();
            _flowManager = GetRequiredService<FlowManager>();
            _catalogRepository = GetRequiredService<ICatalogRepository>();
        }

        [Fact]
        public async Task CreateFlow_WithNullData_ReturnNullException()
        {
            Flow flow = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _flowManager.InsertAsync(flow);
            });
        }

        [Fact]
        public async Task CreateFlow_WithDataAndExistProduct_ReturnFlow()
        {
            var flow = await GetCreate();

            var result = await _flowManager.InsertAsync(flow);

            Assert.NotNull(result);
            Assert.Equal(flow.Code, result.Code);
            Assert.Equal(flow.Name, result.Name);
            Assert.Equal(flow.Description, result.Description);
            Assert.Equal(flow.IsActive, result.IsActive);
            Assert.Equal(flow.ProductId, result.ProductId);
        }

        [Fact]
        public async Task CreateFlow_WithNonExistentProduct_ReturnFlow()
        {
            var flow = await GetCreate();
            flow.SetProductId(Guid.NewGuid());

            await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
            {
                await _flowManager.InsertAsync(flow);
            });
        }

        [Fact]
        public async Task CreateFlow_WithExistentFlow_ReturnAlreadyExistException()
        {
            var product = await GetCreateExistent();

            await Assert.ThrowsAsync<FlowAlreadyExistException>(async () =>
            {
                await _flowManager.InsertAsync(product);
            });
        }

        [Fact]
        public async Task UpdateFlow_WithDataAndExistProduct_ReturnFlow()
        {
            var flows = await _flowRepository.GetListAsync();
            var flowToUpdated = flows.FirstOrDefault();

            var flowUpdated = await GetUpdate();

            flowToUpdated.SetCode(flowUpdated.Code);
            flowToUpdated.SetName(flowUpdated.Name);
            flowToUpdated.SetDescription(flowUpdated.Description);
            flowToUpdated.SetProductId(flowUpdated.ProductId);

            var result = await _flowManager.UpdateAsync(flowToUpdated);

            Assert.NotNull(result);
            Assert.Equal(result.Name, flowUpdated.Name);
            Assert.Equal(result.Description, flowUpdated.Description);
            Assert.Equal(result.ProductId, flowUpdated.ProductId);
        }

        [Fact]
        public async Task UpdateFlow_WithDataAndNonExistProduct_ReturnFlowNotFoundException()
        {
            var flows = await _flowRepository.GetListAsync();
            var flowToUpdated = flows.LastOrDefault();
            flowToUpdated.SetProductId(Guid.NewGuid());

            await Assert.ThrowsAsync<ProductNotFoundException>(async () =>
            {
                await _flowManager.UpdateAsync(flowToUpdated);
            });
        }

        [Fact]
        public async Task GetFlowById_WithExistsFlow_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowManager.GetAsync(flow.Id);

            Assert.Equal(result.Code, flow.Code);
            Assert.Equal(result.Name, flow.Name);
            Assert.Equal(result.Description, flow.Description);
            Assert.Equal(result.ProductId, flow.ProductId);
        }

        [Fact]
        public async Task GetFlowById_WithNonExistsFlow_ReturnNullException()
        {
            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _flowManager.GetAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task GetFlowByCode_WithExistsFlow_ReturnFlow()
        {
            var result = await _flowManager.GetByCodeAsync("FLOW02");

            Assert.NotNull(result);
            Assert.Equal("FLOW02", result.Code);
        }

        [Fact]
        public async Task GetFlowByCode_WithNonExistsFlow_ReturnNullException()
        {
            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _flowManager.GetByCodeAsync("NON_EXISTS");
            });
        }

        [Fact]
        public async Task DeleteFlow_WithExistsFlow_ReturnTrue()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowManager.DeleteAsync(flow.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task Deletelow_WithNonExistsFlow_ReturnNullException()
        {
            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _flowManager.DeleteAsync(Guid.NewGuid());
            });
        }

        [Fact]
        public async Task AddFlowStep_WithDataAndExistFlowStep_ReturnFlowWithFlowStep()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var catalog = await _catalogRepository.GetByCodeAsync("CATALOG01");
            var input = FlowStepInput(flow.Id, catalog.CatalogId);

            var result = await _flowManager.AddFlowStep(input);

            Assert.NotNull(result);
            Assert.Equal(result.FlowSteps.FirstOrDefault().StepId, input.StepId);
            Assert.Equal(result.FlowSteps.FirstOrDefault().FlowId, input.FlowId);
            Assert.Equal(result.FlowSteps.FirstOrDefault().EndPointService, input.EndPointService);
            Assert.Equal(result.FlowSteps.FirstOrDefault().QueueService, input.QueueService);
        }

        [Fact]
        public async Task AddFlowStep_WithDataAndNonExisFlowStep_ReturnCatalogNotFoundException()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var input = FlowStepInput(flow.Id, Guid.NewGuid());

            await Assert.ThrowsAsync<CatalogNotFoundException>(async () =>
            {
                await _flowManager.AddFlowStep(input);
            });
        }

        [Fact]
        public async Task GetFlowByProductId_WithExistsProduct_ReturnFlows()
        {
            var code = "FLOW01";
            var flow = await _flowManager.GetByCodeAsync(code);

            var result = await _flowManager.GetByProductIdAsync(flow.ProductId);

            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.NotNull(result.FirstOrDefault(x => x.Code.Equals(code)));
        }

        [Fact]
        public async Task GetFlowByProductId_WithNonExistsProduct_ReturnEmpty()
        {
            var result = await _flowManager.GetByProductIdAsync(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.False(result.Any());
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithExistsProductAndChannel_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowManager.GetByProductIdAndChannelCodeAsync(flow.ProductId, flow.ChannelCode);

            Assert.NotNull(result);
            Assert.Equal(result.Code, flow.Code);
            Assert.Equal(result.ChannelCode, flow.ChannelCode);
            Assert.Equal(result.Name, flow.Name);
            Assert.Equal(result.Description, flow.Description);
            Assert.Equal(result.ProductId, flow.ProductId);
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithNonExistsProduct_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _flowManager.GetByProductIdAndChannelCodeAsync(Guid.NewGuid(), flow.ChannelCode);
            });
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithNonExistsChannel_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            await Assert.ThrowsAsync<FlowNotFoundException>(async () =>
            {
                await _flowManager.GetByProductIdAndChannelCodeAsync(flow.ProductId, "NON_EXISTS");
            });
        }



        #region Private Methods

        private async Task<Flow> GetCreate()
        {
            var products = await _productRepository.GetAllAsync();
            var product = products.FirstOrDefault();
            var flow = new Flow(
                    "FLOW14",
                    "BANCO01",
                    "Flujo 4",
                    "Descripción Flujo 4",
                    product.ProductId
            );
            return flow;
        }

        private async Task<Flow> GetCreateExistent()
        {
            var flow = new Flow(
                    "FLOW01",
                    "BANCO01",
                    "Flujo 1",
                    "Descripción Flujo 1",
                    Guid.NewGuid()
            );
            return await Task.FromResult(flow);
        }

        private async Task<Flow> GetUpdate()
        {
            var products = await _productRepository.GetAllAsync();
            var product = products.LastOrDefault();
            var flow = new Flow(
                    "FLOW01_02",
                    "BANCO02",
                    "Flujo 1",
                    "Descripción Flujo 1",
                    product.ProductId
            );
            return flow;
        }

        private FlowStepInput FlowStepInput(Guid flowId, Guid stepId)
        {
            return new FlowStepInput()
            {
                FlowId = flowId,
                EndPointService = "",
                Order = 1,
                QueueService = "",
                StepId = stepId
            };
        }


        #endregion    
    }
}
