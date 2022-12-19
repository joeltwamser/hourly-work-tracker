using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Threading;
using System.Windows.Media;
using hourlyWorkTracker.ViewModels;
using hourlyWorkTracker.Views;

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string filename = "appstate.conf";
        ApplicationBehaviorViewModel? a;
        public Mutex? One_session
        { get; set; }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            One_session = new Mutex(true, "hourlyWorkTrackerApplication", out bool isNewInstance);
            if (!isNewInstance)
            {
                MessageBox.Show("Already running application...");
                App.Current.Shutdown();
            }
            try
            {
                using FileStream fs = new(filename, FileMode.Open, FileAccess.Read);
                using StreamReader sr = new(fs);
                List<string> allValues = new();
                while (!sr.EndOfStream)
                {
                    string[] keyValue = sr.ReadLine().Split(new char[] { ',' });
                    foreach (string value in keyValue)
                    {
                        allValues.Add(value);
                    }
                }
                Color rect_fill = (Color)ColorConverter.ConvertFromString(allValues[0]);
                Color tick_fore = (Color)ColorConverter.ConvertFromString(allValues[1]);
                Color button_back = (Color)ColorConverter.ConvertFromString(allValues[2]);
                Color button_text_fore = (Color)ColorConverter.ConvertFromString(allValues[3]);
                Color grid_back = (Color)ColorConverter.ConvertFromString(allValues[4]);
                double opacity = Convert.ToDouble(allValues[5]);
                double hourly_wage = Convert.ToDouble(allValues[6]);
                double total_money = Convert.ToDouble(allValues[7]);
                double money_this_session = Convert.ToDouble(allValues[8]);
                double tracker_width = Convert.ToDouble(allValues[9]);
                double tracker_height = Convert.ToDouble(allValues[10]);
                bool pop_to_front = Convert.ToBoolean(allValues[11]);
                double top = Convert.ToDouble(allValues[12]);
                double left = Convert.ToDouble(allValues[13]);
                TimeSpan session_duration = new(Convert.ToInt64(allValues[14]));
                DateTime session_start_time = Convert.ToDateTime(allValues[15]);
                a = new ApplicationBehaviorViewModel(rect_fill, tick_fore, button_back, button_text_fore, grid_back,
                    opacity, hourly_wage, total_money, money_this_session, tracker_width, tracker_height, 
                    pop_to_front, top, left, session_duration, session_start_time);
            }
            catch ( FileNotFoundException )
            {
                a = new ApplicationBehaviorViewModel();
            }
            TrackerView tr = new()
            {
                DataContext = a
            };
            tr.Show();
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            using FileStream fs = new(filename, FileMode.Create, FileAccess.Write);
            using StreamWriter sw = new(fs);
            {
                if (a != null)
                {
                    sw.Write(a.MyApplicationBehavior.RectangleFill + ",");
                    sw.Write(a.MyApplicationBehavior.TickerForeground + ",");
                    sw.Write(a.MyApplicationBehavior.ButtonBackground + ",");
                    sw.Write(a.MyApplicationBehavior.ButtonTextForeground + ",");
                    sw.Write(a.MyApplicationBehavior.GridBackground + ",");
                    sw.Write(a.MyApplicationBehavior.Opacity + ",");
                    sw.Write(a.MyApplicationBehavior.HourlyWage + ",");
                    sw.Write(a.MyApplicationBehavior.TotalMoneyMade + ",");
                    sw.Write(a.MyApplicationBehavior.MoneyMadeThisSession + ",");
                    sw.Write(a.TrackerWidth + ",");
                    sw.Write(a.TrackerHeight + ",");
                    sw.Write(a.PopToFront + ",");
                    sw.Write(a.Top + ",");
                    sw.Write(a.Left + ",");
                    sw.Write(a.SessionDuration.Ticks + ",");
                    sw.Write(a.SessionStartTime);
                    sw.Close();
                    fs.Close();
                }
            }
        }
    }
}
