using System;

using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Converter
{
    /// <summary>
    /// A converter for converting MS Band temperature sensor value to max of 1 decimal place
    /// </summary>
    public class TemperatureToString : IValueConverter
    {
        /// <summary>
        /// Converts the given heartrate value to given target type to 1 decimal place
        /// </summary>
        /// <param name="value">a temperature value to be converted</param>
        /// <param name="targetType">a target type to convert the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted temperature value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0:0.#}", value);
        }

        /// <summary>
        /// Converts the given heartrate value to given target type to 1 decimal place
        /// </summary>
        /// <param name="value">a temperature value to be converted</param>
        /// <param name="targetType">a target type to convert the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted temperature value</returns>
        /// <exception cref="NotImplementedException">Not implemented yet</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}