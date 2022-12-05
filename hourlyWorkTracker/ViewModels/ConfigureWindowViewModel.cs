using hourlyWorkTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace hourlyWorkTracker.ViewModels
{
    public class ConfigureWindowViewModel : ApplicationBehaviorViewModel, INotifyPropertyChanged
    {
        public ConfigureWindowViewModel()
        {
            Color myGreen = (Color)System.Windows.Media.ColorConverter.ConvertFromString("#118C4F");
            _my_application_behavior = new ApplicationBehavior(myGreen, myGreen, myGreen, Colors.Black, Colors.Black, 1.0,
                25, false, 0.0, false);
        }

        public ConfigureWindowViewModel(ApplicationBehaviorViewModel a)
        {
            MyApplicationBehavior = a.MyApplicationBehavior;
        }
    }
}
