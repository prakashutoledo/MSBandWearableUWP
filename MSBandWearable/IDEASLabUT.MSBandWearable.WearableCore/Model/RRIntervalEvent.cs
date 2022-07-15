/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding MS Band 2 rr interval or ibi sensor event details
    /// </summary>
    public class RRIntervalEvent : BaseEvent
    {
        /// <summary>
        /// An inter beats interval value
        /// </summary>
        public double Ibi { get; set; }
    }
}
