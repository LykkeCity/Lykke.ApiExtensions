namespace Lykke.ApiExtensions
{
    public static class ErrorCodes
    {
        public const int InvalidInputField = 0;

        /// <summary>
        /// Returns, when request is being invoked but it should not be invoked acording to the current status
        /// </summary>
        public const int InconsistentData = 1;
        public const int NotAuthenticated = 2;
        public const int InvalidUsernameOrPassword = 3;
        public const int AssetNotFound = 4;
        public const int NotEnoughFunds = 5;
        public const int VersionNotSupported = 6;
        public const int RuntimeProblem = 7;
        public const int WrongConfirmationCode = 8;
        public const int BackupWarning = 9;
        public const int BackupRequired = 10;
        public const int MaintananceMode = 11;
        public const int NoData = 12;
        public const int ShouldOpenNewChannel = 13;
        public const int ShouldProvideNewTempPubKey = 14;
        public const int ShouldProcesOffchainRequest = 15;
        public const int NoOffchainLiquidity = 16;
    }
}
