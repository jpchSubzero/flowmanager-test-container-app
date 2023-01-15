using Eva.Insurtech.FlowManagers.Trackings.ProcessLogs.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.Trackings.ProcessLogs
{
    public class ProcessLog : ValueObject
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
        /// Request de la petición
        /// </summary>
        public virtual string Request { get; private set; }

        /// <summary>
        /// Response de la petición
        /// </summary>
        public virtual string Response { get; private set; }

        /// <summary>
        /// Servicio utilizado
        /// </summary>
        public virtual string Action { get; private set; }

        /// <summary>
        /// Contador de peticiones iguales para conocer cuantas veces se intentó
        /// </summary>
        public virtual int Version { get; private set; }

        /// <summary>
        /// Fecha de registro del error
        /// </summary>
        public virtual DateTime RegisterTime { get; private set; }

        protected ProcessLog()
        {
        }

        public ProcessLog(
            [NotNull] Guid trackingId,
            [NotNull] Guid stepId,
            [NotNull][MaxLength(ProcessLogConsts.RequestMaxLength)] string request,
            [NotNull][MaxLength(ProcessLogConsts.ResponseMaxLength)] string response,
            [NotNull][MaxLength(ProcessLogConsts.ActionMaxLength)] string action,
            [NotNull] int version
        )
        {
            TrackingId = trackingId;
            StepId = stepId;
            Request = request;
            Response = response;
            Action = action;
            RegisterTime = DateTime.Now;
            Version = version;
        }

        public ProcessLog(ProcessLogInput input)
        {
            TrackingId = input.TrackingId;
            StepId = input.StepId;
            Request = input.Request;
            Response = input.Response;
            Action = input.Action;
            RegisterTime = DateTime.Now;
            Version = input.Version;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrackingId;
            yield return StepId;
            yield return Request;
            yield return Response;
            yield return Action;
            yield return RegisterTime;
            yield return Version;
        }
    }
}