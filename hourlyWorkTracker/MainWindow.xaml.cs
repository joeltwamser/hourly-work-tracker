using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //fields
        private bool resize_window_in_process;
        private const string _starting_money = "$0.00";
        private System.Timers.Timer _timer;
        private Stopwatch _stopwatch;
        private bool _running = false;
        public MainWindow()
        {
            InitializeComponent();
            Opacity = ApplicationSettingsStatic.MainWindowOpacity;
            ApplicationSettingsStatic.TotalMoneyChanged += onTotalMoneyChanged;
            CurrentMoneyDisplay.Text = _starting_money;
            TotalMoneyDisplay.Text = "$" + ApplicationSettingsStatic.TotalMoney.ToString("F2");

            _stopwatch = new Stopwatch();
            _timer = new System.Timers.Timer(10);
            _timer.Elapsed += onTimerElapse;
        }

        private void onTimerElapse(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                double money_made_this_session = _stopwatch.Elapsed.TotalHours * ApplicationSettingsStatic.HourlyWage;
                CurrentMoneyDisplay.Text = "$" + money_made_this_session.ToString("F2");
                TotalMoneyDisplay.Text = "$" + (ApplicationSettingsStatic.TotalMoney + money_made_this_session).ToString("F2");
            });
        }

        private void onStartStopSessionClick(object sender, RoutedEventArgs e)
        {
            if (!_running)
            {
                _stopwatch.Start();
                _timer.Start();
                StartStopSessionButton.Content = "Stop";
                StartStopSessionButton.FontWeight = FontWeights.Bold;
                ResetSessionButton.IsEnabled = false;
                _running = true;
            }
            else
            {
                _stopwatch.Stop();
                _timer.Stop();
                StartStopSessionButton.Content = "Start";
                StartStopSessionButton.FontWeight = FontWeights.Bold;
                ResetSessionButton.IsEnabled = true;
                _running = false;
            }
        }

        private void onResetSessionClick(object sender, RoutedEventArgs e)
        {
            _stopwatch.Reset();
            CurrentMoneyDisplay.Text = _starting_money;
            saveTotalMoney();
        }

        private void onTotalMoneyChanged(object? sender, EventArgs e)
        {
            TotalMoneyDisplay.Text = "$" + ApplicationSettingsStatic.TotalMoney.ToString("F2");
        }

        private void resizeWindowBegin(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            e.Handled = true;
            Rectangle? sender_rectangle = sender as Rectangle;
            if (sender_rectangle != null)
            {
                resize_window_in_process = true;
                sender_rectangle.CaptureMouse();
            }
        }

        private void resizeWindowEnd(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            e.Handled = true;
            Rectangle? sender_rectangle = sender as Rectangle;
            if (sender_rectangle != null)
            {
                resize_window_in_process = false;
                sender_rectangle.ReleaseMouseCapture();
            }
        }

        private void resizeWindow(object sender, MouseEventArgs e)
        {
            if(resize_window_in_process)
            {
                Rectangle? sender_rectangle = sender as Rectangle;
                if(sender_rectangle != null)
                {
                    Window? main_window = sender_rectangle.Tag as Window;
                    if (main_window == null)
                        return;
                    double width = e.GetPosition(main_window).X;
                    double height = e.GetPosition(main_window).Y;
                    double temp_width;
                    double temp_height;
                    sender_rectangle.CaptureMouse();
                    if (sender_rectangle.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                            main_window.Width = width;
                    }
                    if (sender_rectangle.Name.ToLower().Contains("left"))
                    {
                        width -= 5;
                        temp_width = width;
                        width = main_window.Width - width;
                        if (width > 0)
                        {
                            main_window.Width = width;
                            main_window.Left += temp_width;
                        }
                    }
                    if (sender_rectangle.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0)
                            main_window.Height = height;
                    }
                    if (sender_rectangle.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        temp_height = height;
                        height = main_window.Height - height;
                        if (height > 0)
                        {
                            main_window.Height = height;
                            main_window.Top += temp_height;
                        }
                    }
                }
            }
        }

        private void clickedWindow(object sender, MouseButtonEventArgs e)
        {
            if (resize_window_in_process)
                return;
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void openConfigureWindow(object sender, RoutedEventArgs e)
        {
            if (_running)
            {
                MessageBox.Show("Please pause the timer before configuration.");
            }
            else
            {
                ConfigureWindow my_configure_window = new ConfigureWindow();
                my_configure_window.ShowDialog();
            }
        }

        private void onClosed(object sender, EventArgs e)
        {
            saveTotalMoney();
        }

        private void saveTotalMoney()
        {
            double temp;
            bool success = double.TryParse(TotalMoneyDisplay.Text.Trim('$'), out temp);
            if (success)
            {
                ApplicationSettingsStatic.TotalMoney = temp;
            }
        }
    }
}
