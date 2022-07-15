/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using IDEASLabUT.MSBandWearable.Json;

using System.Text.Json.Serialization;

using static IDEASLabUT.MSBandWearable.Extension.JsonStringExtension;

namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An Empatica E4 Band device details POCO
    /// </summary>
    public class EmpaticaE4Band : IPayload
    {
        /// <summary>
        /// An E4Band payload type
        /// </summary>
        [JsonConverter(typeof(PayloadTypeConverter))]
        public PayloadType PayloadType { get; } = PayloadType.E4Band;

        /// <summary>
        /// A subject id of the subject wearing this E4 band
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// A current SwiftUI view of iPad used by subject
        /// </summary>
        public string FromView { get; set; }

        /// <summary>
        /// And Empatica E4 band device details 
        /// </summary>
        public Device Device { get; set; }

        /// <summary>
        /// Returns a serialized json representation of <see cref="BaseMessage"/>
        /// </summary>
        /// <returns>A serialized json string</returns>
        public override string ToString()
        {
            return this.ToJson();
        }
    }
}
