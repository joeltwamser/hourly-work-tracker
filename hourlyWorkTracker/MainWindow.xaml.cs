using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //fields
        bool resize_window_in_process = false;
        public MainWindow()
        {
            InitializeComponent();
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
            ConfigureWindow my_configure_window = new ConfigureWindow();
            my_configure_window.Show();
        }
    }
}
