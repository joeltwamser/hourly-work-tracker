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
using System.IO;

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //fields
        private const string _session_log_filename = "Session Logs.csv";
        private bool resize_window_in_process;
        private const string _starting_money = "$0.00";
        private System.Timers.Timer _timer;
        private Stopwatch _stopwatch;
        private Stopwatch _total_session_time;
        private bool _running = false;
        internal double money_made_last_iteration = 0.0;

        //Dollar Sign Animation fields
        private DoubleAnimation _opacity_animation = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_X = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_Y = new DoubleAnimation();
        private DoubleAnimation _opacity_animation2 = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_X2 = new DoubleAnimation();
        private DoubleAnimation _money_effect_path_animation_Y2 = new DoubleAnimation();
        private Storyboard _money_effect_storyboard;
        private Storyboard _money_effect_storyboard2;
        private double radius = 75.0;
        private bool storyboard1 = true;

        //Powers of Ten Animation fields
        private DoubleAnimation _font_size_animation = new DoubleAnimation();
        private ColorAnimation _foreground_animation = new ColorAnimation();
        private Storyboard _powers_of_ten_storyboard;
        private bool _POT_just_animated = false;

        public MainWindow()
        {
            InitializeComponent();
            Opacity = ApplicationSettingsStatic.MainWindowOpacity;
            ApplicationSettingsStatic.HourlyWageChanged += onHourlyWageChanged;
            ApplicationSettingsStatic.TotalMoneyChanged += onTotalMoneyChanged;
            CurrentMoneyDisplay.Text = "$" + ApplicationSettingsStatic.CurrentSessionMoney.ToString("F2");
            CurrentMoneyDisplay.FontWeight = FontWeights.Bold;
            CurrentMoneyDisplayAddOn.FontWeight = FontWeights.Bold;
            TotalMoneyDisplay.Text = "$" + ApplicationSettingsStatic.TotalMoney.ToString("F2");
            TotalMoneyDisplay.FontWeight = FontWeights.Bold;

            //Dollar Sign ($) Animation Instantiation
            _money_effect_storyboard = new Storyboard();
            _money_effect_storyboard2 = new Storyboard();
            createMoneyEffectAnimation(_opacity_animation, _money_effect_path_animation_X, _money_effect_path_animation_Y, _money_effect_storyboard, MoneyEffect);
            createMoneyEffectAnimation(_opacity_animation2, _money_effect_path_animation_X2, _money_effect_path_animation_Y2, _money_effect_storyboard2, MoneyEffect2);

            //Powers of Ten Animation
            _powers_of_ten_storyboard = new Storyboard();
            createPowersOfTenAnimation(_font_size_animation, _foreground_animation, _powers_of_ten_storyboard, CurrentMoneyDisplayAddOn);

            _stopwatch = new Stopwatch();
            _total_session_time = new Stopwatch();
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

        private void createPowersOfTenAnimation(DoubleAnimation fs_anim, ColorAnimation c_anim, Storyboard st, FrameworkElement fe)
        {
            TextBlock _currentMoneyDisplayAddOn = (TextBlock)fe;
            fs_anim.From = _currentMoneyDisplayAddOn.FontSize * 1.05;
            fs_anim.To = _currentMoneyDisplayAddOn.FontSize;
            //Maybe change this duration to persist longer than just 1 tick of the ticker.
            //I.E. to last longer than just 1 $0.01, regardless of the hourly wage.
            //This might look bad for really really large HourlyWages.
            Duration duration = new Duration(TimeSpan.FromSeconds(36 / ApplicationSettingsStatic.HourlyWage));
            fs_anim.Duration = duration;
            c_anim.From = Colors.Gold;
            c_anim.To = Colors.DarkGreen;
            c_anim.Duration = duration;

            st.Children.Add(fs_anim);
            st.Children.Add(c_anim);
            Storyboard.SetTargetName(fs_anim, fe.Name);
            Storyboard.SetTargetProperty(fs_anim, new PropertyPath(FontSizeProperty));
            Storyboard.SetTargetName(c_anim, fe.Name);
            Storyboard.SetTargetProperty(c_anim, new PropertyPath("(Foreground).(Color)"));
        }

        private void dollarSignAnimation()
        {
            Point current_money_display_location = CurrentMoneyDisplayAddOn.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            current_money_display_location.X += CurrentMoneyDisplayAddOn.ActualWidth;
            Random random = new Random();
            double angle = (double)random.Next(235, 390) * Math.PI / 180.0;
            Point destination = new Point(radius * Math.Cos(angle), radius * Math.Sin(angle));

            if (storyboard1)
            {
                _money_effect_path_animation_X.To = destination.X + current_money_display_location.X;
                _money_effect_path_animation_Y.To = destination.Y + current_money_display_location.Y;
                _money_effect_storyboard.Begin(this, true);
                MoneyEffect.Visibility = Visibility.Visible;
                storyboard1 = false;
            }
            else
            {
                _money_effect_path_animation_X2.To = destination.X + current_money_display_location.X;
                _money_effect_path_animation_Y2.To = destination.Y + current_money_display_location.Y;
                _money_effect_storyboard2.Begin(this, true);
                MoneyEffect2.Visibility = Visibility.Visible;
                storyboard1 = true;
            }
        }

        private void onTimerElapse(object? sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                double money_made_this_session = (_stopwatch.Elapsed.TotalHours * ApplicationSettingsStatic.HourlyWage) + ApplicationSettingsStatic.CurrentSessionMoney;
                double money_made_in_total = (_stopwatch.Elapsed.TotalHours * ApplicationSettingsStatic.HourlyWage) + ApplicationSettingsStatic.TotalMoney;
                double mmts = Math.Round(money_made_this_session, 2);
                double mmli = Math.Round(money_made_last_iteration, 2);

                if (mmts > mmli)
                {
                    dollarSignAnimation();

                    //Handle the POT Effect with the Ticker
                    string nextToDisplay = "$" + money_made_this_session.ToString("F2");
                    if (mmts % 1000 == 0)
                    {
                        CurrentMoneyDisplay.Text = "";
                        _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.3;
                        _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                        CurrentMoneyDisplayAddOn.Text = nextToDisplay;
                        _powers_of_ten_storyboard.Begin(CurrentMoneyDisplayAddOn, true);
                        _POT_just_animated = true;
                    }
                    //In each of these sections, nextToDisplay is guaranteed to have a Length of 6, 5, 4, then 3.
                    else if (mmts % 100 == 0)
                    {
                        CurrentMoneyDisplay.Text = nextToDisplay.Substring(0, nextToDisplay.Length - 6);
                        _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.25;
                        _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                        CurrentMoneyDisplayAddOn.Text = nextToDisplay.Substring(nextToDisplay.Length - 6);
                        _powers_of_ten_storyboard.Begin(CurrentMoneyDisplayAddOn, true);
                        _POT_just_animated = true;
                    }
                    else if (mmts % 10 == 0)
                    {
                        CurrentMoneyDisplay.Text = nextToDisplay.Substring(0, nextToDisplay.Length - 5);
                        _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.2;
                        _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                        CurrentMoneyDisplayAddOn.Text = nextToDisplay.Substring(nextToDisplay.Length - 5);
                        _powers_of_ten_storyboard.Begin(CurrentMoneyDisplayAddOn, true);
                        _POT_just_animated = true;
                    }
                    else if (mmts % 1 == 0)
                    {
                        CurrentMoneyDisplay.Text = nextToDisplay.Substring(0, nextToDisplay.Length - 4);
                        _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.15;
                        _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                        CurrentMoneyDisplayAddOn.Text = nextToDisplay.Substring(nextToDisplay.Length - 4);
                        _powers_of_ten_storyboard.Begin(CurrentMoneyDisplayAddOn, true);
                        _POT_just_animated = true;
                    }
                    //Also I don't update the POT animation when I change my hourly wage.
                    else if ((mmts * 10) % 1 == 0)
                    {
                        CurrentMoneyDisplay.Text = nextToDisplay.Substring(0, nextToDisplay.Length - 2);
                        _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.05;
                        _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                        CurrentMoneyDisplayAddOn.Text = nextToDisplay.Substring(nextToDisplay.Length - 2);
                        _powers_of_ten_storyboard.Begin(CurrentMoneyDisplayAddOn, true);
                        _POT_just_animated = true;
                    }
                    else
                    {
                        if (_POT_just_animated)
                        {
                            CurrentMoneyDisplayAddOn.Text = "";
                            _powers_of_ten_storyboard.Stop();
                        }
                        CurrentMoneyDisplay.Text = nextToDisplay;
                        _POT_just_animated = false;
                    }
                    TotalMoneyDisplay.Text = "$" + money_made_in_total.ToString("F2");
                    money_made_last_iteration = money_made_this_session;
                }
            });
        }

        private void onStartStopSessionClick(object sender, RoutedEventArgs e)
        {
            if (!_running)
            {
                if (ApplicationSettingsStatic.CurrentSessionMoney <= 0)
                {
                    ApplicationSettingsStatic.SessionStartTime = DateTime.Now;
                }
                _stopwatch.Start();
                _total_session_time.Start();
                _timer.Start();
                StartStopSessionButton.Content = "Stop";
                StartStopSessionButton.FontWeight = FontWeights.Bold;
                ResetSessionButton.IsEnabled = false;
                _running = true;
            }
            else
            {
                _stopwatch.Stop();
                _stopwatch.Reset();
                _total_session_time.Stop();
                _timer.Stop();
                _money_effect_storyboard.Stop(this);
                _money_effect_storyboard2.Stop(this);
                MoneyEffect.Visibility = Visibility.Hidden;
                MoneyEffect2.Visibility = Visibility.Hidden;

                //If the Powers of Ten animation is currently running
                if (_powers_of_ten_storyboard.GetCurrentState(CurrentMoneyDisplayAddOn) == 0)
                {
                    string temp = CurrentMoneyDisplay.Text + CurrentMoneyDisplayAddOn.Text;
                    CurrentMoneyDisplayAddOn.Text = "";
                    CurrentMoneyDisplay.Text = temp;
                    _POT_just_animated = false;
                }

                _powers_of_ten_storyboard.Stop(this);
                StartStopSessionButton.Content = "Start";
                StartStopSessionButton.FontWeight = FontWeights.Bold;
                ResetSessionButton.IsEnabled = true;
                ApplicationSettingsStatic.CurrentSessionMoney = save(CurrentMoneyDisplay);
                ApplicationSettingsStatic.TotalMoney = save(TotalMoneyDisplay);
                _running = false;
            }
        }

        private void onResetSessionClick(object sender, RoutedEventArgs e)
        {
            
            money_made_last_iteration = 0.0;
            ApplicationSettingsStatic.TotalMoney = save(TotalMoneyDisplay);

            //Write log of current money made to csv file
            bool exists = File.Exists(_session_log_filename);
            using (FileStream fs = new FileStream(_session_log_filename, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                if(!exists)
                {
                    sw.WriteLine("Session Start Date,Session Duration (hh:mm:ss),Hourly Wage,Wages Earned");
                }
                //Possible eventual bug here in the formatting of the ApplicationSettingsStatic.SessionStartTime.ToString
                //and the _stopwatch.Elapsed.ToString().  These haven't been heavily tested yet.
                sw.Write(ApplicationSettingsStatic.SessionStartTime.ToString() + ",");
                sw.Write((_total_session_time.Elapsed + ApplicationSettingsStatic.SessionDuration).ToString() + ",");
                sw.Write("$" + ApplicationSettingsStatic.HourlyWage.ToString("F2") + "/hr,");
                sw.WriteLine("$" + ApplicationSettingsStatic.CurrentSessionMoney.ToString("F2"));
            }

            _total_session_time.Reset();
            ApplicationSettingsStatic.SessionDuration = new TimeSpan(0);
            ApplicationSettingsStatic.CurrentSessionMoney = 0.0;
            CurrentMoneyDisplay.Text = "$" + ApplicationSettingsStatic.CurrentSessionMoney.ToString("F2");
        }

        private void onHourlyWageChanged(object? sender, EventArgs e)
        {
            Duration d = new Duration(TimeSpan.FromSeconds(72 / ApplicationSettingsStatic.HourlyWage));
            Duration d2 = new Duration(TimeSpan.FromSeconds(36 / ApplicationSettingsStatic.HourlyWage));
            _opacity_animation.Duration = d;
            _opacity_animation2.Duration = d;
            _money_effect_path_animation_X.Duration = d;
            _money_effect_path_animation_Y.Duration = d;
            _money_effect_path_animation_X2.Duration = d;
            _money_effect_path_animation_Y2.Duration = d;
            _font_size_animation.Duration = d2;
            _foreground_animation.Duration = d2;
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
                    _font_size_animation.From = CurrentMoneyDisplay.FontSize * 1.05;
                    _font_size_animation.To = CurrentMoneyDisplay.FontSize;
                    //this doesn't seem right but its the only way I could figure out how to
                    //update CurrentMoneyDisplayAddOn's FontSize once the animations start locking it out.
                    _powers_of_ten_storyboard.Begin(this);
                    _powers_of_ten_storyboard.Stop(this);

                    TotalMoneyDisplay.FontSize = (main_window.Width / 8) * 0.6;
                    MoneyEffect.FontSize = main_window.Width / 16;
                    MoneyEffect2.FontSize = main_window.Width / 16;
                    StartStopSessionButton.FontSize = main_window.Width / 30.77;
                    ResetSessionButton.FontSize = main_window.Width / 30.77;
                    StartStopSessionButton.Height = main_window.Width / 11.43;
                    ResetSessionButton.Height = main_window.Width / 11.43;
                    StartStopSessionButton.Width = main_window.Width / 4;
                    ResetSessionButton.Width = main_window.Width / 4;
                    ResetSessionTextBlock.LineHeight = main_window.Width / 29.63;
                    radius = main_window.Width / 16;

                    setMoneyEffectFromLocation();
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

        private double save(TextBlock tb)
        {
            double temp;
            bool success = double.TryParse(tb.Text.Trim('$'), out temp);
            if (success)
            {
                return temp;
            }
            else
            {
                return -1;
            }
        }

        //maybe make this more general later, and not a function specific to money effect
        private void setMoneyEffectFromLocation()
        {
            Point current_money_display_location = CurrentMoneyDisplayAddOn.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
            current_money_display_location.X += CurrentMoneyDisplayAddOn.ActualWidth;
            Canvas.SetLeft(MoneyEffect, current_money_display_location.X);
            Canvas.SetTop(MoneyEffect, current_money_display_location.Y);
            _money_effect_path_animation_X.From = current_money_display_location.X;
            _money_effect_path_animation_Y.From = current_money_display_location.Y;
            _money_effect_path_animation_X2.From = current_money_display_location.X;
            _money_effect_path_animation_Y2.From = current_money_display_location.Y;
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            setMoneyEffectFromLocation();
        }

        private void onClosing(object sender, EventArgs e)
        {
            if (_running)
            {
                onStartStopSessionClick(StartStopSessionButton, new RoutedEventArgs());
            }
            //ApplicationSettingsStatic.CurrentSessionMoney = save(CurrentMoneyDisplay);
            //ApplicationSettingsStatic.TotalMoney = save(TotalMoneyDisplay);
            ApplicationSettingsStatic.SessionDuration += _total_session_time.Elapsed;
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
