namespace IDEASLabUT.MSBandWearable.Core.Model.Notification
{
    /// <summary>
    /// An interface holding webSocket message <see cref="Notification.PayloadType"/>
    /// </summary>
    public interface IPayload
    {
        PayloadType PayloadType { get; }
    }
}
