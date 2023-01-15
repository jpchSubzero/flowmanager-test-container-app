using Eva.Insurtech.FlowManagers.RequestLogs.Requests.Dtos;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Dtos
{
    public class RequestLogDto : EntityDto<Guid>
    {
        public string Service { get; set; }
        public int Iterations { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Observations { get; set; }
        public ICollection<RequestDto> Requests { get; set; }
    }
}


