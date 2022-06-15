namespace IDEASLabUT.MSBandWearable.Service
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceFactory
    {
        public static IWebSocketService GetWebSocketService => WebSocketService.Singleton;
        public static IBandClientService GetBandClientService => MSBandClientService.Singleton;
    }
}
