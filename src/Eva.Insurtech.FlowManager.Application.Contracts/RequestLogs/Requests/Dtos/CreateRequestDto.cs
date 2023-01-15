using System;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos
{
    public class CreateRequestDto
    {
        public Guid RequestLogId { get; private set; }
        public string TransactionReference { get; set; }
        public string Service { get; set; }
        public string Body { get; set; }
        public string Observations { get; set; }

        public void SetRequestLogId(Guid input)
        {
            RequestLogId = input;
        }
    }
}



