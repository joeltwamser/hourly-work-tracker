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
using System.Windows.Media.Animation;

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
        private DoubleAnimation _opacity_animation = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_X = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_Y = new DoubleAnimation();
        private DoubleAnimation _opacity_animation2 = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_X2 = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_Y2 = new DoubleAnimation();
        private Storyboard _money_effect_storyboard;
        private Storyboard _money_effect_storyboard2;
        internal double money_made_last_iteration = 0.0;
        private double radius = 75.0;
        private bool storyboard1 = true;


        public MainWindow()
        {
            InitializeComponent();
            Opacity = ApplicationSettingsStatic.MainWindowOpacity;
            ApplicationSettingsStatic.HourlyWageChanged += onHourlyWageChanged;
            ApplicationSettingsStatic.TotalMoneyChanged += onTotalMoneyChanged;
            CurrentMoneyDisplay.Text = _starting_money;
            CurrentMoneyDisplay.FontWeight = FontWeights.Bold;
            TotalMoneyDisplay.Text = "$" + ApplicationSettingsStatic.TotalMoney.ToString("F2");
            TotalMoneyDisplay.FontWeight = FontWeights.Bold;

            _money_effect_storyboard = new Storyboard();
            _money_effect_storyboard2 = new Storyboard();
            createMoneyEffectAnimation(_opacity_animation, _money_effect_path_animation_X, _money_effect_path_animation_Y, _money_effect_storyboard, MoneyEffect);
            createMoneyEffectAnimation(_opacity_animation2, _money_effect_path_animation_X2, _money_effect_path_animation_Y2, _money_effect_storyboard2, MoneyEffect2);

            _stopwatch = new Stopwatch();
            _timer = new System.Timers.Timer(10);
            _timer.Elapsed += onTimerElapse;
        }

        private void createMoneyEffectAnimation(DoubleAnimation opac_anim, DoubleAnimation x, DoubleAnimation y, Storyboard st, FrameworkElement fe)
        {
            opac_anim.From = 1.0;
            opac_anim.To = 0.0;
            opac_anim.Duration = new Duration(TimeSpan.FromSeconds(72 / ApplicationSettingsStatic.HourlyWage));
            x.Duration = new Duration(TimeSpan.FromSeconds(72 / ApplicationSettingsStatic.HourlyWage));
            y.Duration = new Duration(TimeSpan.FromSeconds(72 / ApplicationSettingsStatic.HourlyWage));

            st.Children.Add(opac_anim);
            st.Children.Add(x);
            st.Children.Add(y);
            Storyboard.SetTargetName(opac_anim, fe.Name);
            Storyboard.SetTargetProperty(opac_anim, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetName(x, fe.Name);
            Storyboard.SetTargetProperty(x, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTargetName(y, fe.Name);
            Storyboard.SetTargetProperty(y, new PropertyPath(Canvas.TopProperty));
        }

        private void onTimerElapse(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                double money_made_this_session = _stopwatch.Elapsed.TotalHours * ApplicationSettingsStatic.HourlyWage;
                if (Math.Round(money_made_this_session,2) > Math.Round(money_made_last_iteration,2))
                {
                    Point current_money_display_location = CurrentMoneyDisplay.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                    current_money_display_location.X += CurrentMoneyDisplay.ActualWidth;
                    Random random = new Random();
                    double angle = (double)random.Next(235,390) * Math.PI / 180.0;
                    Point destination = new Point(radius * Math.Cos(angle), radius * Math.Sin(angle));

                    if (storyboard1)
                    {
                        _money_effect_path_animation_X.To = destination.X + current_money_display_location.X;
                        _money_effect_path_animation_Y.To = destination.Y + current_money_display_location.Y;
                        _money_effect_storyboard.Begin(this);
                        MoneyEffect.Visibility = Visibility.Visible;
                        storyboard1 = false;
                    }
                    else
                    {
                        _money_effect_path_animation_X2.To = destination.X + current_money_display_location.X;
                        _money_effect_path_animation_Y2.To = destination.Y + current_money_display_location.Y;
                        _money_effect_storyboard2.Begin(this);
                        MoneyEffect2.Visibility = Visibility.Visible;
                        storyboard1 = true;
                    }
                    
                    money_made_last_iteration = money_made_this_session;
                }
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
                _money_effect_storyboard.Stop(this);
                _money_effect_storyboard2.Stop(this);
                MoneyEffect.Visibility = Visibility.Hidden;
                MoneyEffect2.Visibility = Visibility.Hidden;
                StartStopSessionButton.Content = "Start";
                StartStopSessionButton.FontWeight = FontWeights.Bold;
                ResetSessionButton.IsEnabled = true;
                _running = false;
            }
        }

        private void onResetSessionClick(object sender, RoutedEventArgs e)
        {
            _stopwatch.Reset();
            money_made_last_iteration = 0.0;
            CurrentMoneyDisplay.Text = _starting_money;
            saveTotalMoney();
        }

        private void onHourlyWageChanged(object? sender, EventArgs e)
        {
            Duration d = new Duration(TimeSpan.FromSeconds(72 / ApplicationSettingsStatic.HourlyWage));
            _opacity_animation.Duration = d;
            _opacity_animation2.Duration = d;
            _money_effect_path_animation_X.Duration = d;
            _money_effect_path_animation_Y.Duration = d;
            _money_effect_path_animation_X2.Duration = d;
            _money_effect_path_animation_Y2.Duration = d;
            money_made_last_iteration = 0.0;
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
                    MoneyEffect.FontSize = main_window.Width / 16;
                    StartStopSessionButton.FontSize = main_window.Width / 30.77;
                    ResetSessionButton.FontSize = main_window.Width / 30.77;
                    StartStopSessionButton.Height = main_window.Width / 11.43;
                    ResetSessionButton.Height = main_window.Width / 11.43;
                    StartStopSessionButton.Width = main_window.Width / 4;
                    ResetSessionButton.Width = main_window.Width / 4;
                    ResetSessionTextBlock.LineHeight = main_window.Width / 29.63;
                    radius = main_window.Width / 16;
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
            _money_effect_path_animation_X.From = current_money_display_location.X;
            _money_effect_path_animation_Y.From = current_money_display_location.Y;
            _money_effect_path_animation_X2.From = current_money_display_location.X;
            _money_effect_path_animation_Y2.From = current_money_display_location.Y;
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
