using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps
{
    public class PreTracking : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid? TrackingId { get; private set; }
        public virtual string TransactionReference { get; private set; }
        public virtual string Identification { get; private set; }
        public virtual string FullName { get; private set; }
        public virtual string CellPhone { get; private set; }
        public virtual string Email { get; private set; }
        public virtual ICollection<PreTrackingStep> PreTrackingSteps { get; private set; }

        protected PreTracking() { }

        public PreTracking(
            [NotNull] string transactionReference,
            [NotNull] string identification,
            [NotNull] string fullName,
            [NotNull] string cellPhone,
            [NotNull] string email
        )
        {
            TransactionReference = transactionReference;
            Identification = identification;
            FullName = fullName;
            CellPhone = cellPhone;
            Email = email;
        }

        public PreTracking(PreTrackingInput input)
        {
            TransactionReference = input.TransactionReference;
            Identification = input.Identification;
            FullName = input.FullName;
            CellPhone = input.CellPhone;
            Email = input.Email;
        }

        public void AddPreTrackingSteps(PreTrackingStepInput failureLog)
        {
            PreTrackingSteps ??= new List<PreTrackingStep>();
            PreTrackingSteps.Add(new PreTrackingStep(failureLog));
        }

        public void UpdateTracking(Guid input)
        {
            TrackingId = input;
            foreach (var preTrackingStep in PreTrackingSteps)
            {
                preTrackingStep.CleanBody();
            }
        }
    }
}




