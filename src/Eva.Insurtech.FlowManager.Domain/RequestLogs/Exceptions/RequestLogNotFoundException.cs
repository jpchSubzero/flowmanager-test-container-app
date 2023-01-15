using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.RequestLogs.Exceptions
{
    [Serializable]
    public class RequestLogNotFoundException : GeneralException
    {
        protected RequestLogNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public RequestLogNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
