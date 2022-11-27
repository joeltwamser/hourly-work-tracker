using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using hourlyWorkTracker.Models;
using hourlyWorkTracker.Commands;
using System.Windows;

namespace hourlyWorkTracker.ViewModels
{
    public class ApplicationBehaviorViewModel : INotifyPropertyChanged
    {
        public ApplicationBehaviorViewModel()
        {
            SolidColorBrush? holder = new BrushConverter().ConvertFrom("#118C4F") as SolidColorBrush;
            if (holder == null)
            {
                holder = new SolidColorBrush(Colors.DarkGreen);
            }
            _my_application_behavior = new ApplicationBehavior(holder, holder, holder, new SolidColorBrush(Colors.Black),
                new FontFamily("Global User Interface"), 1.0);
        }

        private ApplicationBehavior _my_application_behavior;
        private ICommand? _open_configure_window;
        private ICommand? _close_application;

        public ApplicationBehavior MyApplicationBehavior
        {
            get { return _my_application_behavior; }
            set
            {
                _my_application_behavior = value;
                NotifyPropertyChanged("MyApplicationBehavior");
            }
        }

        public ICommand OpenConfigureWindow
        {
            get
            {
                if (_open_configure_window == null)
                {
                    _open_configure_window = new Command(OpenConfigureWindowExecute, CanOpenConfigureWindowExecute);
                }
                return _open_configure_window;
            }
        }

        private void OpenConfigureWindowExecute(object? parameter)
        {
            if (parameter is Window a)
            {
                a.DataContext = this;
                a.Show();
            }
        }

        //This will probably have conditions at some point
        private bool CanOpenConfigureWindowExecute(object? parameter)
        {
            return true;
        }

        public ICommand CloseApplication
        {
            get
            {
                if (_close_application == null)
                {
                    _close_application = new Command(CloseApplicationExecute, CanCloseApplicationExecute);
                }
                return _close_application;
            }
        }

        private void CloseApplicationExecute(object? parameter)
        {
            if (parameter is Window a)
                a.Close();
        }

        //This will probably have conditions at some point
        private bool CanCloseApplicationExecute(object? parameter)
        {
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
