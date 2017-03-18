using System;

namespace Azure.RestApi.Tests
{
    public static class FakeData
    {
        public const string QUEUE = "queue";
        public const string TABLE = "table";
        public const string BLOB = "blob";

        public const string STORAGEKEY = "SSjwpaQxpuE39Ge1rtQSFG2d+SuyCdLfBavFFvWkgGuiJ3snVmHN7YNYsd1rQ6FVlco2GzCbfkwOplcrHJTgNA==";
        public const string ACCOUNTNAME = "testaccount";

        public static Uri GetBaseUrl(string containerType)
        {
            return new Uri($"https://{ACCOUNTNAME}.{containerType}.core.windows.net/");
        }
    }
}
