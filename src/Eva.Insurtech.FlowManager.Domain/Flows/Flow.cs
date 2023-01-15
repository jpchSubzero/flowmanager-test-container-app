using Eva.Insurtech.FlowManagers.Flows.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eva.Insurtech.FlowManagers.Flows
{
    public class Flow : FullAuditedAggregateRoot<Guid>
    {
        /// <summary>
        /// Código de referencia para contextos externos
        /// </summary>
        public virtual string Code { get; private set; }

        /// <summary>
        /// Código de referencia al canal
        /// </summary>
        public virtual string ChannelCode { get; private set; }

        /// <summary>
        /// Nombre del flujo
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// Descrípción del flujo
        /// </summary>
        public virtual string Description { get; private set; }

        /// <summary>
        /// Estado del flujo (activo o inactivo)
        /// </summary>
        public virtual bool IsActive { get; private set; }

        /// <summary>
        /// Referencia al identificador del producto
        /// </summary>
        public virtual Guid ProductId { get; private set; }

        /// <summary>
        /// Listado de pasos soportados por el flujo
        /// </summary>
        public virtual ICollection<FlowStep> FlowSteps { get; private set; }

        /// <summary>
        /// Tiempo máximo para completar el flujo. 0 es sin límite
        /// </summary>
        public virtual int MaxLifeTime { get; private set; }

        /// <summary>
        /// Tiempo máximo para validar la OTP
        /// </summary>
        public virtual int OtpMaxTime { get; private set; }

        /// <summary>
        /// Máximo número de intentos
        /// </summary>
        public virtual int OtpMaxAttempts { get; private set; }

        /// <summary>
        /// Máximo número de reenvios
        /// </summary>
        public virtual int OtpMaxResends {  get; private set; }

        protected Flow()
        {
        }

        public Flow(
            [NotNull][MaxLength(FlowConsts.CodeMaxLength)] string code,
            [NotNull][MaxLength(FlowConsts.CodeMaxLength)] string channelCode,
            [NotNull][MaxLength(FlowConsts.NameMaxLength)] string name,
            [NotNull][MaxLength(FlowConsts.NameMaxLength)] string description,
            [NotNull] Guid productId,
            [NotNull] bool isActive = true,
            int maxLifeTime = 0,
            int otpMaxTime = 0,
            int otpMaxAttempts = 0,
            int otpMaxResends = 0
        )
        {
            Code = code;
            ChannelCode = channelCode;
            Name = name;
            Description = description;
            ProductId = productId;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
            OtpMaxTime = otpMaxTime;
            OtpMaxAttempts = otpMaxAttempts;
            OtpMaxResends = otpMaxResends;
        }

        public Flow(
            [NotNull][MaxLength(FlowConsts.CodeMaxLength)] string code,
            [NotNull][MaxLength(FlowConsts.CodeMaxLength)] string channelCode,
            [NotNull][MaxLength(FlowConsts.NameMaxLength)] string name,
            [NotNull][MaxLength(FlowConsts.NameMaxLength)] string description,
            [NotNull] Guid productId,
            [NotNull] ICollection<FlowStep> flowSteps,
            [NotNull] bool isActive = true,
            int maxLifeTime = 0,
            int otpMaxTime = 0,
            int otpMaxAttempts = 0,
            int otpMaxResends = 0
        )
        {
            Code = code;
            ChannelCode = channelCode;
            Name = name;
            Description = description;
            ProductId = productId;
            FlowSteps = flowSteps;
            IsActive = isActive;
            MaxLifeTime = maxLifeTime;
            OtpMaxTime = otpMaxTime;
            OtpMaxAttempts = otpMaxAttempts;
            OtpMaxResends = otpMaxResends;
        }

        public void SetName(string input)
        {
            Name = input;
        }

        public void SetDescription(string input)
        {
            Description = input;
        }

        public void SetCode(string input)
        {
            Code = input;
        }

        public void SetChannelCode(string input)
        {
            ChannelCode = input;
        }

        public void SetProductId(Guid input)
        {
            ProductId = input;
        }

        public void SetMaxLifeTime(int input)
        {
            MaxLifeTime = input;
        }

        public void SetOtpMaxTime(int input)
        {
            OtpMaxTime = input;
        }

        public void SetOtpMaxAttempts(int input)
        {
            OtpMaxAttempts = input;
        }

        public void SetOtpMaxResends(int input)
        {
            OtpMaxResends = input;
        }

        public void AddFlowStep(FlowStepInput flowStep)
        {
            FlowSteps ??= new List<FlowStep>();
            FlowSteps.Add(new FlowStep(flowStep));
        }
        
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void OrderFlowSteps() 
        {
            throw new NotImplementedException();
        }
        
        public void UpdateFlowStep()
        {
            throw new NotImplementedException();
        }
        
        public void EnableFlowStep()
        {
            throw new NotImplementedException();
        }
        
        public void DisableFlowStep()
        {
            throw new NotImplementedException();
        }
        
        public void DeleteFlowStep()
        {
            throw new NotImplementedException();
        }
    }
}