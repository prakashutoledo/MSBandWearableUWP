﻿/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable
{
    /// <summary>
    /// Application global constant for MS Band Wearable application
    /// </summary>
    public static class MSBandWearableApplicationGlobals
    {
        public const string ApplicationPropertiesFileName = "ApplicationProperties.json";
        public const string LocalApplicationPropertiesFileName = "ApplicationProperties.local.json";
        public const string ElasticsearchUriJsonKey = "elasticsearch:uri";
        public const string ElasticsearchAuthenticationJsonKey = "elasticsearch:authenticationKey";
        public const string WebSocketConnectionUriJsonKey = "webSocket:connection:uri";
        public const string NtpPoolUriJsonKey = "ntp:pool:uri";
        public const string LoggerFileUriJsonKey = "logger:file:uri";
    }
}
