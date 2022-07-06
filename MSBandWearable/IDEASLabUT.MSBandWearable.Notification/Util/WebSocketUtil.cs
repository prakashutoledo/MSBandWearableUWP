using IDEASLabUT.MSBandWearable.Model.Notification;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Extension.JsonStringExtension;
using static IDEASLabUT.MSBandWearable.Extension.TaskExtension;

namespace IDEASLabUT.MSBandWearable.Util
{
    public static class WebSocketUtil
    {
        private static readonly Lazy<IReadOnlyDictionary<PayloadType, Type>> NotificationTypeMapInstance;

        static WebSocketUtil()
        {
            NotificationTypeMapInstance = new Lazy<IReadOnlyDictionary<PayloadType, Type>>(() => new Dictionary<PayloadType, Type>
            {
                { PayloadType.E4Band, typeof(EmpaticaE4BandMessage) }
            });
        }

        public static IReadOnlyDictionary<PayloadType, Type> SupportedNotificationTypeMap => NotificationTypeMapInstance.Value;

        /// <summary>
        /// Serialize given webSocket raw json message and call the message received function
        /// </summary>
        /// <param name="message">A webSocket json message to be serialized</param>
        /// <param name="processors">A webSocket message post processors based on payload type</param>
        /// <returns>A boolean task that can be awaited</returns>
        public static async Task<bool> ParseMessageAndProcess(string message, IReadOnlyDictionary<PayloadType, Func<object, Task>> messagePostProcessors)
        {
            if (message == null || messagePostProcessors == null || messagePostProcessors.Count == 0)
            {
                return false;
            }

            var baseMessage = await message.FromJsonAsync<BaseMessage>().ConfigureAwait(false);
            
            if (baseMessage == null || !baseMessage.PayloadType.HasValue)
            {
                return false;
            }

            var payloadType = baseMessage.PayloadType.Value;
            if (!SupportedNotificationTypeMap.TryGetValue(payloadType, out Type notificationMessageType))
            {
                return false;
            };

            if (!messagePostProcessors.TryGetValue(payloadType, out var messagePostProcessor))
            {
                return false;
            }

            var websocketMessage = await message.FromJsonAsync(notificationMessageType).ConfigureAwait(false);
            return await messagePostProcessor.Invoke(websocketMessage).ContinueWithStatus().ConfigureAwait(false);
        }
    }
}
