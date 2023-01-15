namespace Eva.Insurtech.FlowManagers.Trackings.ProcessLogs
{
    public static class ProcessLogConsts
    {
        public const int ActionMaxLength = SharedConsts.DescriptionMaxLength;
        public const int RequestMaxLength = SharedConsts.ExceptionMaxLength;
        public const int ResponseMaxLength = SharedConsts.ExceptionMaxLength;
        public const string TableName = "ProcessLogs";
    }
}
