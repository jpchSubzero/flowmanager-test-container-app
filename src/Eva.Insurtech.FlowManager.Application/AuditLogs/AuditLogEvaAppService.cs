using Eva.Framework.Utility.Response.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Eva.Framework.Utility.Response;
using Volo.Abp.ObjectMapping;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.TrackingManagers.Trackings;
using Eva.Insurtech.FlowManagers.Catalogs;
using Volo.Abp.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eva.Insurtech.AuditLogEva.AuditLog
{
    public class AuditLogEvaAppService : FlowManagerAppService, IAuditLogEvaAppService
    {
        private readonly AuditLogEvaManager _auditManager;
        private readonly FlowManager _flowManager;
        private readonly ProductManager _productManager;
        private readonly TrackingManager _trackingManager;
        private readonly IObjectMapper _objectMapper;
        private readonly CatalogManager _catalogManager;

        public AuditLogEvaAppService(AuditLogEvaManager auditManager, IObjectMapper objectMapper, ILogger<AuditLogEvaAppService> logger,
            FlowManager flowManager,
            ProductManager productManager,
            TrackingManager trackingManager, CatalogManager catalogManager
            ) : base(logger)
        {
            _auditManager = auditManager;
            _objectMapper = objectMapper;
            _flowManager = flowManager;
            _productManager = productManager;
            _trackingManager = trackingManager;
            _catalogManager=catalogManager;
        }

        public async Task<Response<AuditLogResponseDto>> InsertAuditAsync(AuditLogInputDto input)
        {
            ResponseManager<AuditLogResponseDto> response = new();
            try
            {
                var result = new AuditLogResponseDto();
                var auditData = _objectMapper.Map<AuditLogInputDto, AuditLogEva>(input);
                var auditExtra=await GetExtraProperties(input.TrackingId);
                var iterator = auditExtra.Result.GetEnumerator();
                while (iterator.MoveNext())
                {
                    var prop = iterator.Current;
                    auditData.ExtraProperties.Add(prop.Key,prop.Value);
                }
                var resultInsert = await _auditManager.InsertAsync(auditData);
                if (resultInsert != null)
                    result.Respond = "Audit accepted";
                return response.OnSuccess(result);
            }
            catch (Exception ex)
            {
                return GetErrorResponse<AuditLogResponseDto>(AuditLogEvaErrors.GetErrorGeneral(), ex);
            }
        }

        public async Task<Response<ExtraPropertyDictionary>> GetExtraProperties(Guid trackingId)
        {
            ResponseManager<ExtraPropertyDictionary> response = new();
            try
            {
                var extraProperties = new ExtraPropertyDictionary();
                var tracking = await _trackingManager.GetAsync(trackingId);
                var flow = await _flowManager.GetAsync(tracking.FlowId);
                var product = await _productManager.GetByExternalIdAsync(flow.ProductId);
                var step = await _catalogManager.GetByExternalIdAsync(tracking.StateId);
                var result = new AuditExtraPropertiesDto()
                {
                    ChannelCode = flow.ChannelCode,
                    ChannelName = flow.Name,
                    FlowCode = flow.Code,
                    FlowId = flow.Id,
                    ProductCode = product.Code,
                    ProductName = product.Name,
                    CurrentStep = (step != null ? step.Name : "No existe trackingId")
                };
                extraProperties.Add("ChannelCode", result.ChannelCode);
                extraProperties.Add("ChannelName", result.ChannelName);
                extraProperties.Add("FlowCode", result.FlowCode);
                extraProperties.Add("ProductCode", result.ProductCode);
                extraProperties.Add("ProductName", result.ProductName);
                extraProperties.Add("CurrentStep",result.CurrentStep);

                return response.OnSuccess(extraProperties);                
            }
            catch(Exception)
            {
                var extraProperties = new ExtraPropertyDictionary();
                extraProperties.Add("ChannelCode", "No existe tracking Id");
                return response.OnSuccess(extraProperties);

            }
        }
    }
}
