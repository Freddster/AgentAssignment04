using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MvvmFoundation.Wpf;

namespace ColorConverterNs
{
    public class ColorConverters : INotifyPropertyChanged
    {

        private ICommand _colorCommand;
        public ICommand ColorCommand
        {
            get { return _colorCommand ?? (_colorCommand = new RelayCommand<string>(ColorBackgroundExecute)); }
        }

        private void ColorBackgroundExecute(string newColor)
        {
            SolidColorBrush newBrush = SystemColors.WindowBrush;    //Sætter default color på brush

            try
            {
                if (newColor != null)   //Hvis der ikke bliver givet nogen string med, så spring over
                {
                    if (newColor != "Default")  //Der er ikke nogen grund til at sætte farven to gange, så derfor spriner vi over
                    {
                        newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(newColor));
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unknown color name, default color is used", "Program error!");
            }

            Application.Current.MainWindow.Resources["BackgroundBrush"] = newBrush;
            //OnPropertyChanged();
        }

        /**/
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}