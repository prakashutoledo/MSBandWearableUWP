namespace IDEASLabUT.MSBandWearable.Core.Model
{
    /// <summary>
    /// POCO holding MS Band 2 gyroscope sensor event details
    /// </summary>
    public class GyroscopeEvent : BaseEvent
    {
        /// <summary>
        /// An angular velocity value in X direction
        /// </summary>
        public double AngularX { get; set; }

        /// <summary>
        /// An angular velocity value in Y direction
        /// </summary>
        public double AngularY { get; set; }

        /// <summary>
        /// An angular velocity value in Z direction
        /// </summary>
        public double AngularZ { get; set; }
    }
}
