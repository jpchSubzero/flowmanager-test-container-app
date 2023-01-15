using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers
{
    public static class ErrorConsts
    {
        public const string ERROR_GENERAL = "ERROR_GENERAL";
        public const string ERROR_ALREADY_EXIST_CODE = "ERROR_ALREADY_EXIST_CODE";
        public const string ERROR_ALREADY_EXIST_LABEL = "ERROR_ALREADY_EXIST_LABEL";
        public const string ERROR_NOT_FOUND_BY_CODE = "ERROR_NOT_FOUND_BY_CODE";
        public const string ERROR_NOT_FOUND_BY_ID = "ERROR_NOT_FOUND_BY_ID";
        public const string ERROR_ID = "ERROR_ID";
        public const string ERROR_LIMIT_MIN = "ERROR_LIMIT_MIN";
        public const string ERROR_LIMIT_MAX = "ERROR_LIMIT_MAX";
        public const string ERROR_CODE = "ERROR_CODE";
        public const string ERROR_DEPENDENT_ID = "ERROR_DEPENDENT_ID";
        public const string ERROR_DEPENDENT_CODE = "ERROR_DEPENDENT_CODE";
        public const string ERROR_IP_V4_ADDRESS = "ERROR_IP_V4_ADDRESS";

        public const string ERROR_NAME = "ERROR_NAME";
        public const string ERROR_DESCRIPTION = "ERROR_DESCRIPTION";
        public const string ERROR_METHOD = "ERROR_METHOD";
        public const string ERROR_ERROR = "ERROR_ERROR";
        public const string ERROR_DETAIL = "ERROR_DETAIL";
        public const string ERROR_LABEL = "ERROR_LABEL";
        public const string ERROR_VALUE = "ERROR_VALUE";
        public const string ERROR_STATE = "ERROR_STATE";
        public const string ERROR_PRICE = "ERROR_PRICE";
        public const string ERROR_TOTAL_PRICE = "ERROR_TOTAL_PRICE";
        public const string ERROR_TAXES = "ERROR_TAXES";

        public const string ERROR_REQUEST = "ERROR_REQUEST";
        public const string ERROR_RESPONSE = "ERROR_RESPONSE";
        public const string ERROR_ACTION = "ERROR_ACTION";

        public const string ERROR_NULL_EXTRA_PROPERTIES = "ERROR_NULL_EXTRA_PROPERTIES";
        public const string ERROR_CHANNEL_NOT_FOUND_BY_CODE = "ERROR_CHANNEL_NOT_FOUND_BY_CODE";
        public const string ERROR_NULL_CODE = "ERROR_NULL_CODE";
        public const string ERROR_CLOSE_ON_LAST_STEP = "ERROR_CLOSE_ON_LAST_STEP";
        public const string ERROR_INCORRECT_START_STEP = "ERROR_INCORRECT_START_STEP";
        public const string ERROR_INCORRECT_END_STEP = "ERROR_INCORRECT_END_STEP";
        public const string ERROR_INCORRECT_BACKWARD_STEPS = "ERROR_INCORRECT_BACKWARD_STEPS";
        public const string ERROR_INCORRECT_FOREWARD_STEPS = "ERROR_INCORRECT_FOREWARD_STEPS";
        public const string ERROR_STEP_OUT_OF_RANGE= "ERROR_STEP_OUT_OF_RANGE";
        public const string ERROR_STEP_NOT_FOUND_ON_FLOW = "ERROR_STEP_NOT_FOUND_ON_FLOW";
        #region Audit
        public const string ERROR_TRACKINGID = "ERROR_TRACKINGID";
        #endregion Audit
        public const string ERROR_TRACKING_TIME_OUT = "ERROR_TRACKING_TIME_OUT";
        public const string ERROR_TRACKING_NOT_FOUND_BY_IP = "ERROR_TRACKING_NOT_FOUND_BY_IP";
        public const string ERROR_TRACKING_EXCEDED_CREATION_BY_DAY = "ERROR_TRACKING_EXCEDED_CREATION_BY_DAY";

        public const string ERROR_PRE_TRACKING_CREATE = "ERROR_PRE_TRACKING_CREATE";
        public const string ERROR_PRE_TRACKING_ADD_STEP = "ERROR_PRE_TRACKING_ADD_STEP";
        public const string ERROR_GET_PRE_TRACKING = "ERROR_GET_PRE_TRACKING";
        
        public const string ERROR_REQUEST_LOG_CREATE = "ERROR_REQUEST_LOG_CREATE";
        public const string ERROR_REQUEST_LOG_ADD_STEP = "ERROR_REQUEST_LOG_ADD_STEP";
        public const string ERROR_GET_REQUEST_LOG = "ERROR_GET_REQUEST_LOG";

        public const string ERROR_TRANSACTION_REFERENCE = "ERROR_TRANSACTION_REFERENCE";
        public const string ERROR_IDENTIFICATION = "ERROR_IDENTIFICATION";
        public const string ERROR_FULL_NAME = "ERROR_FULL_NAME";
        public const string ERROR_CELL_PHONE = "ERROR_CELL_PHONE";
        public const string ERROR_EMAIL = "ERROR_EMAIL";
        public const string ERROR_CONTAINER = "ERROR_CONTAINER";
        public const string ERROR_COMPONENT = "ERROR_COMPONENT";
        public const string ERROR_BODY = "ERROR_BODY";
        public const string ERROR_SERVICE = "ERROR_SERVICE";
    }
}
