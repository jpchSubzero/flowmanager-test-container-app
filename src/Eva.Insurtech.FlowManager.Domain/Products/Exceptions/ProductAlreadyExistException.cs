using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.Products.Exceptions
{
    [Serializable]
    public class ProductAlreadyExistException : GeneralException
    {
        protected ProductAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ProductAlreadyExistException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
