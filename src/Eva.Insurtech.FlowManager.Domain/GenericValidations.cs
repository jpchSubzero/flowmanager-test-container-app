using Eva.Framework.Utility.Response.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eva.Insurtech.FlowManagers
{
    public static class GenericValidations
    {
        public static void ValidateIfItemExists<T>(T item, Exception exception)
        {
            if (item == null)
                throw exception;
        }

        public static void ValidateIfItemExistsOnCollection<T>(T code, ICollection<T> collection, Exception exception)
        {
            if (collection != null && collection.Any(x => code.Equals(x)))
                throw exception;
        }

    }
}
