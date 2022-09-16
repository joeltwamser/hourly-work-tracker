using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for ConfigureWindow.xaml
    /// </summary>
    
    //My current idea is to pass the main window as a tag to this window and store it in private variable (bad practice?)
    //so I can modify the opacity using the value of the slider.  How else am I supposed to do it?
    public partial class ConfigureWindow : Window
    {
        private bool _entered_once = false;

        public ConfigureWindow()
        {
            InitializeComponent();
            OpacitySlider.Value = ApplicationSettingsStatic.OpacitySliderValue;
        }

        private void closeConfigureWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void updateMainWindowOpacity(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_entered_once)
            {
                _entered_once = true;
                return;
            }
            Slider? opacity_slider = sender as Slider;
            if (opacity_slider == null)
            {
                return;
            }
            //is this bad practice? to just get the main window of the application? idk WPF very well.
            Window? main_window = Application.Current.MainWindow;
            main_window.Opacity = opacity_slider.Value / 100.0;
            ApplicationSettingsStatic.MainWindowOpacity = main_window.Opacity;
            ApplicationSettingsStatic.OpacitySliderValue = opacity_slider.Value;
        }

        private void saveHourlyWage(object sender, RoutedEventArgs e)
        {
            string temp = HourlyWageTextBox.Text.Replace(",", string.Empty);
            double hourly_wage = 0.0;
            bool success = double.TryParse(temp, out hourly_wage);
            if(success)
            {
                ApplicationSettingsStatic.HourlyWage = hourly_wage;
                HourlyWageTextBox.Clear();
                SavedTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Incorrect format");
            }
        }

        private void tabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                SavedTextBlock.Visibility = Visibility.Hidden;
            }
        }

        /*private void onWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DockPanel? cw_dock_panel = sender as DockPanel;
            string tb_name = "DynamicTabControl";
            TabControl my_tab_control;
            if (cw_dock_panel != null)
            {
                UIElementCollection dock_panel_children = cw_dock_panel.Children;
                foreach (object child in dock_panel_children)
                {
                    if (child is TabControl && ((child as TabControl)?.Name) == tb_name)
                    {
                        my_tab_control = (TabControl)child;
                        my_tab_control.Height = cw_dock_panel.Height - 55;  //note I'm currently hard coding the 55 (20 + 35). 35 for the bottom grid where the button is.  I can make that dynamic at some point.
                        break;
                    }
                }
                
            }
        }*/
    }
}
