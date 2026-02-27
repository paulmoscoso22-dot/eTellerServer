namespace eTeller.Domain.Common
{
    public static class UserStatusConstants
    {
        public const string Enabled = "ENABLED";
        public const string Disabled = "DISABLED";
        public const string Blocked = "BLOCKED";
        public const string PendingActivation = "PENDING_ACTIVATION";
    }

    public static class AuthenticationResultConstants
    {
        public const string Success = "OK";
        public const string UserAlreadyLogged = "USER_LOGGED";
        public const string CashDeskOccupied = "CLI_OKKUPATO";
        public const string InvalidCredentials = "INVALID_CREDENTIALS";
        public const string UserBlocked = "USER_BLOCKED";
        public const string UserNotFound = "USER_NOT_FOUND";
        public const string PasswordExpired = "PASSWORD_EXPIRED";
        public const string PasswordChangeRequired = "PASSWORD_CHANGE_REQUIRED";
    }

    public static class PasswordPolicyConstants
    {
        public const int MinimumLength = 8;
        public const int MinimumDigits = 2;
        public const int MaxFailedAttempts = 3;
        public const int PasswordHistoryCount = 5;
    }

    public static class ErrorCodes
    {
        public const string UserNotExists = "1320";
        public const string UserBlocked = "1306";
        public const string InvalidCredentials = "9003";
        public const string UserNotEnabled = "9004";
        public const string BranchMismatch = "9005";
        public const string AlreadyLoggedWarning = "9009";
        public const string PasswordChangeError = "9010";
        public const string MacAddressError = "9012";
        public const string UserAlreadyLogged = "9013";
        public const string CashDeskOccupied = "9014";
        public const string GenericLoginError = "9015";
        public const string AlreadyLoggedInfo = "9020";
        public const string PasswordExpired = "9021";
        public const string CashDeskNotEnabled = "9002";
        public const string NoPrinterFiche = "1025";
        public const string MultiplePrintersFiche = "1027";
        public const string NbtstatError = "9011";
    }

    public static class CashDeskStatusConstants
    {
        public const string Enabled = "ENABLED";
        public const string Disabled = "DISABLED";
    }
}
