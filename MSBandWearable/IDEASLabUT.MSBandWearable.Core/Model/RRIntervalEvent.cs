namespace IDEASLabUT.MSBandWearable.Core.Model
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
