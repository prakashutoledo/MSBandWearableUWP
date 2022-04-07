using System;
using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Application.Converter
{
    /// <summary>
    /// A converter for converting MS Band GSR sensor value to max of 5 decimal places
    /// </summary>
    public class GSRConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given GSR value to given target type
        /// </summary>
        /// <param name="value">a GSR value to be converted</param>
        /// <param name="targetType">a target type to convert the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted GSR value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0,-10:0.######}", System.Convert.ToDouble(value));
        }

        /// <summary>
        /// Convert back the given converted GSR value to given target type. Currently not supported for GSR values
        /// </summary>
        /// <param name="value">a value to be converted back</param>
        /// <param name="targetType">a target type to convert back the given value</param>
        /// <param name="parameter">a parameter for given conversion</param>
        /// <param name="language">a supported language to convert the given value</param>
        /// <returns>A formatted and converted back to original GSR value</returns>
        /// <exception cref="NotImplementedException">Not implemented yet</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
