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
        public MainWindow()
        {
            InitializeComponent();
        }

        bool resize_window_in_process = false;
        private void resizeWindowBegin(object sender, MouseButtonEventArgs e)
        {
            Rectangle? sender_rectangle = sender as Rectangle;
            if (sender_rectangle != null)
            {
                resize_window_in_process = true;
                sender_rectangle.CaptureMouse();
            }
        }

        private void resizeWindowEnd(object sender, MouseButtonEventArgs e)
        {
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
                    double width = e.GetPosition(main_window).X;
                    double height = e.GetPosition(main_window).Y;
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
                        main_window.Left += width;
                        width = main_window.Width - width;
                        if (width > 0)
                        {
                            main_window.Width = width;
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
                        main_window.Top += height;
                        height = main_window.Height - height;
                        if (height > 0)
                        {
                            main_window.Height = height;
                        }
                    }
                }
            }
        }
    }
}
