namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding MS Band 2 heart rate sensor event details
    /// </summary>
    public class HeartRateEvent : BaseEvent
    {
        /// <summary>
        /// A heart rate beats per minute value
        /// </summary>
        public double Bpm { get; set; }
    }
}
