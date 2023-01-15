using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs;
using Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs.Inputs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.Trackings.SubStepsLogs
{
    public class SubStepLog : ValueObject
    {
        /// <summary>
        /// Código identificador del tracking
        /// </summary>
        public virtual Guid TrackingId { get; private set; }

        /// <summary>
        /// Paso en que se encontraba el proceso al registrar el error
        /// </summary>
        public virtual Guid StepId { get; private set; }

        /// <summary>
        /// Código de sub paso (pasos propios de cada flujo dentro de un paso del tracking)
        /// </summary>
        public virtual string SubStepCode { get; private set; }

        /// <summary>
        /// Contador de veces que el usuario pasa por ese paso/pantalla
        /// </summary>
        public virtual int Attempts { get; private set; }

        /// <summary>
        /// Fecha de registro del error
        /// </summary>
        public virtual DateTime RegisterTime { get; private set; }

        protected SubStepLog()
        {
        }

        public SubStepLog(
            [NotNull] Guid trackingId,
            [NotNull] Guid stepId,
            [NotNull] int attempts,
            [NotNull][MaxLength(ProcessLogConsts.RequestMaxLength)] string subStepCode
        )
        {
            TrackingId = trackingId;
            StepId = stepId;
            SubStepCode = subStepCode;
            Attempts = attempts;
            RegisterTime = DateTime.Now;
        }

        public SubStepLog(SubStepLogInput input)
        {
            TrackingId = input.TrackingId;
            StepId = input.StepId;
            SubStepCode = input.SubStepCode;
            Attempts = input.Attempts;
            RegisterTime = DateTime.Now;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrackingId;
            yield return StepId;
            yield return SubStepCode;
            yield return Attempts;
            yield return RegisterTime;
        }
    }
}