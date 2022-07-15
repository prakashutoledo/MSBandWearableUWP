/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding MS Band 2 accelerometer sensor event details
    /// </summary>
    public class AccelerometerEvent : BaseEvent
    {
        /// <summary>
        /// A linear acceleration value in X direction
        /// </summary>
        public double AccelerationX { get; set; }

        /// <summary>
        /// A linear acceleration value in Y direction
        /// </summary>
        public double AccelerationY { get; set; }

        /// <summary>
        /// A linear acceleration value in Z direction
        /// </summary>
        public double AccelerationZ { get; set; }
    }
}
