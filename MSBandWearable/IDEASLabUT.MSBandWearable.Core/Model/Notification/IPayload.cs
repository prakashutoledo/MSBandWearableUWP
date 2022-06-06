namespace IDEASLabUT.MSBandWearable.Model.Notification
{
    /// <summary>
    /// An interface holding webSocket message <see cref="Notification.PayloadType"/>
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Gets the payload type
        /// </summary>
        PayloadType PayloadType { get; }
    }
}
