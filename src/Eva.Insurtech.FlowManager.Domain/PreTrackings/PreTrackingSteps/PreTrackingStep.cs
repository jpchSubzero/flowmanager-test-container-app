using Eva.Insurtech.FlowManagers.PreTrackings.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.PreTrackings.PreTrackingSteps
{
    public class PreTrackingStep : ValueObject
    {
        public virtual Guid PreTrackingId { get; private set; }
        public virtual string Container { get; private set; }
        public virtual string Component { get; private set; }
        public virtual string Method { get; private set; }
        public virtual string Body { get; private set; }
        public virtual int Iterations { get; private set; }
        public virtual DateTime RegisterDate { get; private set; }
        public virtual string Observations { get; private set; }

        protected PreTrackingStep()
        { }

        public PreTrackingStep(
            [NotNull] Guid preTrackingId,
            [NotNull] string container,
            [NotNull] string component,
            [NotNull] string method,
            [NotNull] string body,
            string observations
        )
        {
            PreTrackingId = preTrackingId;
            Container = container;
            Component = component;
            Method = method;
            Body = body;
            Observations = observations;
            Iterations = Iterations + 1;
            RegisterDate = DateTime.Now;
        }

        public PreTrackingStep(PreTrackingStepInput input)
        {
            PreTrackingId = input.PreTrackingId;
            Container = input.Container;
            Component = input.Component;
            Method = input.Method;
            Body = input.Body;
            Iterations++;
            RegisterDate = DateTime.Now;
        }

        public void SetBody(string input)
        {
            Body = input;
            Iterations++;
            RegisterDate = DateTime.Now;
        }

        public void CleanBody()
        {
            Body = string.Empty;
        }

        public void SetObservations(string input)
        {
            Observations = input;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PreTrackingId;
            yield return Container;
            yield return Component;
            yield return Method;
            yield return Body;
            yield return Iterations;
            yield return RegisterDate;
            yield return Observations;
        }
    }
}






