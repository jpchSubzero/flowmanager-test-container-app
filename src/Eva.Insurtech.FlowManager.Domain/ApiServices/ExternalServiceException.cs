using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.ApiServices
{
    [Serializable]
    public class ExternalServiceException : GeneralException
    {
        public ExternalServiceException(Error error) : base(error.Code, error.Message, error.Details)
        {
        }

        protected ExternalServiceException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
        }
    }
}
