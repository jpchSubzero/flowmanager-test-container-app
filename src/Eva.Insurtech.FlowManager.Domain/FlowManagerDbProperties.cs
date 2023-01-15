namespace Eva.Insurtech.FlowManagers
{
    public static class FlowManagerDbProperties
    {
        public static string DbTablePrefix { get; set; } = "";

        public static string DbSchema { get; set; } = "FlowManager";

        public const string ConnectionStringName = "FlowManager";
    }
}
