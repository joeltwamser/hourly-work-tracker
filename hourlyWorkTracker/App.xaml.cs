﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Collections;

namespace hourlyWorkTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string filename = "Settings.conf";

        private void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    List<string> allValues = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        string[] keyValue = sr.ReadLine().Split(new char[] { ',' });
                        foreach (string value in keyValue)
                        {
                            allValues.Add(value);
                        }
                    }
                    ApplicationSettingsStatic.OpacitySliderValue = Convert.ToDouble(allValues[0]);
                    ApplicationSettingsStatic.MainWindowOpacity = Convert.ToDouble(allValues[1]);
                    sr.Close();
                    fs.Close();
                }
            }
            catch ( FileNotFoundException ex)
            {
                ApplicationSettingsStatic.OpacitySliderValue = 100.0;
                ApplicationSettingsStatic.MainWindowOpacity = 1.0;
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(ApplicationSettingsStatic.OpacitySliderValue + ",");
                sw.Write(ApplicationSettingsStatic.MainWindowOpacity);
                sw.Close();
                fs.Close();
            }
        }
    }
}
