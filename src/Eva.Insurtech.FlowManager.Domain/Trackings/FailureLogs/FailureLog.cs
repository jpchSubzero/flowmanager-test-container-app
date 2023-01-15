using Eva.Insurtech.FlowManagers.FailureLogs;
using Eva.Insurtech.FlowManagers.Trackings.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public class FailureLog : ValueObject
    {
        /// <summary>
        /// Código identificador del tracking
        /// </summary>
        public virtual Guid TrackingId { get; private set; }

        /// <summary>
        /// Nombre del método donde se generó el error
        /// </summary>
        public virtual string Method { get; private set; }

        /// <summary>
        /// Descripción del error
        /// </summary>
        public virtual string Error { get; private set; }

        /// <summary>
        /// Detalles del error. Ej. Descripción personalizada, excepción, etc
        /// </summary>
        public virtual string Detail { get; private set; }

        /// <summary>
        /// Fecha de registro del error
        /// </summary>
        public virtual DateTime RegisterTime { get; private set; }

        /// <summary>
        /// Estado en que se encontraba el proceso al registrar el error
        /// </summary>
        public virtual Guid StateId { get; private set; }

        /// <summary>
        /// Paso en que se encontraba el proceso al registrar el error
        /// </summary>
        public virtual Guid StepId { get; private set; }

        protected FailureLog()
        {
        }

        public FailureLog(
            [NotNull] Guid trackingId,
            [NotNull][MaxLength(FailureLogConsts.NameMaxLength)] string method,
            [NotNull][MaxLength(FailureLogConsts.DescriptionMaxLength)] string error,
            [NotNull][MaxLength(FailureLogConsts.ExceptionMaxLength)] string detail,
            [NotNull] Guid stateId,
            [NotNull] Guid stepId
        )
        {
            TrackingId = trackingId;
            Method = method;
            Error = error;
            Detail = detail;
            RegisterTime = DateTime.Now;
            StepId = stepId;
        }

    public FailureLog(FailureLogInput input)
        {
            TrackingId = input.TrackingId;
            Method = input.Method;
            Error = input.Error;
            Detail = input.Detail;
            RegisterTime = DateTime.Now;
            StateId = input.StateId;
            StepId = input.StepId;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrackingId;
            yield return Method;
            yield return Error;
            yield return Detail;
            yield return RegisterTime;
            yield return StateId;
            yield return StepId;
        }
    }
}











