using System;
using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Application.Converter
{
    class HeartRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return parameter.ToString() == "min"
                ? ((double)value >= 250) ? "--" : value.ToString()
                : ((double)value <= 0) ? "--" : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
