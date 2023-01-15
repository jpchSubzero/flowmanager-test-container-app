using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers
{
    public static class FlowConsts
    {
        public const int CodeMaxLength = SharedConsts.CodeMaxLength;
        public const int NameMaxLength = SharedConsts.NameMaxLength;
        public const int DescriptionMaxLength = SharedConsts.DescriptionMaxLength;
        public const int EndPointMaxLength = SharedConsts.EndPointMaxLength;
        public const int QueueMaxLength = SharedConsts.QueueMaxLength;
        public const string TableName = "Flows";

        public const string FLOW_STATES = "FLOW_STATES";
        public const string FLOW_GENERAL_STATES = "FLOW_GENERAL_STATES";
        public const string FLOW_STEPS = "FLOW_STEPS";

    }
}
