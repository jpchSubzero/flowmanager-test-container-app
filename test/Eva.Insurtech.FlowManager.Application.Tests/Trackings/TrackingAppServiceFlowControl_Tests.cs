using Eva.Framework.Utility.Option.Contracts;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.ApiServices;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.FlowSteps;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using Eva.Insurtech.Flows.MockHttpMessageHandler;
using Eva.Insurtech.TrackingManagers.Trackings;
using Eva.Insurtech.Trackings;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Shouldly;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using Xunit;
using Xunit.Abstractions;

namespace Eva.Insurtech.Product.Trackings
{
    [ExcludeFromCodeCoverage]
    public class TrackingAppServiceFlowControl_Tests : FlowManagerApplicationTestBase
    {
        private readonly ITrackingAppService _trackingAppService;
        private readonly IFlowAppService _flowAppService;

        private readonly ITestOutputHelper _output;

        public TrackingAppServiceFlowControl_Tests(ITestOutputHelper output)
        {
            _trackingAppService = GetRequiredService<ITrackingAppService>();
            _flowAppService = GetRequiredService<IFlowAppService>();
            _output = output;
        }

        [Fact]
        public async Task ExecFlow_WithoutStartQuotation_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar cotización";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar cotización: " + trackingStep1.Error?.Message);
            Assert.Contains("no se puede Finalizar cotización".ToLower(), trackingStep1.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep1.Error?.Message.ToLower());

            var trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar suscripción".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep1.Error?.Message);
            Assert.Contains("no se puede Finalizar suscripción".ToLower(), trackingStep1.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep1.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndQuotation_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar cotización";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar suscripción".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            var trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep1.Error?.Message);
            Assert.Contains("no se puede Finalizar suscripción".ToLower(), trackingStep1.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep1.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutStartSubscription_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar suscripción";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep1.Error?.Message);
            Assert.Contains("no se puede Finalizar suscripción".ToLower(), trackingStep1.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep1.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndSubscription_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar suscripción";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutStartSale_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar venta";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndSale_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar venta";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutStartPayment_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar pago";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar pago".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndPayment_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar pago";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutStartInspection_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar inspección";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar inspección".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndInspection_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar inspección";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutStartContract_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Iniciar contrato";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar contrato".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutEndContract_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar contrato";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Cerrar venta".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_WithoutCloseSale_ReturnError()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Cerrar venta";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Iniciar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar tracking".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("se esperaba " + expected.ToLower(), trackingStep.Error?.Message.ToLower());


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_AllStepsRightFlow001_ReturnSucess()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW001");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);
            var expected = "Finalizar proceso";

            _output.WriteLine("Operación esperada: " + expected);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);
            Assert.Contains("no se puede Finalizar notificación".ToLower(), trackingStep.Error?.Message.ToLower());
            Assert.Contains("no existe ese paso en el flujo: " + flow.Result.Name.ToLower(), trackingStep.Error?.Message.ToLower());

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Null(trackingStep.Error);


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_AllStepsRightFlow003_ReturnSucess()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW003");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Null(trackingStep.Error);


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_AllStepsRightFlow005_ReturnSucess()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW005");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Null(trackingStep.Error);


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_AllStepsRightFlow006_ReturnSucess()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW006");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Null(trackingStep.Error);


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }

        [Fact]
        public async Task ExecFlow_AllStepsRightFlow012_ReturnSucess()
        {
            var flow = await _flowAppService.GetByCodeAsync("FLOW012");

            CreateTrackingRequestDto newTracking = new()
            {
                WayCode = "BANCAWEB",
                IpClient = "192.168.0.1"
            };

            var tracking = await _trackingAppService.CreateAsync(newTracking, flow.Result.Id);

            var trackingStep = await _trackingAppService.StartQuotationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar cotización: " + trackingStep.Error?.Message);

            var trackingStep1 = await _trackingAppService.EndQuotationAsync(tracking.Result.Id);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar cotización: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSubscriptionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar suscripción: " + trackingStep.Error?.Message);

            trackingStep1 = await _trackingAppService.EndSubscriptionAsync(tracking.Result.Id, false);
            trackingStep.Result = (TrackingDto)trackingStep1.Result;
            _output.WriteLine("Finalizar suscripción: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartSaleAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndSaleAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndPaymentAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar pago: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.CloseSaleAsync(tracking.Result.Id);
            _output.WriteLine("Cerrar venta: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndInspectionAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar inspección: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartContractAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndContractAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar contrato: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.StartNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Iniciar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndNotificationAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar notificación: " + trackingStep.Error?.Message);

            trackingStep = await _trackingAppService.EndProcessAsync(tracking.Result.Id);
            _output.WriteLine("Finalizar proceso: " + trackingStep.Error?.Message);
            Assert.Null(trackingStep.Error);


            Assert.NotNull(tracking);
            Assert.NotNull(tracking.Result);
        }


    }

}

