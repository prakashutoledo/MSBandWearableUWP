using System;
using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Application.Converter
{
    class HeartRateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                object parameter, string language)
        {
            if (parameter.ToString() == "min")
            {
                return ((double)value >= 250) ? "--" : value.ToString();
            }
            else
            {
                return ((double)value <= 0) ? "--" : value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
