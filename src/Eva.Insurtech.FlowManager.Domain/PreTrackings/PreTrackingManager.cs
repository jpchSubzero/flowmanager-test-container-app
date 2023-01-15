using Eva.Insurtech.FlowManagers.PreTrackings.Exceptions;
using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps;
using Eva.Insurtech.FlowManagers.Trackings;
using Eva.Insurtech.FlowManagers.Trackings.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;

namespace Eva.Insurtech.FlowManagers.PreTrackings
{
    public class PreTrackingManager : DomainService
    {
        private readonly IPreTrackingRepository _preTrackingRepository;
        private readonly ITrackingRepository _trackingRepository;

        public PreTrackingManager(IPreTrackingRepository preTrackingRepository, ITrackingRepository trackingRepository)
        {
            _preTrackingRepository = preTrackingRepository;
            _trackingRepository = trackingRepository;
        }

        public async Task<PreTracking> InsertAsync(PreTracking preTracking, bool autoSave = true)
        {
            return await _preTrackingRepository.InsertAsync(preTracking, autoSave);
        }

        public async Task<PreTracking> AddPreTrackingStep(PreTrackingStepInput input)
        {
            var preTracking = await _preTrackingRepository.GetByIdAsync(input.PreTrackingId);
            GenericValidations.ValidateIfItemExists(preTracking, new PreTrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            var preTrackingStep = preTracking.PreTrackingSteps.FirstOrDefault(x => x.Container.Equals(input.Container) && x.Component.Equals(input.Component) && x.Method.Equals(input.Method));
            if (preTrackingStep != null)
            {
                preTrackingStep.SetBody(input.Body);
                preTrackingStep.SetObservations(input.Observations);
                return await _preTrackingRepository.UpdateAsync(preTracking, true);
            }
            preTracking.AddPreTrackingSteps(input);
            return await _preTrackingRepository.UpdateAsync(preTracking, true);
        }

        public async Task<PreTracking> GetById(Guid preTrackingId)
        {
            return await _preTrackingRepository.GetByIdAsync(preTrackingId);
        }

        public async Task<PreTracking> GetByTransactionReference(string transactionReference)
        {
            return await _preTrackingRepository.GetByTransactionReference(transactionReference);
        }

        public async Task<PreTracking> UpdateTracking(string transactionReference, Guid trackingId)
        {
            var preTracking = await GetByTransactionReference(transactionReference);
            GenericValidations.ValidateIfItemExists(preTracking, new PreTrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            var tracking = await _trackingRepository.GetByIdAsync(trackingId);
            GenericValidations.ValidateIfItemExists(tracking, new TrackingNotFoundException(TrackingErrorCodes.GetErrorNotFoundById()));

            preTracking.UpdateTracking(trackingId);
            return await _preTrackingRepository.UpdateAsync(preTracking, true);
        }
    }
}
