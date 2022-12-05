using hourlyWorkTracker.Models;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using hourlyWorkTracker.Services;

namespace hourlyWorkTracker.ViewModels
{
    public class TrackerWindowViewModel : ApplicationBehaviorViewModel, INotifyPropertyChanged
    {
        private IWindowService _my_show_window;

        public TrackerWindowViewModel()
        {
            Color myGreen = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#118C4F");
            _my_application_behavior = new ApplicationBehavior(myGreen, myGreen, myGreen, Colors.Black, Colors.Black, 1.0,
                25, false, 0.0, false);
            _my_show_window = new WindowService();
        }

        public TrackerWindowViewModel(ApplicationBehaviorViewModel a)
        {
            MyApplicationBehavior = a.MyApplicationBehavior;
            _my_show_window = new WindowService();
        }

        protected override void OpenConfigureWindowExecute(object? parameter)
        {
            _my_show_window.ShowWindow(new ConfigureWindowViewModel(this));
        }

        protected override void CloseWindowExecute(object? parameter)
        {
            _my_show_window.CloseWindow(parameter);
        }
    }
}
