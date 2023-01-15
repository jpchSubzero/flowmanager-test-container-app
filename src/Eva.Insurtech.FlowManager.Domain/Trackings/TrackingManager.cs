using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers;
using Eva.Insurtech.FlowManagers.Catalogs;
using Eva.Insurtech.FlowManagers.Catalogs.Exceptions;
using Eva.Insurtech.FlowManagers.Flows;
using Eva.Insurtech.FlowManagers.Flows.Exceptions;
using Eva.Insurtech.FlowManagers.PreTrackings;
using Eva.Insurtech.FlowManagers.Products;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.TrackingManagers.Trackings
{
    public class TrackingManager : DomainService
    {

        private readonly ITrackingRepository _trackingRepository;
        private readonly IFlowRepository _flowRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly ProductManager _productManager;
        private readonly PreTrackingManager _preTrackingManager;

        public TrackingManager(
            ITrackingRepository trackingRepository,
            IFlowRepository flowRepository,
            ICatalogRepository catalogRepository,
            ProductManager productManager,
            PreTrackingManager preTrackingManager
        )
        {
            _trackingRepository = trackingRepository;
            _flowRepository = flowRepository;
            _catalogRepository = catalogRepository;
            _productManager = productManager;
            _preTrackingManager = preTrackingManager;
        }

        public async Task<Tracking> InsertAsync(Tracking tracking, bool autoSave = true)
        {
            await ValidateIfIsNew(tracking, TrackingErrorCodes.GetErrorAlreadyExists());
            await ValidateRequiredIds(tracking);

            return await _trackingRepository.InsertAsync(tracking, autoSave);
        }

        public async Task<Tracking> UpdateAsync(Tracking tracking, bool autoSave = true)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(tracking.Id);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            await ValidateRequiredIds(tracking);

            SetValues(tracking, trackingToUpdate);

            return await _trackingRepository.UpdateAsync(trackingToUpdate, autoSave);
        }

        public async Task<bool> DeleteAsync(Guid trackingId, bool autoSave = true)
        {
            var flow = await _trackingRepository.GetByIdAsync(trackingId);
            GenericValidations.ValidateIfItemExists(flow, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            await _trackingRepository.DeleteAsync(flow, autoSave);
            return true;
        }

        public async Task<ICollection<Tracking>> GetListAsync(bool withDetails = false)
        {
            return await _trackingRepository.GetAllAsync(withDetails);
        }

        public async Task<Tracking> GetAsync(Guid trackingId)
        {
            var tracking = await _trackingRepository.GetByIdAsync(trackingId);
            return tracking;
        }

        public async Task<Tracking> UpdateEndDateAsync(Guid trackingId)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(trackingId);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            trackingToUpdate.UpdateEndDate();

            return await _trackingRepository.UpdateAsync(trackingToUpdate, true);
        }

        public async Task<Tracking> UpdateAbandonDateAsync(Guid trackingId)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(trackingId);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            trackingToUpdate.UpdateAbandonDate();

            return await _trackingRepository.UpdateAsync(trackingToUpdate, true);
        }

        public async Task<Tracking> UpdateStepAsync(Guid trackingId, Guid stepId)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(trackingId);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            var step = await _catalogRepository.GetByExternalIdAsync(stepId);

            GenericValidations.ValidateIfItemExists(step, new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.FLOW_STEP)));

            trackingToUpdate.SetStepId(stepId);

            return await _trackingRepository.UpdateAsync(trackingToUpdate, true);
        }

        public async Task<Tracking> UpdateStateAsync(Guid trackingId, Guid stateId)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(trackingId);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            var state = await _catalogRepository.GetByExternalIdAsync(stateId);

            GenericValidations.ValidateIfItemExists(state, new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.STATE)));

            trackingToUpdate.SetStateId(stateId);

            return await _trackingRepository.UpdateAsync(trackingToUpdate, true);
        }

        public async Task<Tracking> UpdateGeneralStateAsync(Guid trackingId, Guid generalStateId)
        {
            var trackingToUpdate = await _trackingRepository.GetByIdAsync(trackingId);

            GenericValidations.ValidateIfItemExists(trackingToUpdate, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            var generalState = await _catalogRepository.GetByExternalIdAsync(generalStateId);

            GenericValidations.ValidateIfItemExists(generalState, new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.GENERAL_STATE)));

            trackingToUpdate.SetGeneralStateId(generalStateId);

            return await _trackingRepository.UpdateAsync(trackingToUpdate, true);
        }

        public async Task<Tracking> AddFailureLogAsync(FailureLogInput input)
        {
            var tracking = await _trackingRepository.GetByIdAsync(input.TrackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));
            var previousState = tracking.StateId;

            input.StateId = previousState;
            input.StepId = tracking.StepId;
            tracking.AddFailureLogs(input);

            return await _trackingRepository.UpdateAsync(tracking, true);
        }

        public async Task<Tracking> SetExtraPropertiesAsync(Guid trackingId, ExtraPropertyDictionary extraProperties)
        {
            var tracking = await _trackingRepository.GetByIdAsync(trackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            ValidateExtraProperties(extraProperties);
            tracking.SetExtraProperties(extraProperties);

            var trackingUpdated = await _trackingRepository.UpdateAsync(tracking, true);

            if (tracking.ExtraProperties[PreTrackingConsts.TransactionReference] != null)
                await _preTrackingManager.UpdateTracking(tracking.ExtraProperties[PreTrackingConsts.TransactionReference].ToString(), trackingUpdated.Id);

            return trackingUpdated;
        }

        private static void ValidateExtraProperties(ExtraPropertyDictionary extraProperties)
        {
            if (extraProperties.IsNullOrEmpty())
            {
                throw new TrackingExtraPropertiesNullException(TrackingErrorCodes.GetErrorExtraPropertiesNull());
            }
            foreach (var extraProperty in extraProperties)
            {
                if (extraProperty.Key.IsNullOrEmpty() || extraProperty.Value.ToString().IsNullOrEmpty())
                {
                    throw new TrackingExtraPropertiesNullException(TrackingErrorCodes.GetErrorExtraPropertiesNull());
                }
            }
        }

        public async Task<Tracking> GetByExtraPropertiesAsync(ExtraPropertyDictionary extraProperties)
        {
            var tracking = await _trackingRepository.GetByExtraPropertiesAsync(extraProperties);
            return tracking;
        }

        public async Task<Tracking> GetByChannelExtraPropertiesAsync(string channelCode, ExtraPropertyDictionary extraProperties)
        {
            var tracking = await _trackingRepository.GetByChannelExtraPropertiesAsync(channelCode, extraProperties);
            return tracking;
        }

        public async Task<Tracking> GetByWayChannelExtraPropertiesAsync(string wayCode, string channelCode, ExtraPropertyDictionary extraProperties)
        {
            var tracking = await _trackingRepository.GetByWayChannelExtraPropertiesAsync(wayCode, channelCode, extraProperties);
            return tracking;
        }

        public async Task<Tracking> AddProcessLogAsync(ProcessLogInput input)
        {
            var tracking = await _trackingRepository.GetByIdAsync(input.TrackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            input.StepId = tracking.StepId;
            tracking.AddProcessLogs(input);

            return await _trackingRepository.UpdateAsync(tracking, true);
        }

        public async Task<Tracking> AddSubStepLogAsync(SubStepLogInput input)
        {
            var tracking = await _trackingRepository.GetByIdAsync(input.TrackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            input.StepId = tracking.StepId;
            tracking.AddSubStepLogs(input);

            return await _trackingRepository.UpdateAsync(tracking, true);
        }

        public async Task<ICollection<Tracking>> GetByIpClientAsync(string ip)
        {
            return await _trackingRepository.GetByIpClientAsync(ip);
        }

        #region Private Methods

        private static void SetValues(Tracking tracking, Tracking trackingToUpdate)
        {
            trackingToUpdate.SetFlowId(tracking.FlowId);
            trackingToUpdate.SetStepId(tracking.StepId);
            trackingToUpdate.SetStateId(tracking.StateId);
            trackingToUpdate.SetGeneralStateId(tracking.GeneralStateId);
            trackingToUpdate.SetFailureLogs(tracking.FailureLogs);
            trackingToUpdate.SetProccessLogs(tracking.ProcessLogs);
            trackingToUpdate.SetSubStepLogs(tracking.SubStepLogs);
            trackingToUpdate.UpdateChangeStateDate();
        }


        private async Task ValidateIfIsNew(Tracking tracking, Error error)
        {
            if (tracking == null)
                throw new ArgumentNullException(nameof(tracking));
            if (await _trackingRepository.FindIfExistsAsync(tracking) != null)
                throw new TrackingAlreadyExistException(error);
        }

        private async Task ValidateRequiredIds(Tracking tracking)
        {
            if (await _catalogRepository.GetByExternalIdAsync(tracking.StepId) == null)
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.CATALOG));
            if (await _catalogRepository.GetByExternalIdAsync(tracking.StateId) == null)
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.CATALOG));
            if (await _catalogRepository.GetByExternalIdAsync(tracking.GeneralStateId) == null)
                throw new CatalogNotFoundException(CatalogErrorCodes.GetErrorNotFoundById(LabelConsts.CATALOG));
            if (await _flowRepository.GetByIdWithoutInactivesAsync(tracking.FlowId) == null)
                throw new FlowNotFoundException(FlowErrorCodes.GetErrorNotFoundById());
            await _productManager.ValidateIfChannelExistInProduct(tracking.ChannelCode);
        }


        #endregion
    }
}
