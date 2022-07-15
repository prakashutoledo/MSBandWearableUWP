/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
namespace IDEASLabUT.MSBandWearable.Model
{
    /// <summary>
    /// POCO holding date time model mapper value used in line charts
    /// </summary>
    public class DateTimeModel
    {
        /// <summary>
        /// A date time stamp for current value
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// A value of the model
        /// </summary>
        public double Value { get; set; }
    }
}
