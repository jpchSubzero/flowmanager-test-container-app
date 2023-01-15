using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.Catalogs.Exceptions
{
    [Serializable]
    public class CatalogNotFoundException : GeneralException
    {
        protected CatalogNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public CatalogNotFoundException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
