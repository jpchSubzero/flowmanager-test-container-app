using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.PreTrackings.Exceptions
{
    [Serializable]
    public class PreTrackingNotFoundException : GeneralException
    {
        protected PreTrackingNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public PreTrackingNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
