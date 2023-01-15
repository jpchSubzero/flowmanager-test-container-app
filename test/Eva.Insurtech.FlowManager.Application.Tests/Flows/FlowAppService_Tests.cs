using Eva.Framework.Utility.Option.Contracts;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.Flows;
using Eva.Insurtech.Flows.MockHttpMessageHandler;
using Eva.Insurtech.TrackingManagers.Trackings;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Xunit;

namespace Eva.Insurtech.Flow.Flows
{
    [ExcludeFromCodeCoverage]
    public class FlowAppService_Tests : FlowManagerApplicationTestBase
    {
        private readonly IFlowAppService _flowAppService;
        private readonly CatalogManager _catalogManager;
        private readonly Eva.Insurtech.FlowManagers.Flows.FlowManager _flowManager;
        private readonly IFlowRepository _flowRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IAppConfigurationManager _appConfigurationManager;
        private readonly IObjectMapper _objectMapper;
        private readonly TrackingManager _trackingManager;
        private readonly ILogger<FlowAppService> _logger;

        public FlowAppService_Tests()
        {
            _flowAppService = GetRequiredService<IFlowAppService>();
            _catalogManager = GetRequiredService<CatalogManager>();
            _flowManager = GetRequiredService<Eva.Insurtech.FlowManagers.Flows.FlowManager>();
            _flowRepository = GetRequiredService<IFlowRepository>();
            _productRepository = GetRequiredService<IProductRepository>();
            _catalogRepository = GetRequiredService<ICatalogRepository>();
            _appConfigurationManager = GetRequiredService<IAppConfigurationManager>();
            _objectMapper = GetRequiredService<IObjectMapper>();
            _logger = GetRequiredService<ILogger<FlowAppService>>();
            _trackingManager = GetRequiredService<TrackingManager>();
        }

        [Fact]
        public async Task GetFlow_ById_ReturnResponseSuccess()
        {
            var flowExpected = await _flowAppService.GetByCodeAsync("FLOW01");
            var flow = await _flowAppService.GetAsync(flowExpected.Result.Id);

            Assert.Equal(flow.Result.Id, flowExpected.Result.Id);
            Assert.Equal(flow.Result.Name, flowExpected.Result.Name);
            Assert.Equal(flow.Result.Description, flowExpected.Result.Description);
        }

        [Fact]
        public async Task GetFlow_ById_ReturnNotFoundById()
        {
            var result = await _flowAppService.GetAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task GetFlow_ByCode_ReturnResponseSuccess()
        {
            var flows = await _flowAppService.GetListAsync();
            var flow = await _flowAppService.GetByCodeAsync(flows.Result.LastOrDefault().Code);

            Assert.Equal(flow.Result.Id, flows.Result.LastOrDefault().Id);
            Assert.Equal(flow.Result.Name, flows.Result.LastOrDefault().Name);
            Assert.Equal(flow.Result.Description, flows.Result.LastOrDefault().Description);
        }

        [Fact]
        public async Task GetFlow_ByCode_ReturnNotFoundByCode()
        {
            var result = await _flowAppService.GetByCodeAsync("NON_EXISTS");

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_CODE));
        }

