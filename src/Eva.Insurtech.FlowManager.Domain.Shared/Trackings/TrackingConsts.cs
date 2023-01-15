using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers
{
    public static class TrackingConsts
    {
        public const int CodeMaxLength = SharedConsts.CodeMaxLength;
        public const int NameMaxLength = SharedConsts.NameMaxLength;
        public const int DescriptionMaxLength = SharedConsts.DescriptionMaxLength;
        public const int EndPointMaxLength = SharedConsts.EndPointMaxLength;
        public const int QueueMaxLength = SharedConsts.QueueMaxLength;
        public const string TableName = "Trackings";

        //Catálogos
        public const string FLOW_STATES = "FLOW_STATES";
        public const string FLOW_GENERAL_STATES = "FLOW_GENERAL_STATES";
        public const string FLOW_STEPS = "FLOW_STEPS";
        public const string RESPONSE_FILTERS = "RESPONSE_FILTERS";

        //Pasos de flujo
        public const string START_TRACKING = "START_TRACKING";
        public const string END_PROCESS = "END_PROCESS";
        public const string GET_PRODUCTS = "GET_PRODUCTS";
        public const string GET_FLOW = "GET_FLOW";
        public const string GET_PLANS = "GET_PLANS";
        public const string START_SIMULATOR = "START_SIMULATOR";
        public const string START_QUOTATION = "START_QUOTATION";
        public const string START_SUBSCRIPTION = "START_SUBSCRIPTION";
        public const string START_PAYMENT = "START_PAYMENT";
        public const string START_SALE = "START_SALE";
        public const string START_CONTRACT = "START_CONTRACT";
        public const string START_NOTIFICATION = "START_NOTIFICATION";
        public const string START_INSPECTION = "START_INSPECTION";
        public const string CLOSE_SALE = "CLOSE_SALE";

        //Estados de tracking

        public const string TRACKING_START_PREFIX = "START_";
        public const string TRACKING_DONE_SUFFIX = "_DONE";
        public const string TRACKING_ERROR_SUFFIX = "_ERROR";
        public const string TRACKING_CREATED_SUFFIX = "_CREATED";
        public const string TRACKING_STARTED_SUFFIX = "_STARTED";
        public const string TRACKING_ENDED_SUFFIX = "_ENDED";
        public const string TRACKING_REGISTERED_SUFFIX = "_REGISTERED";

        public const string TRACKING_CREATED = "TRACKING_CREATED";
        public const string QUOTATION_STARTED = "QUOTATION_STARTED";
        public const string QUOTATION_DONE = "QUOTATION_DONE";
        public const string QUOTATION_ERROR = "QUOTATION_ERROR";
        public const string SUBSCRIPTION_STARTED = "SUBSCRIPTION_STARTED";
        public const string SUBSCRIPTION_DONE = "SUBSCRIPTION_DONE";
        public const string SUBSCRIPTION_ERROR = "SUBSCRIPTION_ERROR";
        public const string SALE_STARTED = "SALE_STARTED";
        public const string SALE_REGISTERED = "SALE_REGISTERED";
        public const string SALE_ERROR = "SALE_ERROR";
        public const string SALE_PAYMENT_PENDING = "SALE_PAYMENT_PENDING";
        public const string SALE_PAYED = "SALE_PAYED";
        public const string SALE_DONE = "SALE_DONE";
        public const string PAYMENT_STARTED = "PAYMENT_STARTED";
        public const string PAYMENT_DONE = "PAYMENT_DONE";
        public const string PAYMENT_ERROR = "PAYMENT_ERROR";
        public const string CONTRACT_STARTED = "CONTRACT_STARTED";
        public const string CONTRACT_DONE = "CONTRACT_DONE";
        public const string CONTRACT_ERROR = "CONTRACT_ERROR";
        public const string DOCUMENTATION_STARTED = "DOCUMENTATION_STARTED";
        public const string DOCUMENTATION_DONE = "DOCUMENTATION_DONE";
        public const string DOCUMENTATION_ERROR = "DOCUMENTATION_ERROR";
        public const string NOTIFICATION_STARTED = "NOTIFICATION_STARTED";
        public const string NOTIFICATION_DONE = "NOTIFICATION_DONE";
        public const string NOTIFICATION_ERROR = "NOTIFICATION_ERROR";
        public const string INSPECTION_STARTED = "INSPECTION_STARTED";
        public const string INSPECTION_DONE = "INSPECTION_DONE";
        public const string INSPECTION_ERROR = "INSPECTION_ERROR";
        public const string TRACKING_ENDED = "TRACKING_ENDED";
        public const string TRACKING_TURNED_BACK = "TRACKING_TURNED_BACK";
        public const string TRACKING_TURNED_FOREWARD = "TRACKING_TURNED_FOREWARD";

        //Estados generales de tracking
        public const string INITIALIZED = "INITIALIZED";
        public const string IN_PROGRESS = "IN_PROGRESS";
        public const string ENDED = "ENDED";
        public const string TIMED_OUT = "TIMED_OUT";
        public const string ABANDONED = "ABANDONED";
        public const string ERROR = "ERROR";

        public const int START_LABEL = 0;
        public const int END_LABEL = 1;
        public const int CLOSE_LABEL = 2;

        public const int FIRST_STEP = 1;
        public const int FIRST_FLOW_STEP = 2;

        public const string EndQuotationFilterSuffixCode = "_TRA_END_QUO_FILTER";
        public const string EndSubscriptionFilterSuffixCode = "_TRA_END_SUS_FILTER";

        public const string MaxCreateByIp = "MaxCreateByIp";
        public const string Tracking = "Tracking";

        public const string ID = "ID";
        public const string SERVICE = "SERVICE";
        public const string SERVICE_TODAY = "SERVICE_TODAY";

        public const string TRANSACTION_REFERENCE = "TRANSACTION_REFERENCE";

        public const string EmailPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    }
}
