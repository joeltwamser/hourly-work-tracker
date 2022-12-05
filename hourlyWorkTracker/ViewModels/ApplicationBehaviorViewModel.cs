using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows;
using hourlyWorkTracker.Models;
using hourlyWorkTracker.Commands;
using System.Windows.Controls;

namespace hourlyWorkTracker.ViewModels
{
    public class ApplicationBehaviorViewModel : INotifyPropertyChanged
    {
        public ApplicationBehaviorViewModel()
        {
            Color myGreen = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#118C4F");
            _my_application_behavior = new ApplicationBehavior(myGreen, myGreen, myGreen, Colors.Black, Colors.Black, 1.0,
                25, false, 0.0, false);
        }

        protected ApplicationBehavior _my_application_behavior;
        protected ICommand? _open_configure_window;
        protected ICommand? _close_window;
        protected ICommand? _save_hourly_wage;
        protected ICommand? _save_total_money_made;

        public ApplicationBehavior MyApplicationBehavior
        {
            get { return _my_application_behavior; }
            set
            {
                _my_application_behavior = value;
                NotifyPropertyChanged("MyApplicationBehavior");
            }
        }

        protected bool GenericReturnTrue(object? parameter)
        {
            return true;
        }

        public ICommand OpenConfigureWindow
        {
            get
            {
                if (_open_configure_window == null)
                {
                    _open_configure_window = new Command(OpenConfigureWindowExecute, GenericReturnTrue);
                }
                return _open_configure_window;
            }
        }

        protected virtual void OpenConfigureWindowExecute(object? parameter)
        {
            if (parameter is Window a)
            {
                a.DataContext = this;
                a.Show();
            }
        }

        public ICommand CloseWindow
        {
            get
            {
                if (_close_window == null)
                {
                    _close_window = new Command(CloseWindowExecute, GenericReturnTrue);
                }
                return _close_window;
            }
        }

        protected virtual void CloseWindowExecute(object? parameter)
        {
            if (parameter is Window a)
                a.Close();
        }

        public ICommand SaveHourlyWage
        {
            get
            {
                if(_save_hourly_wage == null)
                {
                    _save_hourly_wage = new Command(SaveHourlyWageExecute, TextBoxCanExecute);
                }
                return _save_hourly_wage;
            }
        }

        public void SaveHourlyWageExecute(object? parameter)
        {
            if (parameter is TextBox tb)
            {
                MyApplicationBehavior.HourlyWageChanged = double.TryParse(tb.Text, out double result);
                if (MyApplicationBehavior.HourlyWageChanged)
                {
                    MyApplicationBehavior.HourlyWage = Math.Round(result, 2);
                    tb.Clear();
                }
                //MyApplicationBehavior.HourlyWageChanged = false;
            }
        }

        public ICommand SaveTotalMoneyMade
        {
            get
            {
                if (_save_total_money_made == null)
                {
                    _save_total_money_made = new Command(SaveTotalMoneyMadeExecute, TextBoxCanExecute);
                }
                return _save_total_money_made;
            }
        }

        public void SaveTotalMoneyMadeExecute(object? parameter)
        {
            if (parameter is TextBox tb)
            {
                MyApplicationBehavior.TotalMoneyMadeChanged = double.TryParse(tb.Text, out double result);
                if (MyApplicationBehavior.TotalMoneyMadeChanged)
                {
                    MyApplicationBehavior.TotalMoneyMade = Math.Round(result, 2);
                    tb.Clear();
                }
                //MyApplicationBehavior.TotalMoneyMadeChanged = false;
            }
        }

        public bool TextBoxCanExecute(object? parameter)
        {
            return parameter is TextBox tb && !string.IsNullOrEmpty(tb.Text);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
