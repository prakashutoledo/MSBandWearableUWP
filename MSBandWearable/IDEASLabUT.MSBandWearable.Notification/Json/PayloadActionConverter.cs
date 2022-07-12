using IDEASLabUT.MSBandWearable.Model.Notification;

namespace IDEASLabUT.MSBandWearable.Json
{
    /// <summary>
    /// A custom json enum converter for <see cref="PayloadAction"/>
    /// </summary>
    public class PayloadActionConverter : BaseEnumJsonConverter<PayloadAction>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PayloadActionConverter"/>
        /// </summary>
        public PayloadActionConverter() : base(description => description.ToPayloadAction(), payloadAction => payloadAction.GetDescription())
        {
        }
    }
}
