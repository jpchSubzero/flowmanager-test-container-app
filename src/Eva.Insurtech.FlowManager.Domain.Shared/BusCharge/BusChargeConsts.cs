namespace Eva.Insurtech.FlowManagers.BusCharge
{
    public static class BusChargeConsts
    {
        public const string ApiMethodGetBusServicesRetries = "TRAER REINTENTOS DE BUSS SERVICES";
        public const string MessengerReportApi = "messengerservice/email/send/report";
        public const string MessengerSuccess = "Accepted";
        public const string GetBusServices = "}/bus-charge-by-tracking";
        public const string MessengerTextBus = "Se a producido un error en el siguiente producto : ";
        public const string ComplementEmail = "_NOTIF";
        public const string ErrorEmail = "No se pudo enviar el email de notificación";
    }
}
