using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.Products.Exceptions
{
    [Serializable]
    public class ProductNotFoundException : GeneralException
    {
        protected ProductNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ProductNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
