using SkillProfi_Shared;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SkillProfi_WPFClient.Classes
{
    public class TextWhishOutHTMLConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value==null ? string.Empty : SkillProfiHelper.GetStringWithOutHtml(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && System.Convert.ToString(parameter).Equals("invert", StringComparison.OrdinalIgnoreCase))
                return System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
            else
                return System.Convert.ToBoolean(value) ? Visibility.Visible: Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class NullStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }    
    public class NoCorrectEmalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ||
                   SkillProfiHelper.CheckStringByRegexPattern(value as string, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z") 
                   ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class BidStatusEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value!=null ? SkillProfiHelper.BidStatusGetName((BidStatusEnum)value) : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class BidStatusEnumToClolorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value !=null)
                switch ((BidStatusEnum)value)
                {
                    case BidStatusEnum.at_work:
                        return Brushes.LightBlue;//0xcfe2ff; 
                    case BidStatusEnum.cancelled:
                        return Brushes.LightYellow;//0xfff3cd;
                    case BidStatusEnum.completed:
                        return Brushes.LightGreen;//0xd1e7dd;
                    //case BidStatusEnum.received:
                    //    return Color.Transparent;
                    case BidStatusEnum.rejected:
                        return Brushes.LightPink;//0xf8d7da;
                }
            return DependencyProperty.UnsetValue; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class NullStringToParametrStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()?.Trim()) && parameter != null)
                return parameter;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
