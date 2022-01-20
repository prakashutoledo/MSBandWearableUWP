using System;
using Windows.UI.Xaml.Data;

namespace IDEASLabUT.MSBandWearable.Application.Converter
{
    public class GsrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                object parameter, string language)
        {
            return string.Format("{0,-10:0.######}", System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
