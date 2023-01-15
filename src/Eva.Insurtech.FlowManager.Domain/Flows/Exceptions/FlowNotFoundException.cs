using Eva.Framework.Utility.Response.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.Exceptions
{
    [Serializable]
    public class FlowNotFoundException : GeneralException
    {
        protected FlowNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public FlowNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
