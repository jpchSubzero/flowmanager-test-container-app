using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Catalogs.Exceptions;
using Eva.Insurtech.FlowManagers.Flows.Exceptions;
using Eva.Insurtech.FlowManagers.Flows.Inputs;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Products.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class FlowManager : DomainService
    {
        private readonly IFlowRepository _flowRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;


        public FlowManager(IFlowRepository flowRepository, ICatalogRepository catalogRepository, IProductRepository productRepository)
        {
            _flowRepository = flowRepository;
            _productRepository = productRepository;
            _catalogRepository = catalogRepository;
        }

        public async Task<Flow> InsertAsync(Flow flow, bool autoSave = true)
        {
            await ValidateIfIsNew(flow, FlowErrorCodes.GetErrorAlreadyExists());
            await ValidateRequiredIds(flow);

            return await _flowRepository.InsertAsync(flow, autoSave);
        }

        public async Task<Flow> UpdateAsync(Flow flow, bool autoSave = true)
        {
            await ValidateIfIsNewCode(flow, FlowErrorCodes.GetErrorAlreadyExists());

            var flowToUpdate = await _flowRepository.GetByIdWithInactivesAsync(flow.Id);

            GenericValidations.ValidateIfItemExists(flowToUpdate, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));

            await ValidateRequiredIds(flow);

            SetValues(flow, flowToUpdate);

            return await _flowRepository.UpdateAsync(flowToUpdate, autoSave);
        }
        
        public async Task UpdateManyAsync(ICollection<Flow> flows)
        {
            await _flowRepository.UpdateManyAsync(flows, true);
        }
        
        public async Task<bool> DeleteAsync(Guid flowId, bool autoSave = true)
        {
            var flow = await _flowRepository.GetByIdWithoutInactivesAsync(flowId);
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));
            await _flowRepository.DeleteAsync(flow, autoSave);
            return true;
        }

        public async Task<ICollection<Flow>> GetListAsync(bool withDetails = false)
        {
            return await _flowRepository.GetAllAsync(withDetails);
        }

        public async Task<Flow> GetAsync(Guid flowId)
        {
            var flow = await _flowRepository.GetByIdWithoutInactivesAsync(flowId);
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));
            return flow;
        }

        public async Task<Flow> GetByCodeAsync(string code, bool withInactives = false)
        {
            var flow = await _flowRepository.GetByCodeAsync(code, withInactives);
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundByCode()));
            return flow;
        }

        public async Task<Flow> FindByCodeAsync(string code, bool withInactives = false)
        {
            var flow = await _flowRepository.GetByCodeAsync(code, withInactives);
            return flow;
        }

        public async Task<ICollection<Flow>> GetByProductIdAsync(Guid id)
        {
            var flows = await _flowRepository.GetByProductIdAsync(id);
            GenericValidations.ValidateIfItemExists(flows, new FlowNotFoundException(ProductErrorCodes.GetErrorNotFoundById()));
            return flows;
        }

        public async Task<Flow> GetByProductIdAndChannelCodeAsync(Guid id, string channelCode)
        {
            var flow = await _flowRepository.GetByProductIdAndChannelCodeAsync(id, channelCode);
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(ProductErrorCodes.GetErrorNotFoundById()));
            return flow;
        }

        public async Task<Flow> AddFlowStep(FlowStepInput input)
        {
            var flow = await _flowRepository.GetByIdWithoutInactivesAsync(input.FlowId);
            GenericValidations.ValidateIfItemExists(flow, new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById()));
            var way = await _catalogRepository.GetByExternalIdAsync(input.StepId);
            GenericValidations.ValidateIfItemExists(way, new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.CATALOG)));

            flow.AddFlowStep(input);
            return await _flowRepository.UpdateAsync(flow, true);
        }

        public async Task<bool> DeleteFlowStepAsync(Guid flowId, Guid stepId)
        {
            var flow = await _flowRepository.GetFlowWithDetailsAsync(flowId);
            if (flow == null)
            {
                throw new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById());
            }
            var regStep = flow.FlowSteps.FirstOrDefault(c => c.StepId == stepId);
            if (regStep != null)
            {
                regStep.IsActive = false;
                await _flowRepository.DeleteAsync(flow, true);
                return true;
            }
            return false;
        }
        #region Private Methods

        private static void SetValues(Flow flow, Flow flowToUpdate)
        {
            flowToUpdate.SetCode(flow.Code);
            flowToUpdate.SetName(flow.Name);
            flowToUpdate.SetDescription(flow.Description);
            flowToUpdate.SetProductId(flow.ProductId);
            if (flow.IsActive)
            {
                flowToUpdate.Activate();
            }
            else
            {
                flowToUpdate.Deactivate();
            }
        }

        private async Task ValidateIfIsNew(Flow flow, Error error)
        {
            if (flow == null)
                throw new ArgumentNullException();
            if (await _flowRepository.FindIfExistsAsync(flow) != null)
                throw new FlowAlreadyExistException(error);
        }

        private async Task ValidateIfIsNewCode(Flow flow, Error error)
        {
            if (flow == null)
                throw new ArgumentNullException();
            if (await _flowRepository.FindByCodeAsync(flow) != null)
                throw new FlowAlreadyExistException(error);
        }

        private async Task ValidateRequiredIds(Flow flow)
        {
            if (await _productRepository.GetByExternalIdAsync(flow.ProductId) == null)
                throw new ProductNotFoundException(ProductErrorCodes.GetErrorNotFoundById(LabelConsts.PRODUCT));
        }


        #endregion
    }
}
