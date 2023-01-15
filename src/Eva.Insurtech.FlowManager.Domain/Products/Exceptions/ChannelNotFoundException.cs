using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.Products.Exceptions
{
    [Serializable]
    public class ChannelNotFoundException : GeneralException
    {
        protected ChannelNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ChannelNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
