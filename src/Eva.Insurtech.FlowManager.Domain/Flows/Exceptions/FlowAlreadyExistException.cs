using Eva.Framework.Utility.Response.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Flows.Exceptions
{
    [Serializable]
    public class FlowAlreadyExistException : GeneralException
    {
        protected FlowAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public FlowAlreadyExistException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
