using Eva.Insurtech.FlowManagers.RequestLogs.Inputs;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests;
using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Eva.Insurtech.FlowManagers.RequestLogs
{
    public  class RequestLog : FullAuditedAggregateRoot<Guid>
    {
        public virtual string Service { get; private set; }
        public virtual int Iterations { get; private set; }
        public virtual DateTime RegisterDate { get; private set; }
        public virtual string Observations { get; private set; }
        public virtual ICollection<Request> Requests { get; private set; }

        protected RequestLog() { }

        public RequestLog(
            [NotNull] string service, 
            int iterations, 
            string observations
        )
        {
            Service = service;
            Iterations = iterations;
            RegisterDate = DateTime.Now;
            Observations = observations;
        }

        public RequestLog(RequestLogInput input)
        {
            Service = input.Service;
            Iterations = input.Iterations;
            RegisterDate = DateTime.Now;
            Observations = input.Observations;
        }

        public void AddRequest(RequestInput request)
        {
            Requests ??= new List<Request>();
            Requests.Add(new Request(request));
        }

        public void SetRegisterDate(DateTime input)
        {
            RegisterDate = input;
        }

        public void GroupRequestsByService()
        {
            Iterations = Requests.Count;
            Requests.Clear();
        }
    }
}


