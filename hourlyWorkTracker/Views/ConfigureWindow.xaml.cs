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
        public ConfigureWindow()
        {
            InitializeComponent();
        }
    }
}
