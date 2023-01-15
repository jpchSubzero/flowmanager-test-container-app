using Eva.Framework.Utility.Response.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Trackings.Exceptions
{
    [Serializable]
    public class TrackingAlreadyExistException : GeneralException
    {
        protected TrackingAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public TrackingAlreadyExistException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
