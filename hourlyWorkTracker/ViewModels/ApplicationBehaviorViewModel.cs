using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using hourlyWorkTracker.Models;
using hourlyWorkTracker.Commands;
using System.Windows.Controls;
using hourlyWorkTracker.Views;
using System.Windows;
using System.Timers;
using System.Diagnostics;

namespace hourlyWorkTracker.ViewModels
{
    public class ApplicationBehaviorViewModel : INotifyPropertyChanged
    {
        public ApplicationBehaviorViewModel()
        {
            Color myGreen = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#118C4F");
            _my_application_behavior = new ApplicationBehavior(myGreen, myGreen, myGreen, Colors.Black, Colors.Black, 1.0,
                25, false, 0.0, false, 0.0, false, "Start");

            _start_stop_timer = new Timer(10);
            _start_stop_timer.Elapsed += OnTimerElapse;
            _stopwatch = new Stopwatch();
            _money_made_this_session_holder = 0;
            _total_money_made_holder = 0;
        }

        protected ApplicationBehavior _my_application_behavior;
        protected ICommand? _open_configure_window;
        protected ICommand? _close_window;
        protected ICommand? _save_hourly_wage;
        protected ICommand? _save_total_money_made;
        protected ICommand? _start_stop_tracker;
        protected ICommand? _reset_and_store;
        protected Timer _start_stop_timer;
        protected Stopwatch _stopwatch;
        protected double _money_made_this_session_holder;
        protected double _total_money_made_holder;

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

        //Maybe eventually make this general to just (OpenWindow)
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

        protected void OpenConfigureWindowExecute(object? parameter)
        {
            Window w = new ConfigureView
            {
                DataContext = this
            };
            w.Show();
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

        protected void CloseWindowExecute(object? parameter)
        {
            if (parameter is Window w)
            {
                w.Close();
            }
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

        protected void SaveHourlyWageExecute(object? parameter)
        {
            if (parameter is TextBox tb)
            {
                MyApplicationBehavior.HourlyWageChanged = double.TryParse(tb.Text, out double result);
                if (MyApplicationBehavior.HourlyWageChanged)
                {
                    MyApplicationBehavior.HourlyWage = Math.Round(result, 2);
                    tb.Clear();
                }
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

        protected void SaveTotalMoneyMadeExecute(object? parameter)
        {
            if (parameter is TextBox tb)
            {
                MyApplicationBehavior.TotalMoneyMadeChanged = double.TryParse(tb.Text, out double result);
                if (MyApplicationBehavior.TotalMoneyMadeChanged)
                {
                    MyApplicationBehavior.TotalMoneyMade = Math.Round(result, 2);
                    tb.Clear();
                }
            }
        }

        protected bool TextBoxCanExecute(object? parameter)
        {
            return parameter is TextBox tb && !string.IsNullOrEmpty(tb.Text);
        }

        public ICommand StartStopTracker
        {
            get
            {
                if (_start_stop_tracker == null)
                {
                    _start_stop_tracker = new Command(StartStopTrackerExecute, GenericReturnTrue);
                }
                return _start_stop_tracker;
            }
        }

        protected void StartStopTrackerExecute(object? parameter)
        {
            if (!MyApplicationBehavior.TrackerRunning)
            {
                MyApplicationBehavior.TrackerRunning = true;
                MyApplicationBehavior.StartStopButtonText = "Stop";
                _money_made_this_session_holder = MyApplicationBehavior.MoneyMadeThisSession;
                _total_money_made_holder = MyApplicationBehavior.TotalMoneyMade;
                _start_stop_timer.Start();
                _stopwatch.Start();
            }
            else
            {
                MyApplicationBehavior.TrackerRunning = false;
                MyApplicationBehavior.StartStopButtonText = "Start";
                _start_stop_timer.Stop();
                _stopwatch.Stop();
                _stopwatch.Reset();
            }
        }

        public ICommand ResetAndStore
        {
            get
            {
                if (_reset_and_store == null)
                {
                    _reset_and_store = new Command(ResetAndStoreExecute, ResetAndStoreCanExecute);
                }
                return _reset_and_store;
            }
        }

        //Here we will have to 0 out MyApplicationBehavior.MoneyMadeThisSession
        //and _money_made_this_session_holder
        protected void ResetAndStoreExecute(object? parameter)
        {

        }

        protected bool ResetAndStoreCanExecute(object? parameter)
        {
            if (MyApplicationBehavior.TrackerRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void OnTimerElapse(object? sender, ElapsedEventArgs e)
        {
            MyApplicationBehavior.MoneyMadeThisSession = Math.Round((_stopwatch.Elapsed.TotalHours * MyApplicationBehavior.HourlyWage) + _money_made_this_session_holder, 2);
            MyApplicationBehavior.TotalMoneyMade = Math.Round((_stopwatch.Elapsed.TotalHours * MyApplicationBehavior.HourlyWage) + _total_money_made_holder, 2);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
