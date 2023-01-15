using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Values;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Requests
{
    public class Request : ValueObject
    {
        public virtual Guid RequestLogId { get; private set; }
        public virtual string TransactionReference { get; private set; }
        public virtual string Service { get; private set; }
        public virtual string Body { get; private set; }
        public virtual DateTime RegisterDate { get; private set; }
        public virtual string Observations { get; private set; }

        protected Request() { }

        public Request(
            [NotNull] Guid requestLogId, 
            [NotNull] string transactionReference, 
            [NotNull] string service,
            [NotNull] string body, 
            string observations
        )
        {
            RequestLogId = requestLogId;
            TransactionReference = transactionReference;
            Service = service;
            Body = body;
            RegisterDate = DateTime.Now;
            Observations = observations;
        }

        public Request(RequestInput input)
        {
            RequestLogId = input.RequestLogId;
            TransactionReference = input.TransactionReference;
            Service = input.Service;
            Body = input.Body;
            RegisterDate = DateTime.Now;
            Observations = input.Observations;
        }

        public void SetBody(string input)
        {
            Body = input;
            RegisterDate = DateTime.Now;
        }

        public void SetObservations(string input)
        {
            Observations = input;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return RequestLogId;
            yield return TransactionReference;
            yield return Service;
            yield return Body;
            yield return RegisterDate;
            yield return Observations;
    }
}
}



