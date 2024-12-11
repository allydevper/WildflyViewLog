using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace WildflyViewLog.Converter
{
    public class FileNameTruncator : IValueConverter
    {
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo culture)
        {
            if (value is string fileName)
            {
                try
                {
                    int firstBackslashIndex = fileName.IndexOf('/');
                    int lastBackslashIndex = fileName.LastIndexOf('/');
                    string firstPart = fileName[..(firstBackslashIndex)];
                    string truncatedPath = string.Concat(firstPart, "/...", fileName.AsSpan(lastBackslashIndex));
                    return truncatedPath;
                }
                catch
                {
                    return fileName;
                }
            }

            return value;
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}