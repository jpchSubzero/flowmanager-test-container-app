using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Flows.Inputs;
using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.RequestLogs;
using Eva.Insurtech.FlowManagers.Trackings;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class FlowDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly ITrackingRepository _trackingRepository;
        private readonly IPreTrackingRepository _preTrackingRepository;
        private readonly IRequestLogRepository _requestLogRepository;
        private readonly IGuidGenerator _guidGenerator;

        public FlowDataSeedContributor(IFlowRepository flowRepository, IProductRepository productRepository, ICatalogRepository catalogRepository, ITrackingRepository trackingRepository, IPreTrackingRepository preTrackingRepository, IRequestLogRepository requestLogRepository, IGuidGenerator guidGenerator)
        {
            _flowRepository = flowRepository;
            _productRepository = productRepository;
            _catalogRepository = catalogRepository;
            _trackingRepository = trackingRepository;
            _preTrackingRepository = preTrackingRepository;
            _requestLogRepository = requestLogRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var product1 = await _productRepository.InsertAsync(
                new Product(
                    _guidGenerator.Create(),
                    "PRODUCTO01",
                    "EXT_PRODUCTO01",
                    "Producto 1"
                ),
                true
            );

            var product2 = await _productRepository.InsertAsync(
                new Product(
                    _guidGenerator.Create(),
                    "PRODUCTO02",
                    "EXT_PRODUCTO02",
                    "Producto 2"
                ),
                true
            );

            var product3 = await _productRepository.InsertAsync(
                new Product(
                    _guidGenerator.Create(),
                    "PRODUCTO03",
                    "EXT_PRODUCTO03",
                    "Producto 3"
                ),
                true
            );

            var catalog1 = await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG01",
                    "Catálogo 1"
                ),
                true
            );

            var catalog2 = await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG02",
                    "Catálogo 2"
                ),
                true
            );

            var catalog3 = await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG03",
                    "Catálogo 3"
                ),
                true
            );

            await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG04",
                    "Catálogo 4"
                ),
                true
            );

            await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG05",
                    "Catálogo 5"
                ),
                true
            );

            await _catalogRepository.InsertAsync(
                new Catalog(
                    _guidGenerator.Create(),
                    "CATALOG06",
                    "Catálogo 6"
                ),
                true
            );

            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_SUBSCRIPTION", "Iniciar suscripción"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_SIMULATOR", "Iniciar simulador"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "END_PROCESS", "Finalizar proceso"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_CONTRACT", "Iniciar contrato"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_TRACKING", "Iniciar tracking"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_SALE", "Iniciar venta"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "CLOSE_SALE", "Cerrar venta"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_NOTIFICATION", "Iniciar notificación"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_INSPECTION", "Iniciar inspección"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_PAYMENT", "Iniciar pago"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_STARTED", "Venta iniciada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "ERROR", "Error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_PAYMENT_PENDING", "Venta pago pendiente"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "PAYMENT_DONE", "Pago finalizado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "CONTRACT_STARTED", "Contrato iniciado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "INSPECTION_ERROR", "Inspección con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "CONTRACT_DONE", "Contrato finalizado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "INITIALIZED", "Iniciado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "GET_FLOW", "Obtener flujo"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "INSPECTION_STARTED", "Inspección iniciada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "TRACKING_ENDED", "Tracking terminado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "DOCUMENTATION_ERROR", "Documento con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "GET_PRODUCTS", "Obtener productos"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "DOCUMENTATION_STARTED", "Documento iniciado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "DOCUMENTATION_DONE", "Documento finalizado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_REGISTERED", "Venta finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "QUOTATION_STARTED", "Cotización iniciada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SUBSCRIPTION_ERROR", "Suscripción con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "NOTIFICATION_ERROR", "Notificación con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "CONTRACT_ERROR", "Contrato con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SUBSCRIPTION_STARTED", "Suscripción iniciada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "TIMED_OUT", "Caducado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "PAYMENT_ERROR", "Pago con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "IN_PROGRESS", "En progreso"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_DONE", "Venta finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "TRACKING_CREATED", "Tracking creado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SUBSCRIPTION_DONE", "Suscripción finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "INSPECTION_DONE", "Inspección finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "NOTIFICATION_DONE", "Notificación finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "START_QUOTATION", "Iniciar cotización"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "QUOTATION_DONE", "Cotización finalizada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "QUOTATION_ERROR", "Cotización con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_PAYED", "Venta pagada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "NOTIFICATION_STARTED", "Notificación iniciada"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "SALE_ERROR", "Venta con error"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "ABANDONED", "Abandonado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "GET_PLANS", "Obtener planes"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "PAYMENT_STARTED", "Pago iniciado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "ENDED", "Terminado"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "BANCAMOVIL", "Banca Movil"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "BANCAWEB", "Banca Web"), true);
            await _catalogRepository.InsertAsync(new Catalog(_guidGenerator.Create(), "BPEMBEBIDO", "Banco Pichincha Embebebido"), true);

            Catalog buscatalog3 = new Catalog(_guidGenerator.Create(), "FLOW003_NOTIF", "Datos notificación para FLOW003");
            buscatalog3.SetExtraProperties(new ExtraPropertyDictionary() { { "errorNotification", "{'emailsCustomer':[{'email':'charles.pazmino@willdom.com','name':'Charles Pazmiño'},{'email':'charles.eva.2022@outlook.com','name':'Diego Eva'}],'codeTemplate':'MAILING_REPORT','textData':[{'text':' al ejecutarse en el FLOW003'}]}" } });
            await _catalogRepository.InsertAsync(buscatalog3, true);

            Catalog buscatalog5 = new Catalog(_guidGenerator.Create(), "FLOW005_NOTIF", "Datos notificación para FLOW005");
            buscatalog5.SetExtraProperties(new ExtraPropertyDictionary() { { "errorNotification", "{'emailsCustomer':[{'email':'charles.pazmino@willdom.com','name':'Charles Pazmiño'},{'email':'charles.eva.2022@outlook.com','name':'Diego Eva'}],'codeTemplate':'MAILING_REPORT','textData':[{'text':' al ejecutarse en el FLOW005'}]}" } });
            await _catalogRepository.InsertAsync(buscatalog5, true);

            Catalog buscatalog12 = new Catalog(_guidGenerator.Create(), "FLOW012_NOTIF", "Datos notificación para FLOW012");
            buscatalog12.SetExtraProperties(new ExtraPropertyDictionary() { { "errorNotification", "{'emailsCustomer':[{'email':'charles.pazmino@willdom.com','name':'Charles Pazmiño'},{'email':'charles.eva.2022@outlook.com','name':'Diego Eva'}],'codeTemplate':'MAILING_REPORT','textData':[{'text':' al ejecutarse en el FLOW012'}]}" } });
            await _catalogRepository.InsertAsync(buscatalog12, true);







            var steps = await _catalogRepository.GetAllAsync();
            var startQuotation = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_QUOTATION)).CatalogId;
            var startTracking = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_TRACKING)).CatalogId;
            var startSubscription = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_SUBSCRIPTION)).CatalogId;
            var startPayment = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_PAYMENT)).CatalogId;
            var startSale = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_SALE)).CatalogId;
            var startContract = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_CONTRACT)).CatalogId;
            var startNotification = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_NOTIFICATION)).CatalogId;
            var startInspection = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.START_INSPECTION)).CatalogId;
            var closeSale = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.CLOSE_SALE)).CatalogId;
            var endProcess = steps.FirstOrDefault(x => x.Code.Equals(TrackingConsts.END_PROCESS)).CatalogId;

            var flow1 = await _flowRepository.InsertAsync(
                new Flow(
                    "FLOW01",
                    "BANCO01",
                    "Flujo 1",
                    "Descripción Flujo 1",
                    product1.ProductId
                ),
                true
            );

            await _flowRepository.InsertAsync(
                new Flow(
                    "FLOW02",
                    "BANCO01",
                    "Flujo 2",
                    "Descripción Flujo 2",
                    product2.ProductId
                ),
                true
            );

            await _flowRepository.InsertAsync(
                new Flow(
                    "FLOW03",
                    "BANCO01",
                    "Flujo 3",
                    "Descripción Flujo 3",
                    product3.ProductId
                ),
                true
            );

            await _flowRepository.InsertAsync(
                new Flow(
                    "FLOW04",
                    "BANCO02",
                    "Flujo 3",
                    "Descripción Flujo 3",
                    product3.ProductId
                ),
                true
            );

            await _flowRepository.InsertAsync(
                new Flow(
                    "FLOW05",
                    "BROK002",
                    "Flujo 3",
                    "Descripción Flujo 3",
                    product3.ProductId
                ),
                true
            );

            var flow001 = await _flowRepository.InsertAsync(new Flow("FLOW001", "EVA", "Flujo Sin límites para EVA", "Flujo de ramo vehículos de plan sin límites", product2.ProductId));
            var flow002 = await _flowRepository.InsertAsync(new Flow("FLOW002", "EVA", "Flujo Sin límites para EVA", "Flujo de ramo vehículos de plan sin límites", product3.ProductId, true, 1, 1, 1, 1));
            var flow003 = await _flowRepository.InsertAsync(new Flow("FLOW003", "BANCO01", "Mas vida segura Plus", "Mas vida segura Plus para Banco del Pichincha", product3.ProductId));
            var flow005 = await _flowRepository.InsertAsync(new Flow("FLOW005", "BANCO01", "Plan deuda protegida", "Plan deuda protegida para Banco del Pichincha", product2.ProductId));
            var flow006 = await _flowRepository.InsertAsync(new Flow("FLOW006", "BROK002", "Asistencia Médica para Confiamed", "Asistencia Médica", product1.ProductId));
            var flow012 = await _flowRepository.InsertAsync(new Flow("FLOW012", "BROK001", "Flujo Celu seguro para Menta", "Flujo Celu seguro para Menta", product3.ProductId));

            flow001.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startQuotation, 2, "quotation", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startSubscription, 3, "subscription", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startSale, 4, "sale", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startPayment, 5, "payment", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startInspection, 6, "inspection", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(startContract, 7, "contract", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(closeSale, 8, "sale", string.Empty, true));
            flow001.AddFlowStep(new FlowStepInput(endProcess, 9, "flowmanager", string.Empty, true));

            flow002.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startQuotation, 2, "quotation", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startSubscription, 3, "subscription", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startSale, 4, "sale", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startPayment, 5, "payment", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startInspection, 6, "inspection", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(startContract, 7, "contract", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(closeSale, 8, "sale", string.Empty, true, 1));
            flow002.AddFlowStep(new FlowStepInput(endProcess, 9, "flowmanager", string.Empty, true, 1));

            flow003.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startQuotation, 2, "quotation", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startSubscription, 3, "subscription", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startPayment, 4, "payment", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startSale, 5, "sale", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startContract, 6, "contract", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(startNotification, 7, "messengerservice", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(closeSale, 8, "sale", string.Empty, true));
            flow003.AddFlowStep(new FlowStepInput(endProcess, 9, "flowmanager", string.Empty, true));

            flow005.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(startSubscription, 2, "subscription", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(startSale, 3, "sale", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(closeSale, 4, "sale", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(startContract, 5, "contract", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(startNotification, 6, "messengerservice", string.Empty, true));
            flow005.AddFlowStep(new FlowStepInput(endProcess, 7, "flowmanager", string.Empty, true));

            flow006.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startQuotation, 2, "quotation", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startSubscription, 3, "subscription", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startSale, 4, "sale", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startPayment, 5, "payment", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startContract, 6, "contract", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(closeSale, 7, "sale", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(startNotification, 8, "messengerservice", string.Empty, true));
            flow006.AddFlowStep(new FlowStepInput(endProcess, 9, "flowmanager", string.Empty, true));

            flow012.AddFlowStep(new FlowStepInput(startTracking, 1, "flowmanager", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(startQuotation, 2, "quotation", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(startSubscription, 3, "subscription", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(startSale, 4, "sale", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(closeSale, 5, "sale", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(startContract, 6, "contract", string.Empty, true));
            flow012.AddFlowStep(new FlowStepInput(endProcess, 7, "flowmanager", string.Empty, true));

            await _trackingRepository.InsertAsync(
                new Tracking(
                    flow1.Id,
                    catalog1.CatalogId,
                    catalog2.CatalogId,
                    catalog3.CatalogId,
                    "BANCO01",
                    "BANCAMOVIL",
                    "192.168.0.1"
                ),
                true
            );

            await _trackingRepository.InsertAsync(
                new Tracking(
                    flow1.Id,
                    catalog1.CatalogId,
                    catalog2.CatalogId,
                    catalog3.CatalogId,
                    "BANCO01",
                    "BANCAMOVIL",
                    "192.168.0.1"
                ),
                true
            );

            await _preTrackingRepository.InsertAsync(
                new PreTracking(
                    "pre-tracking-test",
                    "1123659863",
                    "Juan José Ramírez Rodríguez",
                    "0952148520",
                    "email@email.com"
                )
            );
            await _requestLogRepository.InsertAsync(
                new RequestLog(
                    "service-test",
                    1,
                    ""
                )
            );

            await _preTrackingRepository.InsertAsync(
                new PreTracking(
                    "82592564-40d6-e6bf-2d42-3a017bffe69c",
                    "1123659863",
                    "Juan José Ramírez Rodríguez",
                    "0952148520",
                    "email@email.com"
                )
            );
        }
    }
}

