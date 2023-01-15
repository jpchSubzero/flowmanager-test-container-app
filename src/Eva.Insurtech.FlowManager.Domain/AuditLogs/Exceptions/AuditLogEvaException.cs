using Eva.Framework.Utility.Response.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Eva.Insurtech.AuditLogEva.AuditLogs.Exceptions
{
    [Serializable]
    public class AuditLogEvaException : GeneralException
    {

        protected AuditLogEvaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public AuditLogEvaException(Error error) : base(error.Code, error.Message, error.Details) { }
    }
}
