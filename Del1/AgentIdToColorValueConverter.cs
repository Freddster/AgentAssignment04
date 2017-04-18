using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Diagnostics;
using System.Globalization;

namespace AgentAssignment
{
    class AgentIdToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string id = value as string;
            if (id == null)
                id = "";
            return (id == "007" ? Brushes.Blue : Brushes.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
