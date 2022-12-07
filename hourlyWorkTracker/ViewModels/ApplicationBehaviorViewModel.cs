using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using hourlyWorkTracker.Models;
using hourlyWorkTracker.Commands;
using System.Windows.Controls;
using hourlyWorkTracker.Views;
using System.Windows;
using System.Windows.Threading;
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

            _money_made_this_session_holder = 0;
            _total_money_made_holder = 0;

            _start_stop_timer = new DispatcherTimer(DispatcherPriority.Render);
            _start_stop_timer.Tick += OnTimerElapse;
            _start_stop_timer.Interval = TimeSpan.FromMilliseconds(10);
            _stopwatch = new Stopwatch();
            _hourly_wage_changed_timer = new DispatcherTimer(DispatcherPriority.Render);
            _hourly_wage_changed_timer.Tick += OnSaveTimerElapse;
            _hourly_wage_changed_timer.Interval = TimeSpan.FromSeconds(3);
            _total_money_made_changed_timer = new DispatcherTimer(DispatcherPriority.Render);
            _total_money_made_changed_timer.Tick += OnSaveTimerElapse;
            _total_money_made_changed_timer.Interval = TimeSpan.FromSeconds(3);

            _tracker_height = 450;
            _tracker_width = 800;
        }

        public ApplicationBehaviorViewModel(Color rectangle_fill, Color ticker_foreground, Color button_background,
            Color button_text_foreground, Color grid_background, double opacity, double hourly_wage,
            double total_money_made, double money_made_this_session)
        {
            _my_application_behavior = new ApplicationBehavior(rectangle_fill, ticker_foreground, button_background,
                button_text_foreground, grid_background, opacity, hourly_wage, false, total_money_made, false,
                money_made_this_session, false, "Start");

            _money_made_this_session_holder = 0;
            _total_money_made_holder = 0;

            _start_stop_timer = new DispatcherTimer(DispatcherPriority.Render);
            _start_stop_timer.Tick += OnTimerElapse;
            _start_stop_timer.Interval = TimeSpan.FromMilliseconds(10);
            _stopwatch = new Stopwatch();
            _hourly_wage_changed_timer = new DispatcherTimer(DispatcherPriority.Render);
            _hourly_wage_changed_timer.Tick += OnSaveTimerElapse;
            _hourly_wage_changed_timer.Interval = TimeSpan.FromSeconds(3);
            _total_money_made_changed_timer = new DispatcherTimer(DispatcherPriority.Render);
            _total_money_made_changed_timer.Tick += OnSaveTimerElapse;
            _total_money_made_changed_timer.Interval = TimeSpan.FromSeconds(3);

            _tracker_height = 450;
            _tracker_width = 800;
        }

        //Fields
        protected ApplicationBehavior _my_application_behavior;
        protected ICommand? _open_configure_window;
        protected ICommand? _close_window;
        protected ICommand? _save_hourly_wage;
        protected ICommand? _save_total_money_made;
        protected ICommand? _start_stop_tracker;
        protected ICommand? _reset_and_store;
        protected ICommand? _restore_defaults;
        private double _tracker_height;
        private double _tracker_width;

        protected double _money_made_this_session_holder;
        protected double _total_money_made_holder;

        //This timer info should probably be extracted to a class
        protected DispatcherTimer _start_stop_timer;
        protected Stopwatch _stopwatch;
        protected DispatcherTimer _hourly_wage_changed_timer;
        protected DispatcherTimer _total_money_made_changed_timer;

        public ApplicationBehavior MyApplicationBehavior
        {
            get { return _my_application_behavior; }
            set
            {
                _my_application_behavior = value;
                NotifyPropertyChanged("MyApplicationBehavior");
            }
        }

        public double TrackerHeight
        {
            get { return _tracker_height; }
            set
            {
                _tracker_height = value;
                NotifyPropertyChanged("TrackerHeight");
            }
        }

        public double TrackerWidth
        {
            get { return _tracker_width; }
            set
            {
                _tracker_width = value;
                NotifyPropertyChanged("TrackerWidth");
            }
        }

        public TimeSpan AnimationDuration
        {
            get
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(72 / MyApplicationBehavior.HourlyWage);
                return timeSpan;
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
            if (parameter is TrackerView)
            {
                Application.Current.Shutdown();
            }
            else if (parameter is Window w)
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
                    _hourly_wage_changed_timer.Start();
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
                    _total_money_made_changed_timer.Start();
                }
            }
        }

        protected bool TextBoxCanExecute(object? parameter)
        {
            return parameter is TextBox tb && !string.IsNullOrEmpty(tb.Text) && !MyApplicationBehavior.TrackerRunning;
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
            MyApplicationBehavior.MoneyMadeThisSession = 0;
            _money_made_this_session_holder = 0;
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

        public ICommand RestoreDefaults
        {
            get
            {
                if (_restore_defaults == null)
                {
                    _restore_defaults = new Command(RestoreDefaultsExecute, GenericReturnTrue);
                }
                return _restore_defaults;
            }
        }

        protected void RestoreDefaultsExecute(object? parameter)
        {
            Color myGreen = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#118C4F");
            MyApplicationBehavior.RectangleFill = myGreen;
            MyApplicationBehavior.TickerForeground = myGreen;
            MyApplicationBehavior.ButtonBackground = myGreen;
            MyApplicationBehavior.ButtonTextForeground = Colors.Black;
            MyApplicationBehavior.GridBackground = Colors.Black;
            MyApplicationBehavior.Opacity = 1.0;
            TrackerHeight = 450;
            TrackerWidth = 800;
        }

        private void OnTimerElapse(object? sender, EventArgs e)
        {
            MyApplicationBehavior.MoneyMadeThisSession = (_stopwatch.Elapsed.TotalHours * MyApplicationBehavior.HourlyWage) + _money_made_this_session_holder;
            MyApplicationBehavior.TotalMoneyMade = (_stopwatch.Elapsed.TotalHours * MyApplicationBehavior.HourlyWage) + _total_money_made_holder;
        }

        private void OnSaveTimerElapse(object? sender, EventArgs e)
        {
            if (sender is not DispatcherTimer t) { return; }
            if (t == _hourly_wage_changed_timer)
            {
                MyApplicationBehavior.HourlyWageChanged = false;
            }
            else if (t == _total_money_made_changed_timer)
            {
                MyApplicationBehavior.TotalMoneyMadeChanged = false;
            }
            t.Stop();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
