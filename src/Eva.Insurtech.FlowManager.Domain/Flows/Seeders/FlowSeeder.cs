using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Products;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Eva.Insurtech.FlowManagers.Flows.Seeders
{
    [ExcludeFromCodeCoverage]
    public static class FlowSeeder
    {
        private static int _affectedRows = 0;
        public static async Task<int> LoadInitialData(
            ProductManager productManager,
            CatalogManager catalogManager,
            FlowManager flowManager
        )
        {
            var products = await productManager.GetAllAsync();
            var catalogs = await catalogManager.GetAllAsync();
            if (products.Any() && catalogs.Any())
            {
                await LoadFlows(flowManager, productManager, catalogManager);
            }
            return _affectedRows;
        }

        private static async Task LoadFlows(FlowManager flowManager, ProductManager productManager, CatalogManager catalogManager)
        {
            List<Flow> flowsToCreate = new();

            var steps = await catalogManager.GetAllAsync();
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

            var products = await productManager.GetAllAsync();
            var prod001 = products.FirstOrDefault(x => x.Code.Equals("PROD001")).ProductId;
            var prod002 = products.FirstOrDefault(x => x.Code.Equals("PROD002")).ProductId;
            var prod003 = products.FirstOrDefault(x => x.Code.Equals("PROD003")).ProductId;
            var prod004 = products.FirstOrDefault(x => x.Code.Equals("PROD004")).ProductId;
            var prod005 = products.FirstOrDefault(x => x.Code.Equals("PROD005")).ProductId;
            var prod006 = products.FirstOrDefault(x => x.Code.Equals("PROD006")).ProductId;
            var prod007 = products.FirstOrDefault(x => x.Code.Equals("PROD007")).ProductId;
            var prod008 = products.FirstOrDefault(x => x.Code.Equals("PROD008")).ProductId;
            var prod009 = products.FirstOrDefault(x => x.Code.Equals("PROD009")).ProductId;
            var prod010 = products.FirstOrDefault(x => x.Code.Equals("PROD010")).ProductId;
            var prod011 = products.FirstOrDefault(x => x.Code.Equals("PROD011")).ProductId;
            var prod012 = products.FirstOrDefault(x => x.Code.Equals("PROD012")).ProductId;
            var segDesNomCon = products.FirstOrDefault(x => x.Code.Equals("SEG_DES_NOM_CON")).ProductId;
            var prodGenericDelta = products.FirstOrDefault(x => x.Code.Equals("PROD_GENERIC_DELTA")).ProductId;
            var prod013 = products.FirstOrDefault(x => x.Code.Equals("PROD013")).ProductId;

            //Estructura de constructor de Flow
            //code,channelCode,name,description,productId,isActive,maxLifeTime,otpMaxTime,otpMaxAttempts,otpMaxResends

            var flowSteps1 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW001", "EVA", "Flujo Sin límites para EVA", "Flujo de ramo vehículos de plan sin límites", prod001, flowSteps1, true));

            var flowSteps2 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW002", "EVA", "Seguro por kilómetros para EVA", "Contrata el plan que mejor se ajuste a los kilómetros que recorres, manteniendo las mejores coberturas y beneficios.", prod002, flowSteps2, true));

            var flowSteps3 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startSubscription, 2, "subscription", string.Empty, true),
                new FlowStep(startSale, 3, "sale", string.Empty, true),
                new FlowStep(closeSale, 4, "sale", string.Empty, true),
                new FlowStep(startContract, 5, "contract", string.Empty, true),
                new FlowStep(startNotification, 6, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 7, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW003", "BANCO01", "Mas vida segura Plus", "Mas vida segura Plus para Banco del Pichincha", prod004, flowSteps3, true, 0, 2, 4, 5));

            var flowSteps5 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startSubscription, 2, "subscription", string.Empty, true),
                new FlowStep(startSale, 3, "sale", string.Empty, true),
                new FlowStep(closeSale, 4, "sale", string.Empty, true),
                new FlowStep(startContract, 5, "contract", string.Empty, true),
                new FlowStep(startNotification, 6, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 7, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW005", "BANCO01", "Plan deuda protegida", "Plan deuda protegida para Banco del Pichincha", prod005, flowSteps5, true, 0, 2, 4, 5));

            var flowSteps6 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startSubscription, 2, "subscription", string.Empty, true),
                new FlowStep(startSale, 3, "sale", string.Empty, true),
                new FlowStep(startPayment, 4, "payment", string.Empty, true),
                new FlowStep(closeSale, 5, "sale", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(startNotification, 7, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 8, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW004", "BANCO01", "Protección total Fraudes para Banco del Pichincha", "Protección total Fraudes", prod003, flowSteps6, true));

            var flowSteps7 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW006", "BROK002", "Asistencia Médica para Confiamed", "Asistencia Médica", prod006, flowSteps7, true));

            var flowSteps8 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW007", "BROK001", "NovaPack para Menta", "NovaPack", prod007, flowSteps8, true));

            var flowSteps9 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW008", "BROK001", "Seguro por kilómetros para Menta", "Contrata el plan que mejor se ajuste a los kilómetros que recorres, manteniendo las mejores coberturas y beneficios.", prod002, flowSteps9, true));

            var flowSteps10 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW009", "BANCO01", "Seguro por kilómetros para Banco del Pichincha", "Contrata el plan que mejor se ajuste a los kilómetros que recorres, manteniendo las mejores coberturas y beneficios.", prod002, flowSteps10, true));

            var flowSteps11 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW010", "BROK001", "Asistencia Médica para Menta", "Asistencia Médica", prod006, flowSteps11, true));

            var flowSteps12 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW011", "EVA", "Asistencia Médica para Banco del Pichincha", "Asistencia Médica", prod006, flowSteps12, true));

            var flowSteps13 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(endProcess, 8, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW012", "BROK001", "Flujo Celu seguro para Menta", "Flujo Celu seguro para Menta", prod008, flowSteps13, true));

            var flowSteps14 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW013", "BANCO01", "Flujo Seguro Vehicular Total para Banco del Pichincha", "Flujo Seguro Vehicular Total para Banco del Pichincha", prod009, flowSteps14, true));

            var flowSteps15 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startInspection, 6, "inspection", string.Empty, true),
                new FlowStep(startContract, 7, "contract", string.Empty, true),
                new FlowStep(closeSale, 8, "sale", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW014", "BANCO01", "Flujo Seguro Vehicular Kilómetros para Banco del Pichincha", "Flujo Seguro Vehicular Kilómetros para Banco del Pichincha", prod010, flowSteps15, true));

            var flowSteps16 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW015", "BROK002", "Gastos Médicos Mayores Confiamed", "Gastos Médicos Mayores", prod011, flowSteps16, true));

            var flowSteps17 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW016", "BROK001", "Gastos Médicos Mayores Menta", "Gastos Médicos Mayores", prod011, flowSteps17, true));

            var flowSteps18 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW017", "EVA", "Gastos Médicos Mayores Eva", "Gastos Médicos Mayores", prod011, flowSteps18, true));

            var flowSteps19 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startSubscription, 2, "subscription", string.Empty, true),
                new FlowStep(startSale, 3, "sale", string.Empty, true),
                new FlowStep(closeSale, 4, "sale", string.Empty, true),
                new FlowStep(startContract, 5, "contract", string.Empty, true),
                new FlowStep(startNotification, 6, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 7, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW018", "BANCO01", "Salud Integral Total para Banco del Pichincha", "Salud Integral Total para Banco del Pichincha", prod012, flowSteps19, true, 0, 2, 4, 5));

            var flowSteps20 = new List<FlowStep>
            {
                 new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW019", "BROK002", "InspiraT Confiamed", "InspiraT Confiamed", prod013, flowSteps20, true));

            var flowSteps21 = new List<FlowStep>
            {
                 new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startPayment, 5, "payment", string.Empty, true),
                new FlowStep(startContract, 6, "contract", string.Empty, true),
                new FlowStep(closeSale, 7, "sale", string.Empty, true),
                new FlowStep(startNotification, 8, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 9, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW020", "EVA", "InspiraT Eva", "InspiraT Eva", prod013, flowSteps21, true));

            var flowDes1 = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startSubscription, 2, "subscription", string.Empty, true),
                new FlowStep(startSale, 3, "sale", string.Empty, true),
                new FlowStep(closeSale, 4, "sale", string.Empty, true),
                new FlowStep(startContract, 5, "contract", string.Empty, true),
                new FlowStep(startNotification, 6, "messengerservice", string.Empty, true),
                new FlowStep(endProcess, 7, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW_DES1", "BANCO01", "Flujo seguro de desempleo nómina confianza", "Flujo seguro de desempleo nómina confianza para Banco del Pichincha", segDesNomCon, flowDes1, true));

            var flowGenericDelta = new List<FlowStep>
            {
                new FlowStep(startTracking, 1, "flowmanager", string.Empty, true),
                new FlowStep(startQuotation, 2, "quotation", string.Empty, true),
                new FlowStep(startSubscription, 3, "subscription", string.Empty, true),
                new FlowStep(startSale, 4, "sale", string.Empty, true),
                new FlowStep(startContract, 5, "contract", string.Empty, true),
                new FlowStep(endProcess, 6, "flowmanager", string.Empty, true)
            };
            flowsToCreate.Add(new Flow("FLOW_GENERIC_DELTA", "EVA", "Flujo genérico para delta", "Flujo genérico para delta", prodGenericDelta, flowGenericDelta, true));


            await ProcessFlows(flowManager, flowsToCreate);

            var flows = await flowManager.GetListAsync(true);
            _affectedRows += flows.Count + flows.Sum(x => x.FlowSteps.Count);
        }

        private static async Task ProcessFlows(FlowManager flowManager, List<Flow> flowsToCreate)
        {
            var previousItems = await flowManager.GetListAsync(true);
            List<Flow> updated = new();

            foreach (var flowToCreate in flowsToCreate)
            {
                var oldFlow = await flowManager.FindByCodeAsync(flowToCreate.Code, true);
                if (oldFlow == null)
                {
                    await flowManager.InsertAsync(flowToCreate);
                }
                else
                {
                    oldFlow.SetName(flowToCreate.Name);
                    oldFlow.SetDescription(flowToCreate.Description);
                    oldFlow.SetProductId(flowToCreate.ProductId);
                    oldFlow.SetChannelCode(flowToCreate.ChannelCode);
                    oldFlow.SetMaxLifeTime(flowToCreate.MaxLifeTime);
                    oldFlow.SetOtpMaxAttempts(flowToCreate.OtpMaxAttempts);
                    oldFlow.SetOtpMaxResends(flowToCreate.OtpMaxResends);
                    oldFlow.SetOtpMaxTime(flowToCreate.OtpMaxTime);
                    if (flowToCreate.IsActive) oldFlow.Activate(); else oldFlow.Deactivate();
                    oldFlow.IsDeleted = flowToCreate.IsDeleted;
                    oldFlow.FlowSteps?.Clear();

                    flowToCreate.FlowSteps.ToList().ForEach(x => {
                        x.SetFlowId(oldFlow.Id);
                        oldFlow.FlowSteps.Add(x);
                    });
                    updated.Add(oldFlow);
                }
            }

            var deletedItems = previousItems.Where(x => !flowsToCreate.Any(y => y.Code.Equals(x.Code))).ToList();
            foreach (var deletedItem in deletedItems)
            {
                deletedItem.FlowSteps?.Clear();
                deletedItem.Deactivate();
                deletedItem.IsDeleted = true;
                updated.Add(deletedItem);
            }

            await flowManager.UpdateManyAsync(updated);
        }
    }
}