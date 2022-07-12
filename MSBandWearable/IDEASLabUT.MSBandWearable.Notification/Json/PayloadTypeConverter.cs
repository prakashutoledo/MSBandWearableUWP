using IDEASLabUT.MSBandWearable.Model.Notification;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="PayloadType"/>
    /// </summary>
    public class PayloadTypeConverter : BaseEnumJsonConverter<PayloadType>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PayloadTypeConverter"/>
        /// </summary>
        public PayloadTypeConverter() : base(description => description.ToPayloadType(), payloadType => payloadType.GetDescription())
        {
        }
    }
}
