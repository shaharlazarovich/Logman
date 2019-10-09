namespace Logman.Common.Code
{
    public static class Constants
    {
        public const string DefaultConnectionStringName = "Default";
        public const string RouteEventVersion1 = "api/events/v1";
        public const string LayoutViewModelName = "LayoutViewModel";

        #region HttpMessages

        public static class ApiResponseCodes
        {
            public const int OK = 0;
        }

        public static class HttpMessages
        {
            public const string NoEntryWasPassed = "No log or event entry was passed.";
        }


        public static class SpecialValues
        {
            public const long InvalidId = -1;
            public const int MaxRowCount = 500;
            public const int PageSize = 15;
            public const string SessionPrefix = "sessionKey";
            public const string CurrentUserSessionKey = "currentUser";
            public const int AlertDefaultPeriodValue = 7;
            public const string AuthCookieName = "LogmanAuthCookieName";
        }
        #endregion
    }
}