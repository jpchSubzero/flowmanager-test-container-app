using System;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Requests.Inputs
{
    public class RequestInput
    {
        public Guid RequestLogId { get; set; }
        public string TransactionReference { get; set; }
        public string Service { get; set; }
        public string Body { get; set; }
        public string Observations { get; set; }
    }
}
