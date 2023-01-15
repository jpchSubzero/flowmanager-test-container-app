using System;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos
{
    public class RequestDto
    {
        public Guid RequestLogId { get; set; }
        public string TransactionReference { get; set; }
        public string Service { get; set; }
        public string Body { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Observations { get; set; }
    }
}



