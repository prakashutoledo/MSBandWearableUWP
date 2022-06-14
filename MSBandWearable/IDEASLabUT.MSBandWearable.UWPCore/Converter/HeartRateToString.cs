using System;

using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Converter
{
    /// <summary>
    /// A converter for converting MS Band HeartRate sensor value to show "--" for heartrate value <= 0 || >= 220
    /// </summary>
    public class HeartRateToString : IValueConverter
    {
        /// <summary>
        /// Converts the given HeartRate value to given target type
        /// </summary>
        /// <param name="value">a HeartRate value to be converted</param>
        /// <param name="targetType">a target type to convert the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted HeartRate value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return parameter.ToString() == "min"
                ? ((double) value >= 220) ? "--" : value.ToString()
                : ((double) value <= 0) ? "--" : value.ToString();
        }

        /// <summary>
        /// Convert back the given converted HeartRate value to given target type. 
        /// Currently not supported converting back to original heartrate values
        /// </summary>
        /// <param name="value">a heartrate value to be converted back</param>
        /// <param name="targetType">a target type to convert the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted back to original HeartRate value</returns>
        /// <exception cref="NotImplementedException">Not implemented yet</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
