using Eva.Framework.Utility.Response.Models;
using System;
using System.Runtime.Serialization;

namespace Eva.Insurtech.FlowManagers.Catalogs.Exceptions
{
    [Serializable]
    public class CatalogAlreadyExistException : GeneralException
    {
        protected CatalogAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public CatalogAlreadyExistException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
