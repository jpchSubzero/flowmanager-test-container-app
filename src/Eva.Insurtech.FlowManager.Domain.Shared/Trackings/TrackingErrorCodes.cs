using Eva.Framework.Utility.Response.Models;
using Eva.Insurtech.FlowManagers.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eva.Insurtech.FlowManagers.Trackings
{
    public static class TrackingErrorCodes
    {
        private static IStringLocalizer<FlowManagerResource> _localizer;
        private static string entity;

        public static void InitStringLocalizer(IStringLocalizer<FlowManagerResource> localizer)
        {
            _localizer = localizer;
            entity = _localizer.GetString(LabelConsts.TRACKING);
        }

        public static Error GetErrorGeneral()
        {
            return new Error() { Code = ErrorConsts.ERROR_GENERAL, Message = _localizer.GetString(ErrorConsts.ERROR_GENERAL) };
        }

        public static Error GetErrorContextAlreadyExists()
        {
            return new Error() { Code = ErrorConsts.ERROR_ALREADY_EXIST_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_ALREADY_EXIST_CODE, _localizer.GetString(LabelConsts.CONTEXT)) };
        }

        public static Error GetErrorAlreadyExists()
        {
            return new Error() { Code = ErrorConsts.ERROR_ALREADY_EXIST_CODE, Message = _localizer.GetString(ErrorConsts.ERROR_ALREADY_EXIST_CODE, entity) };
        }

        public static Error GetErrorNotFoundById()
        {
            return new Error() { Code = ErrorConsts.ERROR_NOT_FOUND_BY_ID, Message = _localizer.GetString(ErrorConsts.ERROR_NOT_FOUND_BY_ID, entity) };
        }

        public static Error GetErrorExtraPropertiesNull()
        {
            return new Error() { Code = ErrorConsts.ERROR_NULL_EXTRA_PROPERTIES, Message = _localizer.GetString(ErrorConsts.ERROR_NULL_EXTRA_PROPERTIES, entity) };
        }

        public static Error GetErrorLastStep(string name)
        {
            return new Error() { Code = ErrorConsts.ERROR_CLOSE_ON_LAST_STEP, Message = _localizer.GetString(ErrorConsts.ERROR_CLOSE_ON_LAST_STEP, name) };
        }

        public static Error GetErrorIncorrectStartStep(string incorrect, string correct, int incorrectActionOption, int correctActionOption)
        {
            string incorrectAction = GetAction(incorrectActionOption);
            string correctAction = GetAction(correctActionOption);
            return new Error() { Code = ErrorConsts.ERROR_INCORRECT_START_STEP, Message = _localizer.GetString(ErrorConsts.ERROR_INCORRECT_START_STEP, incorrectAction, incorrect, correctAction, correct) };
        }

        public static Error GetErrorIncorrectEndStep(string incorrect, string correct, int incorrectActionOption, int correctActionOption)
        {
            string incorrectAction = GetAction(incorrectActionOption);
            string correctAction = GetAction(correctActionOption);
            return new Error() { Code = ErrorConsts.ERROR_INCORRECT_END_STEP, Message = _localizer.GetString(ErrorConsts.ERROR_INCORRECT_END_STEP, incorrectAction, incorrect, correctAction, correct) };
        }

        public static Error GetErrorNotEnoughToBackward()
        {
            return new Error() { Code = ErrorConsts.ERROR_INCORRECT_BACKWARD_STEPS, Message = _localizer.GetString(ErrorConsts.ERROR_INCORRECT_BACKWARD_STEPS) };
        }

        public static Error GetErrorNotEnoughToForeward()
        {
            return new Error() { Code = ErrorConsts.ERROR_INCORRECT_FOREWARD_STEPS, Message = _localizer.GetString(ErrorConsts.ERROR_INCORRECT_FOREWARD_STEPS) };
        }

        public static Error GetErrorStepNotFoundOnFlow(string incorrect, int incorrectActionOption, string flowName)
        {
            string incorrectAction = GetAction(incorrectActionOption);
            return new Error() { Code = ErrorConsts.ERROR_STEP_NOT_FOUND_ON_FLOW, Message = _localizer.GetString(ErrorConsts.ERROR_STEP_NOT_FOUND_ON_FLOW, incorrectAction, incorrect, flowName) };
        }

        private static string GetAction(int actionOption)
        {
            return actionOption switch
            {
                TrackingConsts.START_LABEL => (string)_localizer.GetString(LabelConsts.START_LABEL),
                TrackingConsts.END_LABEL => (string)_localizer.GetString(LabelConsts.END_LABEL),
                TrackingConsts.CLOSE_LABEL => (string)_localizer.GetString(LabelConsts.CLOSE_LABEL),
                _ => throw new NotSupportedException(),
            };
        }

        public static Error GetErrorStepOutOfRange(int min, int max)
        {
            return new Error() { Code = ErrorConsts.ERROR_STEP_OUT_OF_RANGE, Message = _localizer.GetString(ErrorConsts.ERROR_STEP_OUT_OF_RANGE, min, max) };
        }

        public static Error GetErrorTimeoutTracking()
        {
            return new Error() { Code = ErrorConsts.ERROR_TRACKING_TIME_OUT, Message = _localizer.GetString(ErrorConsts.ERROR_TRACKING_TIME_OUT) };
        }

        public static Error GetErrorNotFoundByIp()
        {
            return new Error() { Code = ErrorConsts.ERROR_TRACKING_NOT_FOUND_BY_IP, Message = _localizer.GetString(ErrorConsts.ERROR_TRACKING_NOT_FOUND_BY_IP) };
        }

        public static Error GetErrorTrackingExcededCreationByDay()
        {
            return new Error() { Code = ErrorConsts.ERROR_TRACKING_EXCEDED_CREATION_BY_DAY, Message = _localizer.GetString(ErrorConsts.ERROR_TRACKING_EXCEDED_CREATION_BY_DAY) };
        }

        public static Error GetErrorPreTrackingCreate()
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_PRE_TRACKING_CREATE, 
                Message = _localizer.GetString(ErrorConsts.ERROR_PRE_TRACKING_CREATE) 
            };
        }

        public static Error GetErrorPreTrackingAddStep()
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_PRE_TRACKING_ADD_STEP, 
                Message = _localizer.GetString(ErrorConsts.ERROR_PRE_TRACKING_ADD_STEP) 
            };
        }

        public static Error GetErrorGetPreTracking(string criteria)
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_GET_PRE_TRACKING, 
                Message = _localizer.GetString(ErrorConsts.ERROR_GET_PRE_TRACKING, criteria) 
            };
        }

        public static Error GetErrorRequestLogCreate()
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_REQUEST_LOG_CREATE, 
                Message = _localizer.GetString(ErrorConsts.ERROR_REQUEST_LOG_CREATE) 
            };
        }

        public static Error GetErrorRequestLogAddStep()
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_REQUEST_LOG_ADD_STEP, 
                Message = _localizer.GetString(ErrorConsts.ERROR_REQUEST_LOG_ADD_STEP) 
            };
        }

        public static Error GetErrorGetRequestLog(string criteria)
        {
            return new Error() { 
                Code = ErrorConsts.ERROR_GET_REQUEST_LOG, 
                Message = _localizer.GetString(ErrorConsts.ERROR_GET_REQUEST_LOG, criteria) 
            };
        }
    }
}