        [Fact]
        public async Task CreateFlow_WithData_ReturnResponseSuccess()
        {
            CreateFlowDto input = await GetCreateNew();

            Mock<HttpMessageHandler> handlerMock = GetChannelMock();
            FlowAppService flowAppService = GetCustomFlowAppService(handlerMock);
            var expectedUri = new Uri($"https://des-api-eva.novaecuador.com/product/api/channel/{input.ChannelCode}/by-code");

            var result = await flowAppService.InsertAsync(input);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get && req.RequestUri == expectedUri
               ),
               ItExpr.IsAny<CancellationToken>()
            );

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.Code, input.Code);
            Assert.Equal(result.Result.Name, input.Name);
            Assert.Equal(result.Result.Description, input.Description);
        }

        [Fact]
        public async Task CreateFlow_WithExistsFlow_ReturnResponseSuccess()
        {
            CreateFlowDto input = await GetCreateExistent();
            var result = await _flowAppService.InsertAsync(input);
            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_ALREADY_EXIST_CODE));
        }

        [Fact]
        public async Task CreateFlow_WithNonExistsProduct_ReturnNotFoundById()
        {
            CreateFlowDto input = await GetCreateNew();
            input.ProductId = Guid.NewGuid();

            Mock<HttpMessageHandler> handlerMock = GetProductMock();
            FlowAppService flowAppService = GetCustomFlowAppService(handlerMock);
            var expectedUri = new Uri($"https://des-api-eva.novaecuador.com/product/api/product/{input.ProductId}");

            var result = await flowAppService.InsertAsync(input);

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
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
        public async Task UpdateFlow_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowRepository.GetListAsync();

            var flowDto = await GetUpdate();

            var result = await _flowAppService.UpdateAsync(flowDto, flow.FirstOrDefault().Id);
            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.Name, flowDto.Name);
            Assert.Equal(result.Result.Description, flowDto.Description);
        }

        [Fact]
        public async Task UpdateFlow_WithNonExistsFlow_ReturnNotFoundById()
        {
            var flowDto = await GetUpdate();

            var result = await _flowAppService.UpdateAsync(flowDto, Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task DeleteFlow_WithData_ReturnResponseSuccess()
        {
            var flow = await _flowRepository.GetListAsync(false);

            var result = await _flowAppService.DeleteAsync(flow.FirstOrDefault().Id);
            Assert.True(result.Result);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task DeleteFlow_WithNonExistsFlow_ReturnNotFoundById()
        {
            var result = await _flowAppService.DeleteAsync(Guid.NewGuid());
            Assert.False(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));

        }

        [Fact]
        public async Task AddFlowStep_WithData_ReturnSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var catalog = await _catalogManager.GetByCodeAsync("CATALOG02", TrackingConsts.FLOW_STEPS);

            var input = await GetFlowStepDto(catalog.CatalogId);
            var result = await _flowAppService.AddFlowStep(input, flow.Id);

            Assert.NotNull(result.Result);
            Assert.True(result.Success);
            Assert.Equal(result.Result.FlowSteps.FirstOrDefault().FlowId, input.FlowId);
            Assert.Equal(result.Result.FlowSteps.FirstOrDefault().StepId, input.StepId);
            Assert.Equal(result.Result.FlowSteps.FirstOrDefault().QueueService, input.QueueService);
            Assert.Equal(result.Result.FlowSteps.FirstOrDefault().EndPointService, input.EndPointService);
        }

        [Fact]
        public async Task AddFlowStep_WithNonExistsFlow_ReturnNotFoundById()
        {
            var catalog = await _catalogManager.GetByCodeAsync("CATALOG01", TrackingConsts.FLOW_STEPS);
            var input = await GetFlowStepDto(catalog.CatalogId);

            var result = await _flowAppService.AddFlowStep(input, Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task GetFlowByProductId_WithExistsProduct_ReturnFlows()
        {
            var code = "FLOW01";

            var flow = await _flowManager.GetByCodeAsync(code);

            var result = await _flowAppService.GetByProductId(flow.ProductId);

            Assert.NotNull(result);
            Assert.True(result.Result.Any());
            Assert.NotNull(result.Result.FirstOrDefault(x => x.Code.Equals(code)));
        }

        [Fact]
        public async Task GetFlowByProductId_WithNonExistsProduct_ReturnEmpty()
        {
            var result = await _flowAppService.GetByProductId(Guid.NewGuid());

            Assert.NotNull(result);
            Assert.False(result.Result.Any());
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithExistsProductAndChannel_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowAppService.GetByProductAndChannel(flow.ProductId, flow.ChannelCode);

            Assert.NotNull(result);
            Assert.Equal(result.Result.Code, flow.Code);
            Assert.Equal(result.Result.ChannelCode, flow.ChannelCode);
            Assert.Equal(result.Result.Name, flow.Name);
            Assert.Equal(result.Result.Description, flow.Description);
            Assert.Equal(result.Result.ProductId, flow.ProductId);
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithNonExistsProduct_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowAppService.GetByProductAndChannel(Guid.NewGuid(), flow.ChannelCode);

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }

        [Fact]
        public async Task GetFlowByProductIdAndChannelCode_WithNonExistsChannel_ReturnFlow()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var result = await _flowAppService.GetByProductAndChannel(flow.ProductId, "NON_EXISTS");

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_CHANNEL_NOT_FOUND_BY_CODE));
        }

        [Fact]
        public async Task GetFlow_ByTrackingId_ReturnResponseSuccess()
        {
            var flow = await _flowManager.GetByCodeAsync("FLOW01");

            var trackingId = await CreateTracking(flow.Id);
            var result = await _flowAppService.GetByTrackingAsync(trackingId);

            Assert.Equal(flow.Id, result.Result.Id);
            Assert.Equal(flow.Name, result.Result.Name);
            Assert.Equal(flow.Description, result.Result.Description);
        }

        [Fact]
        public async Task GetFlow_ByTrackingId_ReturnNotFoundById()
        {
            var result = await _flowAppService.GetByTrackingAsync(Guid.NewGuid());

            Assert.Null(result.Result);
            Assert.False(result.Success);
            Assert.True(result.Error.Code.Equals(ErrorConsts.ERROR_NOT_FOUND_BY_ID));
        }





        #region Private Methods

        private async Task<CreateFlowDto> GetCreateNew()
        {
            var product = await _productRepository.GetByCodeAsync("PRODUCTO02");

            var input = new CreateFlowDto()
            {
                Code = "FLOW140",
                ChannelCode = "BANCO01",
                Name = "Flujo 140",
                Description = "Descripción Flujo 140",
                ProductId = product.ProductId
            };
            return input;
        }

        private async Task<CreateFlowDto> GetCreateExistent()
        {
            var product = await _productRepository.GetByCodeAsync("PRODUCTO02");

            var input = new CreateFlowDto()
            {
                Code = "FLOW01",
                ChannelCode = "BANCO01",
                Name = "Flujo 1",
                Description = "Descripción Flujo 1",
                ProductId = product.ProductId
            };
            return input;
        }

        private async Task<UpdateFlowDto> GetUpdate()
        {
            var product = await _productRepository.GetByCodeAsync("PRODUCTO02");

            var input = new UpdateFlowDto()
            {
                Code = "FLOW01_01",
                ChannelCode = "BANCO02",
                Name = "Flujo 1",
                Description = "Descripción Flujo 1",
                ProductId = product.ProductId
            };

            return input;
        }

        private static async Task<CreateFlowStepDto> GetFlowStepDto(Guid stepId)
        {
            var input = new CreateFlowStepDto()
            {
                EndPointService = "",
                Order = 1,
                QueueService = "",
                StepId = stepId
            };
            return await Task.FromResult(input);
        }

        private static Mock<HttpMessageHandler> GetProductMock()
        {
            return FakeHttpClient.GetMockHttpMessageHandler(@"{
'$id': '1',
'success': true,
'result': {
'$id': '2',
'code': 'PROD0011',
'externalCode': 'EXT_PROD0011',
'name': 'Seguro sin límites 1111111',
'description': 'No importa si decides contratar este seguro por un mes o por un año, todos los meses pagarás un único valor mensualizado.',
'issuesPolicyMother': true,
'insuranceTypeId': '460cc702-619b-4ed9-b7da-629000b9d412',
'insuranceCarrierId': '341232dc-e1b7-4d13-b62f-d66b4fa3986c',
'contractTypeId': '92b081c7-ce5e-4c60-a14b-cf0fff125567',
'isActive': true,
'linkTC': 'string',
'exclusions': {
'$id': '3',
'$values': [
{
'$id': '4',
'code': 'EXCL001',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Tu auto tiene más de 15 años de fabricación.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 1,
'id': '9b69981b-6e06-4481-5fb3-08d9be92194e'
},
{
'$id': '5',
'code': 'EXCL002',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'El siniestro tiene un valor menor al deducible mínimo. Ejm: Si tu auto está asegurado por $20.000 y tu siniestro tuvo un costo de $150, la aseguradora no cubre el accidente porque el valor mínimo del deducible es $200. Consulta este valor en la sección de \\\'Deducibles\\\'.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 2,
'id': 'eaccef60-c5e9-4bfc-5fb4-08d9be92194e'
},
{
'$id': '6',
'code': 'EXCL003',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Tu auto tiene una valor comercial menor a $10.000.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 3,
'id': 'a5c1c681-b4c5-431b-5fb5-08d9be92194e'
},
{
'$id': '7',
'code': 'EXCL004',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'El conductor y asegurado es menor a 18 años o mayor a 70 años.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 4,
'id': '65ab4ce5-b62b-4c8f-5fb6-08d9be92194e'
},
{
'$id': '8',
'code': 'EXCL005',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Usas tu auto con un fin comercial, como alquiler o taxi.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 1,
'id': '14b2530e-635b-48c4-c2d4-08d9bf2542c1'
},
{
'$id': '9',
'code': 'EXCL006',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Transitas con tu auto por playas, ríos, esteros o realizas actividades 4x4.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 6,
'id': 'bf0fcfb1-ed8f-4703-c2d5-08d9bf2542c1'
},
{
'$id': '10',
'code': 'EXCL007',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Realizas reparaciones o cualquier cambio de pieza de tu vehículo, antes de recibir la autorización de la aseguradora.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 7,
'id': '0ddad56e-0964-48ee-c2d6-08d9bf2542c1'
},
{
'$id': '11',
'code': 'EXCL008',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Tu auto sufre cualquier daño ocasionado por el uso inapropiado o alteración del combustible.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 8,
'id': '3fe130cf-724a-4aab-c2d7-08d9bf2542c1'
},
{
'$id': '12',
'code': 'EXCL009',
'name': 'Sin nombre',
'isActive': true,
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'La información sobre el siniestro es falsa, exagerada o llegas a un acuerdo con terceros antes de recibir la autorización de la aseguradora.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'coverageId': '00000000-0000-0000-0000-000000000000',
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb',
'priority': 9,
'id': '3e46d5c6-cd00-4d21-c2d8-08d9bf2542c1'
}
]
},
'benefits': {
'$id': '13',
'$values': [ ]
},
'deductibles': {
'$id': '14',
'$values': [
{
'$id': '15',
'code': 'DEDU001',
'name': 'Pérdida parcial del vehículo por choque',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida parcial, debes asumir el VALOR MÁS ALTO de las siguientes tres opciones:\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ol\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'1% del valor asegurado del vehículo.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'$200\',\'Attr\':{},\'Children\':[]}]}]}\r\n',
'isActive': true,
'priority': 1,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '16',
'code': 'DEDU002',
'name': 'Pérdida total del vehículo por choque',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total, debes asumir el 10% del valor del siniestro. \',\'Attr\':{},\'Children\':[]}]}\r\n',
'isActive': true,
'priority': 2,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '17',
'code': 'DEDU003',
'name': 'Pérdida total del vehículo por robo CON dispositivo satelital',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total y cuentas con dispositivo satelital, debes asumir el 5% del valor del siniestro. \',\'Attr\':{},\'Children\':[]}]}\r\n',
'isActive': true,
'priority': 3,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '18',
'code': 'DEDU004',
'name': 'Pérdida total del vehículo por robo SIN dispositivo satelital',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si el valor asegurado de tu vehículo es MAYOR a $30.000, debes asumir el 20% del valor del siniestro. Si el valor de tu vehículo es MENOR $30.000, debes asumir el 10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'isActive': true,
'priority': 4,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
}
]
},
'coverages': {
'$id': '19',
'$values': [
{
'$id': '20',
'code': 'COBE001',
'name': 'Protección total',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Tu auto está totalmente cubierto ante cualquier daño parcial o total, ya sea por accidente o robo. Además también cuentas con esta protección en toda la Comunidad Andina, es decir en Bolivia, Colombia y Perú.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 1,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '21',
'code': 'COBE002',
'name': 'Accidentes Personales a Ocupantes del vehículo',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si a causa de un accidente, tú y/o tus acompañantes quedan incapacitados o fallecen, la aseguradora pagará hasta $5.000 por cada afectado. Aplica para máximo 5 ocupantes.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 2,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '22',
'code': 'COBE003',
'name': 'Muerte accidental del titular',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si un accidente ocasiona el fallecimiento del titular del seguro, los herederos legales reciben $10.000.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 3,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '23',
'code': 'COBE004',
'name': 'Lesiones a Ocupantes del vehículo',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si a causa de un accidente, tú y/o tus acompañantes sufren alguna lesión, la aseguradora pagará hasta $3.000 por cada afectado. Aplica para máximo 5 ocupantes.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 4,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '24',
'code': 'COBE005',
'name': 'Gastos de Grúa y Transporte del vehículo',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si a causa de un accidente, tú y/o tus acompañantes sufren alguna lesión, la aseguradora pagará hasta $3.000 por cada afectado. Aplica para máximo 5 ocupantes.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 5,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '25',
'code': 'COBE006',
'name': 'Protección jurídica en proceso penal',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si sufres un accidente o cualquier avería, cuentas con un monto de hasta $250 adicionales al monto que cubre la \\\'Asistencia vehicular 24/7\\\', para movilizar tu vehículo. Consulta este monto en la sección de \\\'Asistencias\\\'.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 6,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '26',
'code': 'COBE007',
'name': 'Responsabilidad Civil',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Cuentas con $1.000 para gastos de procesos penales en caso de accidente. Este valor se devuelve por reembolso.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 7,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
{
'$id': '27',
'code': 'COBE008',
'name': 'Amparo Patrimonial',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Estás cubierto si causas un accidente tipificado como culpa grave (por desatender las señales o normas de tránsito, carecer de licencia, tener la licencia caducada o estar bajo efectos de bebidas embriagantes).\',\'Attr\':{},\'Children\':[]}]}\r\n',
'limitValueMin': 0.00,
'limitValueMax': 0.00,
'limitEventMin': 0,
'limitEventMax': 0,
'priority': 8,
'isActive': true,
'productId': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
}
]
},
'requirements': {
'$id': '28',
'$values': [ ]
},
'periodicityPayments': {
'$id': '29',
'$values': [ ]
},
'paymentModes': {
'$id': '30',
'$values': [ ]
},
'assistances': {
'$id': '31',
'$values': [
{
'$id': '32',
'name': 'Auto sustituto',
'description': ' {\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Recibe un auto sustituto por 10 días en pérdidas parciales y 20 días en pérdidas totales, cuando el valor del siniestro sea mayor a $1.000 y la reparación de tu auto supere los 3 días. La gama del auto sustituto dependerá del valor asegurado de tu vehículo:\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ul\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Vehículos con un valor asegurado de $0 a $20.000: Gama baja.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Vehículos con un valor asegurado de $20.001 a $40.000: Gama media.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Vehículos con un valor asegurado de $40.001 en adelante: Gama alta.\',\'Attr\':{},\'Children\':[]}]}]}\r\n',
'code': 'ASIS001',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 1,
'products': {
'$id': '33',
'$values': [
{
'$ref': '2'
}
]
},
'id': 'c81a92cf-cf25-3532-84b8-3a00c8506d1a'
},
{
'$id': '34',
'name': 'Asistencia vehicular 24/7',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Cuentas con auxilio mecánico para averías leves como llantas bajas, falta de combustible, batería baja, llaves dentro del auto y traslado de ambulancia. Recibe hasta $300 para gastos de grúa y transporte del vehículo.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'code': 'ASIS002',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 2,
'products': {
'$id': '35',
'$values': [
{
'$ref': '2'
}
]
},
'id': '39f867b1-aed6-b1d6-771e-3a00c850db1a'
},
{
'$id': '36',
'name': 'Ángel guardián',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Cuentas con un chofer para regresar a tu domicilio desde eventos o reuniones sociales. Límite de movilización: 50 kilómetros o 2 horas de distancia. Aplica para máximo 5 eventos al año.\',\'Attr\':{},\'Children\':[]}]}',
'code': 'ASIS003',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 3,
'products': {
'$id': '37',
'$values': [
{
'$ref': '2'
}
]
},
'id': '85a9e4c1-27c0-3658-5530-3a00c8521159'
},
{
'$id': '38',
'name': 'Documentos protegidos',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Recibe hasta $100 en caso de pérdida o robo de tus documentos (cédula, matrícula o licencia), sin importar el lugar donde suceda el incidente. Aplica para 1 evento al año.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'code': 'ASIS004',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 4,
'products': {
'$id': '39',
'$values': [
{
'$ref': '2'
}
]
},
'id': 'f99d0cbf-f260-c5bc-ed7b-3a00cc39e42b'
},
{
'$id': '40',
'name': 'Llave Protegida',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Recibe hasta $150 para reemplazar tu llave, reparar la cerradura de la puerta principal o requerir un cerrajero para abrir la puerta de tu vehículo. Aplica para 1 evento al año.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'code': 'ASIS005',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 5,
'products': {
'$id': '41',
'$values': [
{
'$ref': '2'
}
]
},
'id': '1912b9c7-ca8d-8924-b371-3a00cc3a447d'
},
{
'$id': '42',
'name': 'Asistencia legal',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Cuentas con asesoría legal de manera telefónica o presencial en caso de sufrir un accidente. La asistencia presencial aplica para Quito, Guayaquil, Cuenca y Ambato. \',\'Attr\':{},\'Children\':[]}]}\r\n',
'code': 'ASIS006',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 6,
'products': {
'$id': '43',
'$values': [
{
'$ref': '2'
}
]
},
'id': '901096c1-37f3-ed16-10ad-3a00cc3a98b3'
},
{
'$id': '44',
'name': 'Asistencia para emergencias en el hogar',
'description': '{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Cuentas con servicios de cerrajería, vidriería, electricidad y plomería para reparar cualquier eventualidad en tu casa.\',\'Attr\':{},\'Children\':[]}]}\r\n',
'code': 'ASIS007',
'isActive': true,
'minEvents': 0,
'maxEvents': 0,
'priority': 7,
'products': {
'$id': '45',
'$values': [
{
'$ref': '2'
}
]
},
'id': '5a29653e-0510-07a4-6d14-3a00cc3aff94'
}
]
},
'channels': {
'$id': '46',
'$values': [
{
'$id': '47',
'code': 'BANCO01',
'name': 'Banco del Pichincha',
'description': 'Descripción Banco del Pichincha',
'isActive': true,
'countryId': '92868ba1-76fb-4b34-8751-ddd874ee8bba',
'currencyId': '2cfd94c4-3e53-47f8-9f71-6d74099779e7',
'channelsWays': {
'$id': '48',
'$values': [ ]
},
'channelContacts': {
'$id': '49',
'$values': [ ]
},
'products': {
'$id': '50',
'$values': [
{
'$ref': '2'
}
]
},
'id': '73b6ac58-aa85-42b2-bf74-1583999ae14e'
},
{
'$id': '51',
'code': 'BANCO02',
'name': 'Banco General Rumiñahui',
'description': 'Descripción Banco General Rumiñahui',
'isActive': true,
'countryId': '92868ba1-76fb-4b34-8751-ddd874ee8bba',
'currencyId': '2cfd94c4-3e53-47f8-9f71-6d74099779e7',
'channelsWays': {
'$id': '52',
'$values': [ ]
},
'channelContacts': {
'$id': '53',
'$values': [ ]
},
'products': {
'$id': '54',
'$values': [
{
'$ref': '2'
}
]
},
'id': 'b7e1322f-16ea-4876-887c-8370eea43df2'
}
]
},
'id': '4683f67e-b616-ecfe-0387-3a00bda4bcfb'
},
'error': null,
'targetUrl': '',
'unAuthorizedRequest': true
}");
        }

        private static Mock<HttpMessageHandler> GetChannelMock()
        {
            return FakeHttpClient.GetMockHttpMessageHandler(@"
                {
                '$id': '1',
                'success': true,
                'result': {
                '$id': '2',
                'code': 'BANCO01',
                'name': 'Banco del Pichincha',
                'description': 'Banco del Pichincha',
                'businessIdentification': '1790010937001',
                'businessIdentificationType': 'BUSINESS_TYPE_RUC',
                'businessName': 'BANCO PICHINCHA C.A.',
                'isActive': true,
                'isDeleted': false,
                'countryId': '82592564-40d6-e6bf-2d42-3a017bffe69c',
                'currencyId': '1457659e-69a3-5364-d18a-3a017bffdf46',
                'channelsWays': {
                '$id': '3',
                '$values': [
                {
                '$id': '4',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'wayId': 'ba77467c-7b76-0e0e-70da-3a017c029bc0',
                'isActive': true
                },
                {
                '$id': '5',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'wayId': '4f42c4e8-eed3-f996-2f47-3a017c029e72',
                'isActive': true
                }
                ]
                },
                'channelContacts': {
                '$id': '6',
                '$values': [
                {
                '$id': '7',
                'label': 'Phone',
                'value': '022999999',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'isActive': true
                }
                ]
                },
                'products': {
                '$id': '8',
                '$values': [
                {
                '$id': '9',
                'code': 'PROD002',
                'externalCode': 'EXT_PROD002',
                'name': 'Seguro por kilómetros',
                'description': 'Contrata el plan que mejor se ajuste a los kilómetros que recorres, manteniendo las mejores coberturas y beneficios.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '567d45c2-c812-85f9-7a2b-3a024fe72ed7',
                'insuranceCarrierId': '377c3f28-531b-2d26-b71b-3a024fe72091',
                'contractTypeId': '209439ed-5442-4203-744d-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': '',
                'exclusions': {
                '$id': '10',
                '$values': [ ]
                },
                'benefits': {
                '$id': '11',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '12',
                '$values': [
                {
                '$id': '13',
                'code': 'DEDU007',
                'channelId': '955e6096-db07-3758-65b2-3a024fe742ea',
                'name': 'Pérdida total del vehículo por choque o robo',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total, debes asumir el 15% del valor del siniestro.\',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 2,
                'productId': '86d07944-f27b-5655-f21d-3a024fe77762'
                },
                {
                '$id': '14',
                'code': 'DEDU008',
                'channelId': '955e6096-db07-3758-65b2-3a024fe742ea',
                'name': 'Pérdida parcial del vehículo por choque',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida parcial, debes asumir el VALOR MÁS ALTO de las siguientes tres opciones: \',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ol\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\' 1% del valor asegurado del vehículo.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Deducible mínimo, dependiendo del valor asegurado de tu vehículo. Revisa la Tabla de deducibles.\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'h4\',\'Text\':\'Tabla de deducibles\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'table\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tbody\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Valor Asegurado del Vehículo\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Deducible mínimo\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Menor o igual a $15,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$150\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $15,001 y $20,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$200\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $20,001 y $25,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$250\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $25,001 y $35,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$300\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $35,001 y $50,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$350\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $50,001 y $120,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$500\',\'Attr\':{},\'Children\':[]}]}]}]}]}]',
                'isActive': true,
                'priority': 1,
                'productId': '86d07944-f27b-5655-f21d-3a024fe77762'
                },
                {
                '$id': '15',
                'code': 'DEDU005',
                'channelId': '74b5f785-978c-9a45-83e5-3a024fe749de',
                'name': 'Pérdida total del vehículo por choque o robo',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total, debes asumir el 15% del valor del siniestro.  \',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 5,
                'productId': '86d07944-f27b-5655-f21d-3a024fe77762'
                },
                {
                '$id': '16',
                'code': 'DEDU006',
                'channelId': '74b5f785-978c-9a45-83e5-3a024fe749de',
                'name': 'Pérdida parcial del vehículo por choque',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida parcial, debes asumir el VALOR MÁS ALTO de las siguientes tres opciones:\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ol\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'1% del valor asegurado del vehículo.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Deducible mínimo, dependiendo del valor asegurado de tu vehículo. Revisa la siguiente tabla.\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'h4\',\'Text\':\'Tabla de Responsabilidad Civil\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'table\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tbody\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Valor Asegurado del Vehículo\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Límite Responsabilidad Civil\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Menor o igual a $15,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$150\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $15,001 y $20,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$200\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $20,001 y $25,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$250\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $25,001 y $35,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$300\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $35,001 y $50,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$350\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $50,001 y $120,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$500\',\'Attr\':{},\'Children\':[]}]}]}]}]}]',
                'isActive': true,
                'priority': 6,
                'productId': '86d07944-f27b-5655-f21d-3a024fe77762'
                }
                ]
                },
                'coverages': {
                '$id': '17',
                '$values': [ ]
                },
                'requirements': {
                '$id': '18',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '19',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '20',
                '$values': [ ]
                },
                'assistances': {
                '$id': '21',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '22',
                '$values': [ ]
                },
                'id': '86d07944-f27b-5655-f21d-3a024fe77762'
                },
                {
                '$id': '23',
                'code': 'PROD003',
                'externalCode': 'EXT_PROD003',
                'name': 'Protección total Fraudes',
                'description': 'Con el seguro de fraudes puedes recuperar tu dinero que haya sido usado de tus tarjetas sin tu autorización.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '989b0044-4bb1-ec07-87e1-3a024fe7301f',
                'insuranceCarrierId': '99a57596-d7eb-b718-d1c8-3a024fe72205',
                'contractTypeId': 'f102f7ab-135c-480d-744e-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': '',
                'exclusions': {
                '$id': '24',
                '$values': [ ]
                },
                'benefits': {
                '$id': '25',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '26',
                '$values': [ ]
                },
                'coverages': {
                '$id': '27',
                '$values': [ ]
                },
                'requirements': {
                '$id': '28',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '29',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '30',
                '$values': [ ]
                },
                'assistances': {
                '$id': '31',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '32',
                '$values': [ ]
                },
                'id': 'cf9f161e-2db7-a10d-59a2-3a024fe77a68'
                },
                {
                '$id': '33',
                'code': 'PROD004',
                'externalCode': 'EXT_PROD004',
                'name': 'Mas vida segura Plus',
                'description': 'Proge a tu familia, incluso si tu no estás con ellos.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '720a93b8-037c-dfec-689d-3a024fe72d6b',
                'insuranceCarrierId': '99a57596-d7eb-b718-d1c8-3a024fe72205',
                'contractTypeId': 'f102f7ab-135c-480d-744e-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': '',
                'exclusions': {
                '$id': '34',
                '$values': [ ]
                },
                'benefits': {
                '$id': '35',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '36',
                '$values': [ ]
                },
                'coverages': {
                '$id': '37',
                '$values': [ ]
                },
                'requirements': {
                '$id': '38',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '39',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '40',
                '$values': [ ]
                },
                'assistances': {
                '$id': '41',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '42',
                '$values': [ ]
                },
                'id': 'eb572ed9-b4b3-80ba-5145-3a024fe77d7f'
                },
                {
                '$id': '43',
                'code': 'PROD005',
                'externalCode': 'EXT_PROD005',
                'name': 'Plan deuda protegida',
                'description': 'Protege a tus familiares de pagar tus deudas en caso de muerte o incapacidad.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '989b0044-4bb1-ec07-87e1-3a024fe7301f',
                'insuranceCarrierId': '99a57596-d7eb-b718-d1c8-3a024fe72205',
                'contractTypeId': 'f102f7ab-135c-480d-744e-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': 'https://desstorageaccountventa.blob.core.windows.net/bpichinchadocs/SDP/CERTIFICADO%20DE%20SEGURO%20%20PLAN%20DEUDA%20PROTEGIDA%202022.pdf?sp=r&st=2022-05-06T15:27:45Z&se=2022-07-07T04:27:45Z&spr=https&sv=2020-08-04&sr=c&sig=VLfv8DOvl1jji28XDmX6Uc8MUXtIbJ3cBepyfjawzXA%3D',
                'exclusions': {
                '$id': '44',
                '$values': [ ]
                },
                'benefits': {
                '$id': '45',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '46',
                '$values': [ ]
                },
                'coverages': {
                '$id': '47',
                '$values': [ ]
                },
                'requirements': {
                '$id': '48',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '49',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '50',
                '$values': [ ]
                },
                'assistances': {
                '$id': '51',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '52',
                '$values': [ ]
                },
                'id': 'cc191487-40c9-d45a-9844-3a024fe7812c'
                },
                {
                '$id': '53',
                'code': 'PROD009',
                'externalCode': 'EXT_PROD009',
                'name': 'Seguro Vehicular Total',
                'description': 'No importa si decides contratar este seguro por un mes o por un año, todos los meses pagarás un único valor mensualizado.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '567d45c2-c812-85f9-7a2b-3a024fe72ed7',
                'insuranceCarrierId': '377c3f28-531b-2d26-b71b-3a024fe72091',
                'contractTypeId': 'f102f7ab-135c-480d-744e-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': '',
                'exclusions': {
                '$id': '54',
                '$values': [ ]
                },
                'benefits': {
                '$id': '55',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '56',
                '$values': [
                {
                '$id': '57',
                'code': 'DEDU009',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida parcial del vehículo por choque',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida parcial, debes asumir el VALOR MÁS ALTO de las siguientes tres opciones:\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ol\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'1% del valor asegurado del vehículo.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'El valor de $200\',\'Attr\':{},\'Children\':[]}]}]}]',
                'isActive': true,
                'priority': 1,
                'productId': '591acafb-dd84-b8df-3921-3a02c96470dc'
                },
                {
                '$id': '58',
                'code': 'DEDU010',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida total del vehículo por choque',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total, debes asumir el 10% del valor del siniestro. \',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 2,
                'productId': '591acafb-dd84-b8df-3921-3a02c96470dc'
                },
                {
                '$id': '59',
                'code': 'DEDU011',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida total del vehículo por robo CON dispositivo satelital',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total y cuentas con dispositivo satelital, debes asumir el 5% del valor del siniestro. \',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 3,
                'productId': '591acafb-dd84-b8df-3921-3a02c96470dc'
                },
                {
                '$id': '60',
                'code': 'DEDU012',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida total del vehículo por robo SIN dispositivo satelital',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si el valor asegurado de tu vehículo es MAYOR o IGUAL a $30,000, debes asumir el 20% del valor del siniestro. Si el valor de tu vehículo es MENOR a $30,000, debes asumir el 10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 4,
                'productId': '591acafb-dd84-b8df-3921-3a02c96470dc'
                }
                ]
                },
                'coverages': {
                '$id': '61',
                '$values': [ ]
                },
                'requirements': {
                '$id': '62',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '63',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '64',
                '$values': [ ]
                },
                'assistances': {
                '$id': '65',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '66',
                '$values': [ ]
                },
                'id': '591acafb-dd84-b8df-3921-3a02c96470dc'
                },
                {
                '$id': '67',
                'code': 'PROD010',
                'externalCode': 'EXT_PROD010',
                'name': 'Seguro Vehicular Kilómetros',
                'description': 'Ahorra en tu seguro y contrata el plan que mejor se ajuste a los kilómetros que recorres.',
                'issuesPolicyMother': true,
                'insuranceTypeId': '567d45c2-c812-85f9-7a2b-3a024fe72ed7',
                'insuranceCarrierId': '377c3f28-531b-2d26-b71b-3a024fe72091',
                'contractTypeId': 'f102f7ab-135c-480d-744e-08d9d9fbbcde',
                'isActive': true,
                'isDeleted': false,
                'linkTC': '',
                'exclusions': {
                '$id': '68',
                '$values': [ ]
                },
                'benefits': {
                '$id': '69',
                '$values': [ ]
                },
                'deductibles': {
                '$id': '70',
                '$values': [
                {
                '$id': '71',
                'code': 'DEDU013',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida total del vehículo por choque o robo',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida total, debes asumir el 15% del valor del siniestro.  \',\'Attr\':{},\'Children\':[]}]}]',
                'isActive': true,
                'priority': 5,
                'productId': 'e95f2f59-a874-295c-37d6-3a02c9647375'
                },
                {
                '$id': '72',
                'code': 'DEDU014',
                'channelId': '8f48659f-0123-1eb7-3709-3a024fe737e1',
                'name': 'Pérdida parcial del vehículo por choque',
                'description': '[{\'NodeType\':\'Element\',\'Tag\':\'body\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'p\',\'Text\':\'Si tu accidente es catalogado como pérdida parcial, debes asumir el VALOR MÁS ALTO de las siguientes tres opciones: \',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'ol\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'10% del valor del siniestro.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'1% del valor asegurado del vehículo.\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'li\',\'Text\':\'Deducible mínimo, dependiendo del valor asegurado de tu vehículo. Revisa la Tabla de deducibles.\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'h4\',\'Text\':\'Tabla de Deducibles\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'table\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tbody\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Valor Asegurado del Vehículo\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Deducible Mínimo\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Menor o igual a $15,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$150\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $15,001 y $20,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$200\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $20,001 y $25,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$250\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $25,001 y $35,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$300\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $35,001 y $50,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$350\',\'Attr\':{},\'Children\':[]}]},{\'NodeType\':\'Element\',\'Tag\':\'tr\',\'Text\':null,\'Attr\':{},\'Children\':[{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'Entre $50,001 y $120,000\',\'Attr\':{},\'Children\':[]},{\'NodeType\':\'Element\',\'Tag\':\'td\',\'Text\':\'$500\',\'Attr\':{},\'Children\':[]}]}]}]}]}]',
                'isActive': true,
                'priority': 6,
                'productId': 'e95f2f59-a874-295c-37d6-3a02c9647375'
                }
                ]
                },
                'coverages': {
                '$id': '73',
                '$values': [ ]
                },
                'requirements': {
                '$id': '74',
                '$values': [ ]
                },
                'periodicityPayments': {
                '$id': '75',
                '$values': [ ]
                },
                'paymentModes': {
                '$id': '76',
                '$values': [ ]
                },
                'assistances': {
                '$id': '77',
                '$values': [ ]
                },
                'productDescriptions': {
                '$id': '78',
                '$values': [ ]
                },
                'id': 'e95f2f59-a874-295c-37d6-3a02c9647375'
                }
                ]
                },
                'id': '8f48659f-0123-1eb7-3709-3a024fe737e1'
                },
                'error': null,
                'targetUrl': '',
                'unAuthorizedRequest': true
                }
            ");
        }

        private FlowAppService GetCustomFlowAppService(Mock<HttpMessageHandler> handlerMock)
        {
            var httpClient = new HttpClient(handlerMock.Object);
            var apiServiceManager = new ApiServiceManager(httpClient);
            var productManager = new ProductManager(_productRepository, apiServiceManager, _appConfigurationManager);
            var flowAppService = new FlowAppService(_flowManager, productManager, _catalogManager ,_trackingManager, _objectMapper, _logger);
            return flowAppService;
        }

        private async Task<Guid> CreateTracking(Guid flowId)
        {
            var catalog1 = await _catalogRepository.GetByCodeAsync("CATALOG01");
            var catalog2 = await _catalogRepository.GetByCodeAsync("CATALOG02");
            var catalog3 = await _catalogRepository.GetByCodeAsync("CATALOG03");
            
            await _catalogRepository.GetByExternalIdAsync(catalog1.CatalogId);

            var tracking = new Tracking(
                flowId,
                catalog1.CatalogId,
                catalog2.CatalogId,
                catalog3.CatalogId,
                "BANCO01",
                "BANCAMOVIL",
                "192.168.0.1"
            );

            var result = await _trackingManager.InsertAsync(tracking);
            return result.Id;
        }


        #endregion
    }

}








