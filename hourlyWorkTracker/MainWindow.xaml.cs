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
            CurrentMoneyDisplay.FontWeight = FontWeights.Bold;
            TotalMoneyDisplay.Text = "$" + ApplicationSettingsStatic.TotalMoney.ToString("F2");
            TotalMoneyDisplay.FontWeight = FontWeights.Bold;
            

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
                    double calculated_height = 0.0;
                    bool entered_right_or_left = false;
                    bool entered_top_or_bottom = false;
                    double ratio = 16.0 / 9.0;
                    sender_rectangle.CaptureMouse();

                    if (sender_rectangle.Name.ToLower().Contains("right"))
                    {
                        width += 5;
                        if (width > 0)
                        {
                            main_window.Width = width;
                            calculated_height = width / ratio;
                        }
                        entered_right_or_left = true;
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
                            calculated_height = width / ratio;
                        }
                        entered_right_or_left = true;
                    }
                    if (sender_rectangle.Name.ToLower().Contains("bottom"))
                    {
                        height += 5;
                        if (height > 0 && !entered_right_or_left)
                        {
                            main_window.Height = height;
                            main_window.Width = height * ratio;
                        }
                        else if (height > 0 && entered_right_or_left)
                            main_window.Height = calculated_height;
                        entered_top_or_bottom = true;
                    }
                    if (sender_rectangle.Name.ToLower().Contains("top"))
                    {
                        height -= 5;
                        if (entered_right_or_left && calculated_height > 0)
                        {
                            temp_height = main_window.Height;
                            main_window.Height = calculated_height;
                            main_window.Top += (temp_height - calculated_height);
                        }
                        else if (!entered_right_or_left)
                        {
                            temp_height = height;
                            height = main_window.Height - height;
                            if (height > 0)
                            {
                                main_window.Height = height;
                                main_window.Top += temp_height;
                                main_window.Width = height * ratio;
                            }
                        }
                        entered_top_or_bottom = true;
                    }
                    if (entered_right_or_left && !entered_top_or_bottom)
                    {
                        main_window.Height = calculated_height;
                    }
                    ContainerCanvas.Height = main_window.Height;
                    ContainerCanvas.Width = main_window.Width;
                    MainWindowGrid.Height = ContainerCanvas.Height;
                    MainWindowGrid.Width = ContainerCanvas.Width;

                    //There's GOT to be a better way than this to dynamically
                    //resize elements based on the size of a parent element.
                    //I'm for sure doing this wrong.
                    //I already know I could replace these numbers with variables,
                    //eliminate most of the definitions from the UI, and programmatically
                    //define these in the constructor probably.  That'd probably be at least
                    //better since I'm already doing it this way.
                    CurrentMoneyDisplay.FontSize = main_window.Width / 8;
                    TotalMoneyDisplay.FontSize = (main_window.Width / 8) * 0.6;
                    StartStopSessionButton.FontSize = main_window.Width / 30.77;
                    ResetSessionButton.FontSize = main_window.Width / 30.77;
                    StartStopSessionButton.Height = main_window.Width / 11.43;
                    ResetSessionButton.Height = main_window.Width / 11.43;
                    StartStopSessionButton.Width = main_window.Width / 4;
                    ResetSessionButton.Width = main_window.Width / 4;
                    ResetSessionTextBlock.LineHeight = main_window.Width / 29.63;
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

        private void saveTotalMoney()
        {
            double temp;
            bool success = double.TryParse(TotalMoneyDisplay.Text.Trim('$'), out temp);
            if (success)
            {
                ApplicationSettingsStatic.TotalMoney = temp;
            }
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            Point current_money_display_location = CurrentMoneyDisplay.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            current_money_display_location.X += CurrentMoneyDisplay.ActualWidth;
            Canvas.SetLeft(MoneyEffect, current_money_display_location.X);
            Canvas.SetTop(MoneyEffect, current_money_display_location.Y);
        }

        private void onClosing(object sender, EventArgs e)
        {
            if (_running)
            {
                onStartStopSessionClick(StartStopSessionButton, new RoutedEventArgs());
            }
            saveTotalMoney();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
