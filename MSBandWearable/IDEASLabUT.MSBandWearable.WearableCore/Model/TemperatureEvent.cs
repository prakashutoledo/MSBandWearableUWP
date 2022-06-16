namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding MS Band 2 temperature sensor event details
    /// </summary>
    public class TemperatureEvent : BaseEvent
    {
        /// <summary>
        /// A temperature value in degree celsius (°C)
        /// </summary>
        public double Temperature { get; set; }
    }
}
